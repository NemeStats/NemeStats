#region LICENSE
// NemeStats is free software under the GNU General Public License version 3 or later.
// Copyright (C) 2015 Jacob Gordon. See <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using BusinessLogic.Models.Games;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using StructureMap;
using UI.App_Start;

namespace NemeStats.Browser.Support
{
    public sealed class StagedQaModule : IHttpModule
    {
        private static readonly object Gate = new object();
        private static int moduleInstances;
        private bool registered;
        internal static readonly JavaScriptSerializer Json = new JavaScriptSerializer();
        internal static Dictionary<string, object> Manifest;
        internal static Dictionary<string, object> Fixtures;
        internal static ExternalServiceFixtures External;
        internal static PipeControl Pipe;
        internal static RoutingObserver Routing;
        internal static string ManifestPath;
        internal static volatile bool BootstrapCompleted;
        internal static DateTime OverridesInstalledUtc;
        internal static int NestedScopesVerified;
        internal static DateTime FirstNestedScopeVerifiedUtc;
        internal static readonly System.Collections.Concurrent.ConcurrentQueue<string> Lifecycle =
            new System.Collections.Concurrent.ConcurrentQueue<string>();

        public void Init(HttpApplication app)
        {
            try { InitGuarded(app); }
            catch (Exception error)
            {
                if (Manifest != null && Manifest.ContainsKey("evidenceRoot"))
                {
                    var detail = error.ToString();
                    foreach (var key in new[] { "qaPipeSecret", "qaFixturePassword" })
                        if (Manifest.ContainsKey(key)) detail = detail.Replace((string)Manifest[key], "[redacted]");
                    var connection = Microsoft.Azure.CloudConfigurationManager.GetSetting("Database.ConnectionString", false);
                    if (!String.IsNullOrEmpty(connection)) detail = detail.Replace(connection, "[redacted connection]");
                    File.WriteAllText(Path.Combine((string)Manifest["evidenceRoot"], "task-2-activation-failure.txt"), detail);
                }
                throw;
            }
        }

