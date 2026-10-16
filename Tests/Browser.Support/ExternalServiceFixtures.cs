#region LICENSE
// NemeStats is free software under the GNU General Public License version 3 or later.
// Copyright (C) 2015 Jacob Gordon. See <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading.Tasks;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;

namespace NemeStats.Browser.Support
{
    public sealed class ExternalServiceFixtures : IBoardGameGeekApiClient, IIdentityMessageService, INemeStatsEventTracker
    {
        public readonly ConcurrentQueue<string> Calls = new ConcurrentQueue<string>();
        public readonly ConcurrentQueue<string> ConfirmationLinks = new ConcurrentQueue<string>();
        private readonly string primaryUserName;
        private readonly string secondUserName;
        private readonly Uri baseUrl;
        private readonly int[] importIds;
        private readonly string importCell;
        private readonly int generation;
        public static readonly string[] ImportCells =
            { "debug-desktop", "debug-mobile", "optimized-desktop", "optimized-mobile" };

        public ExternalServiceFixtures(string primaryUserName, string secondUserName, Uri baseUrl, string cell, int generation)
        {
            if (baseUrl.Scheme != Uri.UriSchemeHttps || !baseUrl.IsLoopback ||
                !String.IsNullOrEmpty(baseUrl.UserInfo))
                throw new InvalidOperationException("Fixture callback must use owned local HTTPS.");
            this.primaryUserName = primaryUserName;
            this.secondUserName = secondUserName;
            this.baseUrl = baseUrl;
            var index = Array.IndexOf(ImportCells, cell);
            if (index < 0 || generation < 0 || generation > 1000)
                throw new InvalidOperationException("Unknown Browser QA fixture cell or generation.");
            this.importCell = cell;
            this.generation = generation;
            importIds = new[] { 910101 + generation * 8 + index * 2, 910102 + generation * 8 + index * 2 };
        }

        public List<SearchBoardGameResult> SearchBoardGames(string query, bool exactMatch = false)
        {
            Calls.Enqueue("BGG.Search");
            if (query == "Browser QA error")
                throw new InvalidOperationException("Controlled Browser QA BGG search failure.");
            if (query == "Browser QA empty") return new List<SearchBoardGameResult>();
            if (query != "Browser QA BGG")
                return new List<SearchBoardGameResult>();
            return new List<SearchBoardGameResult> { new SearchBoardGameResult
                { BoardGameId = 910003, BoardGameName = "Browser QA BGG lookup", YearPublished = 2022 } };
        }
        public GameDetails GetGameDetails(int gameId)
        {
            Calls.Enqueue("BGG.Details");
            if (gameId != 910003 && !importIds.Contains(gameId))
                throw new InvalidOperationException("No game fixture is configured for this ID.");
            return new GameDetails
            {
                GameId = gameId,
                Name = gameId == 910003 ? "Browser QA BGG lookup" :
                    "Browser QA import " + importCell + (generation == 0 ? "" : " generation " + generation) +
                    ((gameId - 910101) % 2 == 0 ? " Alpha" : " Beta"),
                YearPublished = gameId == 910003 ? 2022 : (gameId - 910101) % 2 == 0 ? 2020 : 2021,
                Description = String.Concat(Enumerable.Repeat(
                    "Browser QA players explore a deterministic local board game. Each round offers a new choice, " +
                    "and the highest score wins after the final round. This fixture supplies enough description " +
                    "for the actual readmore control to expand and collapse. ", 20)),
                Image = "", Thumbnail = "", MinPlayers = 2, MaxPlayers = 6,
                MinPlayTime = 30, MaxPlayTime = 60, IsExpansion = false,
                BGGRating = 7m, AverageRating = 7.5m, AverageWeight = 2m, Rank = 100,
                Designers = new List<string> { "Browser QA designer" },
                Publishers = new List<string> { "Browser QA publisher" },
                Artists = new List<string> { "Browser QA artist" }
            };
        }
        public UserDetails GetUserDetails(string userName)
        {
            Calls.Enqueue("BGG.User");
            if (userName != primaryUserName && userName != secondUserName)
                throw new InvalidOperationException("No BGG user fixture is configured for this name.");
            return new UserDetails { UserId = userName == primaryUserName ? 900001 : 900002,
                Name = userName, Avatar = "" };
        }
        public List<GameDetails> GetUserGames(string userName)
        {
            Calls.Enqueue("BGG.Games");
            if (userName == secondUserName)
                throw new InvalidOperationException("Controlled Browser QA BGG import failure.");
            if (userName != primaryUserName)
                throw new InvalidOperationException("No import fixture is configured for this name.");
            return new List<GameDetails> { GetGameDetails(importIds[0]), GetGameDetails(importIds[1]) };
        }
        public string GetGameThumbnail(int gameId) { Calls.Enqueue("BGG.Thumbnail"); return GetGameDetails(gameId).Thumbnail; }
        public Task SendAsync(IdentityMessage message)
        {
            Calls.Enqueue("Email.Captured");
            if (message.Subject == "Confirm Your NemeStats Account")
            {
                var match = Regex.Match(message.Body ?? "", "<a href=\"([^\"]+)\">link</a>");
                Uri link;
                if (!match.Success || !Uri.TryCreate(HttpUtility.HtmlDecode(match.Groups[1].Value),
                        UriKind.Absolute, out link) || link.Scheme != Uri.UriSchemeHttps || !link.IsLoopback ||
                    link.Authority != baseUrl.Authority || !String.IsNullOrEmpty(link.UserInfo) ||
                    link.AbsolutePath != "/Account/ConfirmEmail" || !String.IsNullOrEmpty(link.Fragment))
                    throw new InvalidOperationException("Captured confirmation link is not owned local HTTPS.");
                var query = HttpUtility.ParseQueryString(link.Query);
                if (query.Count != 2 || String.IsNullOrEmpty(query["userId"]) ||
                    String.IsNullOrEmpty(query["code"]) || query.GetValues("userId").Length != 1 ||
                    query.GetValues("code").Length != 1)
                    throw new InvalidOperationException("Captured confirmation link has invalid parameters.");
                // Token-bearing links remain in memory only; no body, destination or token is logged.
                ConfirmationLinks.Enqueue(link.AbsoluteUri);
            }
            return Task.FromResult(0);
        }
        public void TrackPlayedGame(ApplicationUser user, TransactionSource source) { Calls.Enqueue("Analytics.PlayedGame"); }
        public void TrackUserRegistration(TransactionSource source) { Calls.Enqueue("Analytics.Registration"); }
        public void TrackGamingGroupCreation(TransactionSource source) { Calls.Enqueue("Analytics.GroupCreation"); }
        public void TrackGameDefinitionCreation(ApplicationUser user, string name) { Calls.Enqueue("Analytics.GameCreation"); }
        public void TrackPlayerCreation(ApplicationUser user) { Calls.Enqueue("Analytics.PlayerCreation"); }
        public void TrackGamingGroupUpdate(ApplicationUser user) { Calls.Enqueue("Analytics.GroupUpdate"); }
    }
}