        private void InitGuarded(HttpApplication app)
        {
            lock (Gate)
            {
                if (Pipe == null)
                {
                    ManifestPath = ConfigurationManager.AppSettings["BrowserQa.RunManifest"];
                    if (String.IsNullOrEmpty(ManifestPath)) throw new InvalidOperationException("Missing protected QA manifest.");
                    AssertManifestAcl(ManifestPath);
                    Manifest = Json.Deserialize<Dictionary<string, object>>(File.ReadAllText(ManifestPath));
                    var run = (string)Manifest["runId"];
                    var safe = SafetyPreflight.ValidateEffectiveConfiguration(run);
                    var database = (Dictionary<string, object>)Manifest["database"];
                    if (!Equals(database["server"], safe.Server) || !Equals(database["catalog"], safe.Catalog) ||
                        !Equals(database["ownerRunId"], run) || !Equals(database["catalogExistedBefore"], false) ||
                        !Equals(Manifest["preparationReady"], true) || !Equals(Manifest["firewallVerified"], true) ||
                        !Equals(Manifest["evidenceOwnerSid"], WindowsIdentity.GetCurrent().User.Value) ||
                        !String.Equals(Path.GetFullPath((string)Manifest["stagedHost"]).TrimEnd('\\'),
                            HttpRuntime.AppDomainAppPath.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Guarded QA host ownership mismatch.");
                    AssertDatabaseOwnership(safe, run);
                    var scope = StructuremapMvc.StructureMapDependencyScope;
                    if (scope == null || scope.Container == null || scope.CurrentNestedContainer != null)
                        throw new InvalidOperationException("Root scope unavailable or overrides are too late.");
                    External = new ExternalServiceFixtures("qa_" + run.Replace("-", ""),
                        "qa_second_" + run.Replace("-", ""), new Uri((string)Manifest["httpsOrigin"]),
                        Manifest.ContainsKey("qaFixtureCell") ? (string)Manifest["qaFixtureCell"] : "debug-desktop",
                        Manifest.ContainsKey("qaFixtureGeneration") ? Convert.ToInt32(Manifest["qaFixtureGeneration"]) : 0);
                    scope.Container.Configure(c =>
                    {
                        c.For<IBoardGameGeekApiClient>().Use(External);
                        c.For<IIdentityMessageService>().Use(External);
                        c.For<INemeStatsEventTracker>().Use(External);
                    });
                    OverridesInstalledUtc = DateTime.UtcNow;
                    Lifecycle.Enqueue("RootOverridesInstalled:no-current-request-scope");
                    var resolver = Microsoft.AspNet.SignalR.GlobalHost.DependencyResolver;
                    Routing = new RoutingObserver(resolver.Resolve<Microsoft.AspNet.SignalR.Messaging.IMessageBus>());
                    resolver.Register(typeof(Microsoft.AspNet.SignalR.Messaging.IMessageBus), () => Routing);
                    Lifecycle.Enqueue("OriginalSignalRMessageBusObserverInstalled");
                    System.Web.Mvc.GlobalFilters.Filters.Add(new QaExceptionObserver(), int.MaxValue);
                    Task.Run(() => Seed(scope.Container)).GetAwaiter().GetResult();
                    Lifecycle.Enqueue("OwnedPasswordFixturesConfirmed");
                    Pipe = new PipeControl(run, (string)Manifest["qaPipeName"], (string)Manifest["qaPipeSecret"]);
                    Pipe.Start();
                }
                moduleInstances++;
                registered = true;
                Lifecycle.Enqueue("ModuleInstanceRegistered:" + moduleInstances);
            }
            app.BeginRequest += (sender, args) => Lifecycle.Enqueue("BeginRequest:" + app.Request.Path);
            app.PostMapRequestHandler += (sender, args) => Lifecycle.Enqueue("PostMapRequestHandler:" + app.Request.Path);
            app.PostAcquireRequestState += (sender, args) =>
            {
                var nested = StructuremapMvc.StructureMapDependencyScope.CurrentNestedContainer;
                if (nested == null || !ReferenceEquals(nested.GetInstance<IBoardGameGeekApiClient>(), External) ||
                    !ReferenceEquals(nested.GetInstance<IIdentityMessageService>(), External) ||
                    !ReferenceEquals(nested.GetInstance<INemeStatsEventTracker>(), External))
                    throw new InvalidOperationException("Tested nested scope did not inherit root boundaries.");
                System.Threading.Interlocked.Increment(ref NestedScopesVerified);
                lock (Gate)
                    if (FirstNestedScopeVerifiedUtc == default(DateTime))
                        FirstNestedScopeVerifiedUtc = DateTime.UtcNow;
                Lifecycle.Enqueue("NestedRootBoundariesVerified:" + app.Request.Path);
            };
            app.PreRequestHandlerExecute += (sender, args) => Lifecycle.Enqueue("PreRequestHandlerExecute:" + app.Request.Path);
            app.PostRequestHandlerExecute += (sender, args) => Lifecycle.Enqueue("PostRequestHandlerExecute:" + app.Request.Path);
            app.EndRequest += (sender, args) =>
            {
                Lifecycle.Enqueue("EndRequest:" + app.Request.Path + ":" + app.Response.StatusCode);
                if (app.Request.IsSecureConnection && app.Request.Url.IsLoopback &&
                    app.Response.StatusCode == 200 && NestedScopesVerified > 0)
                    BootstrapCompleted = true;
            };
        }

        private static async Task Seed(IContainer root)
        {
            if (Manifest.ContainsKey("fixtures"))
            {
                Fixtures = (Dictionary<string, object>)Manifest["fixtures"];
                if (!Equals(Fixtures["ownerRunId"], Manifest["runId"]) ||
                    !Equals(Fixtures["userName"], "qa_" + ((string)Manifest["runId"]).Replace("-", "")))
                    throw new InvalidOperationException("Recorded fixture identity mismatch.");
                using (var db = new NemeStatsDbContext())
                {
                    var userId = (string)Fixtures["userId"];
                    var userName = (string)Fixtures["userName"];
                    var groupName = "Browser QA " + (string)Manifest["runId"];
                    var groupId = Convert.ToInt32(Fixtures["gamingGroupId"]);
                    if (!db.Users.Any(u => u.Id == userId && u.UserName == userName &&
                            u.EmailConfirmed && u.CurrentGamingGroupId == groupId) ||
                        !db.GamingGroups.Any(g => g.Id == groupId && g.OwningUserId == userId &&
                            g.Name == groupName))
                        throw new InvalidOperationException("Existing fixture ownership mismatch.");
                    using (var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db)))
                    {
                        var authenticated = await manager.FindAsync(userName, (string)Manifest["qaFixturePassword"])
                            .ConfigureAwait(false);
                        if (authenticated == null || authenticated.Id != userId)
                            throw new InvalidOperationException("Recorded fixture is not password-capable.");
                    }
                }
                SeedOwnedGame(root);
                await SeedExtendedFixtures(root).ConfigureAwait(false);
                return;
            }
            using (var nested = root.GetNestedContainer())
            using (var db = new NemeStatsDbContext())
            using (var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db)))
            {
                var run = (string)Manifest["runId"];
                if (db.Users.Any() || db.GamingGroups.Any())
                    throw new InvalidOperationException("Bootstrap refuses an occupied fixture catalog.");
                var user = new ApplicationUser { UserName = "qa_" + run.Replace("-", ""),
                    Email = "qa_" + run.Replace("-", "") + "@example.invalid", EmailConfirmed = true };
                var password = (string)Manifest["qaFixturePassword"];
                var result = await manager.CreateAsync(user, password).ConfigureAwait(false);
                if (!result.Succeeded) throw new InvalidOperationException("Synthetic Identity creation failed.");
                var created = nested.GetInstance<IGamingGroupSaver>().CreateNewGamingGroup(
                    "Browser QA " + run, TransactionSource.WebApplication, user);
                nested.GetInstance<IBoardGameGeekUserSaver>().CreateUserDefintion(
                    new CreateBoardGameGeekUserDefinitionRequest { Name = "qa_" + run.Replace("-", ""),
                        BoardGameGeekUserId = 900001, Avatar = "" }, user);
                nested.GetInstance<IDataContext>().CommitAllChanges();
                Fixtures = new Dictionary<string, object> { { "userId", user.Id }, { "userName", user.UserName },
                    { "gamingGroupId", created.NewlyCreatedGamingGroup.Id }, { "ownerRunId", run } };
                Manifest["fixtures"] = Fixtures;
                File.WriteAllText(ManifestPath, Json.Serialize(Manifest));
            }
            SeedOwnedGame(root);
            await SeedExtendedFixtures(root).ConfigureAwait(false);
        }

        private static async Task SeedExtendedFixtures(IContainer root)
        {
            var run = (string)Manifest["runId"];
            var secondName = "qa_second_" + run.Replace("-", "");
            var secondGroupName = "Browser QA second " + run;
            using (var nested = root.GetNestedContainer())
            using (var db = new NemeStatsDbContext())
            using (var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db)))
            {
                ApplicationUser second;
                int secondGroupId;
                if (Fixtures.ContainsKey("secondUserId"))
                {
                    var secondId = (string)Fixtures["secondUserId"];
                    secondGroupId = Convert.ToInt32(Fixtures["secondGamingGroupId"]);
                    second = db.Users.SingleOrDefault(u => u.Id == secondId && u.UserName == secondName);
                    if (second == null || !Equals(Fixtures["secondUserName"], secondName) ||
                        !second.EmailConfirmed || second.CurrentGamingGroupId != secondGroupId ||
                        !db.GamingGroups.Any(g => g.Id == secondGroupId && g.OwningUserId == secondId &&
                            g.Name == secondGroupName))
                        throw new InvalidOperationException("Recorded second fixture ownership mismatch.");
                }
                else
                {
                    if (db.Users.Any(u => u.UserName == secondName) ||
                        db.GamingGroups.Any(g => g.Name == secondGroupName))
                        throw new InvalidOperationException("Extension refuses unrecorded second fixtures.");
                    second = new ApplicationUser { UserName = secondName,
                        Email = secondName + "@example.invalid", EmailConfirmed = true };
                    var result = await manager.CreateAsync(second, (string)Manifest["qaFixturePassword"])
                        .ConfigureAwait(false);
                    if (!result.Succeeded) throw new InvalidOperationException("Second synthetic Identity creation failed.");
                    var created = nested.GetInstance<IGamingGroupSaver>().CreateNewGamingGroup(
                        secondGroupName, TransactionSource.WebApplication, second);
                    secondGroupId = created.NewlyCreatedGamingGroup.Id;
                    nested.GetInstance<IBoardGameGeekUserSaver>().CreateUserDefintion(
                        new CreateBoardGameGeekUserDefinitionRequest { Name = secondName,
                            BoardGameGeekUserId = 900002, Avatar = "" }, second);
                    nested.GetInstance<IDataContext>().CommitAllChanges();
                    Fixtures["secondUserId"] = second.Id;
                    Fixtures["secondUserName"] = secondName;
                    Fixtures["secondGamingGroupId"] = secondGroupId;
                    File.WriteAllText(ManifestPath, Json.Serialize(Manifest));
                }
                var primaryId = (string)Fixtures["userId"];
                var primary = db.Users.Single(u => u.Id == primaryId);
                var primaryGroupId = Convert.ToInt32(Fixtures["gamingGroupId"]);
                if (primary.UserName != "qa_" + run.Replace("-", "") ||
                    primary.CurrentGamingGroupId != primaryGroupId ||
                    !db.GamingGroups.Any(g => g.Id == primaryGroupId && g.OwningUserId == primaryId &&
                        g.Name == "Browser QA " + run))
                    throw new InvalidOperationException("Primary fixture ownership mismatch.");
                foreach (var user in new[] { primary, second })
                {
                    var authenticated = await manager.FindAsync(user.UserName, (string)Manifest["qaFixturePassword"])
                        .ConfigureAwait(false);
                    if (!user.EmailConfirmed || authenticated == null || authenticated.Id != user.Id)
                        throw new InvalidOperationException("Extended fixture is not confirmed and password-capable.");
                    var expectedGroupId = user.Id == primaryId ? primaryGroupId : secondGroupId;
                    var expectedEmail = user.UserName + "@example.invalid";
                    if (!db.Users.Any(u => u.Id == user.Id && u.UserName == user.UserName &&
                        u.Email == expectedEmail && u.EmailConfirmed && u.CurrentGamingGroupId == expectedGroupId))
                        throw new InvalidOperationException("Persisted confirmed account ownership mismatch.");
                    var expectedBggId = user.Id == primaryId ? 900001 : 900002;
                    if (!db.BoardGameGeekUserDefinitions.Any(b => b.ApplicationUserId == user.Id &&
                        b.Name == user.UserName && b.Id == expectedBggId))
                        throw new InvalidOperationException("Recorded BGG fixture ownership mismatch.");
                }
                SeedGroupContent(nested, db, primary, primaryGroupId, "primary", 20);
                SeedGroupContent(nested, db, second, secondGroupId, "second", 2);
                Fixtures["passwordCapable"] = true;
                Fixtures["ownershipVerified"] = true;
                Fixtures["boardGameGeekUserName"] = primary.UserName;
                Fixtures["secondBoardGameGeekUserName"] = secondName;
                Fixtures["bggSearchSuccess"] = "Browser QA BGG";
                Fixtures["bggSearchEmpty"] = "Browser QA empty";
                Fixtures["bggSearchFailure"] = "Browser QA error";
                Fixtures["bggSearchIds"] = new[] { 910003 };
                Fixtures["bggSearchNames"] = new[] { "Browser QA BGG lookup" };
                var batches = new Dictionary<string, object>();
                var generation = Manifest.ContainsKey("qaFixtureGeneration") ? Convert.ToInt32(Manifest["qaFixtureGeneration"]) : 0;
                var suffix = generation == 0 ? "" : " generation " + generation;
                for (var i = 0; i < ExternalServiceFixtures.ImportCells.Length; i++)
                {
                    var cell = ExternalServiceFixtures.ImportCells[i];
                    batches[cell] = new Dictionary<string, object>
                    {
                        { "gameIds", new[] { 910101 + generation * 8 + i * 2, 910102 + generation * 8 + i * 2 } },
                        { "gameNames", new[] { "Browser QA import " + cell + suffix + " Alpha (2020)",
                            "Browser QA import " + cell + suffix + " Beta (2021)" } }
                    };
                }
                Fixtures["importBatches"] = batches;
                var selectedBatch = (Dictionary<string, object>)batches[
                    Manifest.ContainsKey("qaFixtureCell") ? (string)Manifest["qaFixtureCell"] : "debug-desktop"];
                Fixtures["bggImportIds"] = selectedBatch["gameIds"];
                Fixtures["bggImportNames"] = selectedBatch["gameNames"];
                Fixtures["primaryPlayerIds"] = Fixtures["playerIds"];
                File.WriteAllText(ManifestPath, Json.Serialize(Manifest));
                var fixtureEvidenceName = Manifest.ContainsKey("qaEvidenceMode") &&
                    (string)Manifest["qaEvidenceMode"] == "upgraded" ? "upgraded-fixtures.json" : "task-3-fixtures.json";
                File.WriteAllText(Path.Combine((string)Manifest["evidenceRoot"], fixtureEvidenceName),
                    Json.Serialize(new { runId = run, fixtures = Fixtures,
                        identityOperation = "UserManager.CreateAsync(user,password)",
                        passwordCheck = "UserManager.FindAsync", rootOverridesBeforeSeed = OverridesInstalledUtc != default(DateTime) }));
                Lifecycle.Enqueue("ExtendedOwnedFixturesConfirmed");
            }
        }

        private static void SeedGroupContent(IContainer nested, NemeStatsDbContext db,
            ApplicationUser user, int groupId, string prefix, int extraGames)
        {
            var players = new List<int>();
            var linked = db.Players.Single(p => p.GamingGroupId == groupId && p.ApplicationUserId == user.Id);
            if (!linked.Active) throw new InvalidOperationException("Owned linked player is inactive.");
            players.Add(linked.Id);
            if (linked.Name != user.UserName)
                throw new InvalidOperationException("Owned linked player identity mismatch.");
            for (int i = 1; i <= (prefix == "primary" ? 2 : 1); i++)
            {
                var key = prefix + "Player" + i + "Id";
                var name = "Browser QA " + prefix + " player " + i + " " + Manifest["runId"];
                var player = db.Players.SingleOrDefault(p => p.GamingGroupId == groupId && p.Name == name);
                if (player != null && !Fixtures.ContainsKey(key))
                    throw new InvalidOperationException("Unrecorded owned player cannot be adopted.");
                if (player == null)
                {
                    if (Fixtures.ContainsKey(key)) throw new InvalidOperationException("Recorded owned player is missing.");
                    player = nested.GetInstance<IPlayerSaver>().CreatePlayer(
                        new CreatePlayerRequest { Name = name, GamingGroupId = groupId }, user);
                }
                if (!player.Active || player.ApplicationUserId != null ||
                    (Fixtures.ContainsKey(key) && Convert.ToInt32(Fixtures[key]) != player.Id))
                    throw new InvalidOperationException("Recorded owned player identity mismatch.");
                Fixtures[key] = player.Id;
                File.WriteAllText(ManifestPath, Json.Serialize(Manifest));
                players.Add(player.Id);
            }
            var games = new List<int>();
            if (prefix == "primary") games.Add(Convert.ToInt32(Fixtures["gameDefinitionId"]));
            for (int i = 1; i <= extraGames; i++)
            {
                var key = prefix + "Game" + i + "Id";
                var name = "Browser QA " + prefix + " game " + i.ToString("D2") + " " + Manifest["runId"];
                var game = db.GameDefinitions.SingleOrDefault(g => g.GamingGroupId == groupId && g.Name == name);
                if (game != null && !Fixtures.ContainsKey(key))
                    throw new InvalidOperationException("Unrecorded owned game cannot be adopted.");
                if (game == null)
                {
                    if (Fixtures.ContainsKey(key)) throw new InvalidOperationException("Recorded owned game is missing.");
                    game = nested.GetInstance<ICreateGameDefinitionComponent>().Execute(
                        new CreateGameDefinitionRequest { Name = name, GamingGroupId = groupId,
                            BoardGameGeekGameDefinitionId = prefix == "primary" && i == 1 ? (int?)910003 : null }, user);
                }
                if (!game.Active || (Fixtures.ContainsKey(key) && Convert.ToInt32(Fixtures[key]) != game.Id))
                    throw new InvalidOperationException("Recorded owned game identity mismatch.");
                Fixtures[key] = game.Id;
                File.WriteAllText(ManifestPath, Json.Serialize(Manifest));
                games.Add(game.Id);
                if (prefix == "primary" && i == 1)
                {
                    if (game.BoardGameGeekGameDefinitionId != 910003 ||
                        !db.BoardGameGeekGameDefinitions.Any(b => b.Id == 910003 && b.Name == "Browser QA BGG lookup" &&
                            b.Description.Length > 1000))
                        throw new InvalidOperationException("Persisted readmore metadata fixture mismatch.");
                    Fixtures["readmoreGameDefinitionId"] = game.Id;
                    Fixtures["readmoreBoardGameGeekId"] = 910003;
                }
            }
            Fixtures[prefix == "primary" ? "playerIds" : "secondPlayerIds"] = players.ToArray();
            Fixtures[prefix == "primary" ? "gameDefinitionIds" : "secondGameDefinitionIds"] = games.ToArray();
            var playedKey = prefix == "primary" ? "playedGameId" : "secondPlayedGameId";
            var notes = "Browser QA " + prefix + " played game " + Manifest["runId"];
            var played = db.PlayedGames.SingleOrDefault(p => p.GamingGroupId == groupId && p.Notes == notes);
            if (played != null && !Fixtures.ContainsKey(playedKey))
                throw new InvalidOperationException("Unrecorded played game cannot be adopted.");
            if (played == null)
            {
                if (Fixtures.ContainsKey(playedKey))
                    throw new InvalidOperationException("Recorded played game is missing.");
                var saved = nested.GetInstance<ICreatePlayedGameComponent>().Execute(new NewlyCompletedGame
                {
                    GamingGroupId = groupId, GameDefinitionId = games[0], Notes = notes,
                    DatePlayed = new DateTime(2020, 1, 2), TransactionSource = TransactionSource.WebApplication,
                    PlayerRanks = players.Select((id, index) => new PlayerRank
                        { PlayerId = id, GameRank = index + 1, PointsScored = 30 - index * 10 }).ToList()
                }, user);
                Fixtures[playedKey] = saved.Id;
                File.WriteAllText(ManifestPath, Json.Serialize(Manifest));
                played = db.PlayedGames.Single(p => p.Id == saved.Id);
            }
            var persistedResults = db.PlayerGameResults.Where(result => result.PlayedGameId == played.Id).ToList();
            if (played.Id != Convert.ToInt32(Fixtures[playedKey]) ||
                played.CreatedByApplicationUserId != user.Id || played.GameDefinitionId != games[0] ||
                played.NumberOfPlayers != players.Count || played.DatePlayed != new DateTime(2020, 1, 2) ||
                !persistedResults.Select(p => p.PlayerId).OrderBy(id => id)
                    .SequenceEqual(players.OrderBy(id => id)) ||
                persistedResults.Any(p => p.GameRank != players.IndexOf(p.PlayerId) + 1))
                throw new InvalidOperationException("Persisted played-game fixture ownership mismatch.");
            if (db.Players.Count(p => p.GamingGroupId == groupId && players.Contains(p.Id) && p.Active) != players.Count ||
                db.GameDefinitions.Count(g => g.GamingGroupId == groupId && games.Contains(g.Id) && g.Active) != games.Count)
                throw new InvalidOperationException("Persisted group content verification failed.");
            Fixtures[prefix + "PersistedPlayerCount"] = players.Count;
            Fixtures[prefix + "PersistedGameCount"] = games.Count;
        }

        private static void SeedOwnedGame(IContainer root)
        {
            var run = (string)Manifest["runId"];
            var userId = (string)Fixtures["userId"];
            var groupId = Convert.ToInt32(Fixtures["gamingGroupId"]);
            var name = "Browser QA game " + run;
            using (var nested = root.GetNestedContainer())
            using (var db = new NemeStatsDbContext())
            {
                var game = db.GameDefinitions.SingleOrDefault(g => g.GamingGroupId == groupId && g.Name == name);
                if (game != null && !Fixtures.ContainsKey("gameDefinitionId"))
                    throw new InvalidOperationException("Unrecorded original game cannot be adopted.");
                if (game == null)
                {
                    if (Fixtures.ContainsKey("gameDefinitionId"))
                        throw new InvalidOperationException("Recorded owned game is missing.");
                    var user = db.Users.Single(u => u.Id == userId);
                    nested.GetInstance<ICreateGameDefinitionComponent>().Execute(
                        new CreateGameDefinitionRequest { Name = name, GamingGroupId = groupId }, user);
                    game = db.GameDefinitions.Single(g => g.GamingGroupId == groupId && g.Name == name);
                    Lifecycle.Enqueue("OwnedGameCreated:existing-domain-operation");
                }
                if (!game.Active || (Fixtures.ContainsKey("gameDefinitionId") &&
                    Convert.ToInt32(Fixtures["gameDefinitionId"]) != game.Id))
                    throw new InvalidOperationException("Recorded owned game identity mismatch.");
                Fixtures["gameDefinitionId"] = game.Id;
                Fixtures["gameDefinitionName"] = name;
                Manifest["fixtures"] = Fixtures;
                File.WriteAllText(ManifestPath, Json.Serialize(Manifest));
                Lifecycle.Enqueue("OwnedGamePersistenceConfirmed:" + game.Id);
            }
        }

        private static void AssertManifestAcl(string path)
        {
            var sid = WindowsIdentity.GetCurrent().User;
            var acl = File.GetAccessControl(path);
            if (!acl.AreAccessRulesProtected || !acl.GetOwner(typeof(SecurityIdentifier)).Equals(sid))
                throw new InvalidOperationException("Manifest ACL is not owned and protected.");
            foreach (FileSystemAccessRule rule in acl.GetAccessRules(true, true, typeof(SecurityIdentifier)))
                if (rule.AccessControlType == AccessControlType.Allow && !rule.IdentityReference.Equals(sid))
                    throw new InvalidOperationException("Manifest permits an unrelated identity.");
        }

        private static void AssertDatabaseOwnership(SafetyPreflightResult safe, string run)
        {
            using (var db = new SqlConnection("Data Source=" + safe.Server + ";Initial Catalog=" + safe.Catalog + ";Integrated Security=True"))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandText = "SELECT CONVERT(nvarchar(36),value) FROM sys.extended_properties WHERE class=0 AND name=N'NemeStatsBrowserQaRunId'";
                    if (!Equals(query.ExecuteScalar(), run)) throw new InvalidOperationException("Catalog ownership marker mismatch.");
                    query.CommandText = "SELECT CONVERT(nvarchar(32),value) FROM sys.extended_properties WHERE class=0 AND name=N'NemeStatsBrowserQaRole'";
                    if (!Equals(query.ExecuteScalar(), "application")) throw new InvalidOperationException("Catalog role marker mismatch.");
                }
            }
        }

        public void Dispose()
        {
            lock (Gate)
            {
                if (!registered) return;
                registered = false;
                moduleInstances--;
                Lifecycle.Enqueue("ModuleInstanceReleased:" + moduleInstances);
                // IHttpModule belongs to one pooled HttpApplication instance.
                // Keep the shared AppDomain listener until the last instance closes.
                if (moduleInstances != 0) return;
                if (Pipe != null)
                {
                    Lifecycle.Enqueue("ModuleDisposeStarted");
                    File.WriteAllText(Path.Combine((string)Manifest["evidenceRoot"], "task-2-dispose-started.json"),
                        Json.Serialize(new { runId = Manifest["runId"],
                            pid = System.Diagnostics.Process.GetCurrentProcess().Id,
                            appDomainId = AppDomain.CurrentDomain.Id, lifecycle = Lifecycle.ToArray() }));
                    Pipe.Dispose();
                    var receipt = Path.Combine((string)Manifest["evidenceRoot"], "task-2-dispose.json");
                    var temporary = receipt + ".tmp";
                    File.WriteAllText(temporary,
                        Json.Serialize(new { runId = Manifest["runId"], disposed = true,
                            pid = System.Diagnostics.Process.GetCurrentProcess().Id,
                            appDomainId = AppDomain.CurrentDomain.Id, utc = DateTime.UtcNow.ToString("o") }));
                    if (!File.Exists(receipt)) File.Move(temporary, receipt);
                    else File.Delete(temporary);
                }
            }
        }

        private sealed class QaExceptionObserver : System.Web.Mvc.IExceptionFilter
        {
            public void OnException(System.Web.Mvc.ExceptionContext context)
            {
                // Observe before the product's external exception filter; never handle it.
                var detail = context.Exception.ToString();
                foreach (var key in new[] { "qaPipeSecret", "qaFixturePassword" })
                    if (Manifest.ContainsKey(key)) detail = detail.Replace((string)Manifest[key], "[redacted]");
                var connection = Microsoft.Azure.CloudConfigurationManager.GetSetting("Database.ConnectionString", false);
                if (!String.IsNullOrEmpty(connection)) detail = detail.Replace(connection, "[redacted connection]");
                Lifecycle.Enqueue("MvcException:" + detail);
            }
        }
    }
}
