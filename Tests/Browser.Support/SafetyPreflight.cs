#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Azure;

namespace NemeStats.Browser.Support
{
    public sealed class SafetyPreflightResult
    {
        internal SafetyPreflightResult(string server, string catalog, string fingerprint)
        {
            Server = server;
            Catalog = catalog;
            Fingerprint = fingerprint;
        }

        public string Server { get; private set; }
        public string Catalog { get; private set; }
        public string Fingerprint { get; private set; }
    }

    public static class SafetyPreflight
    {
        private const string ConnectionSetting = "Database.ConnectionString";
        private const string CatalogPrefix = "NemeStatsJQueryMigration_";

        // Reads the effective staged setting but deliberately does not create a DbContext,
        // connect to SQL, create a catalog, run migrations, or bootstrap the application.
        public static SafetyPreflightResult ValidateEffectiveConfiguration(string expectedRunId)
        {
            var raw = CloudConfigurationManager.GetSetting(ConnectionSetting, false);
            if (String.IsNullOrWhiteSpace(raw))
                throw new InvalidOperationException("The effective staged database connection setting is missing.");

            var result = ValidateConnectionString(raw, expectedRunId);
            var runId = Guid.Parse(expectedRunId);
            var port = 51000 + (Int64.Parse(runId.ToString("N").Substring(0, 8),
                NumberStyles.HexNumber, CultureInfo.InvariantCulture) % 10000);
            var origin = "https://localhost:" + port.ToString(CultureInfo.InvariantCulture);
            if (CloudConfigurationManager.GetSetting("urlRoot", false) != origin ||
                CloudConfigurationManager.GetSetting("emailConfirmationCallbackUrl", false) !=
                    origin + "/Account/ConfirmEmail")
                throw new InvalidOperationException("The effective callback and urlRoot must use this run's HTTPS origin.");
            return result;
        }

        public static SafetyPreflightResult ValidateConnectionString(string raw, string expectedRunId)
        {
            Guid runId;
            if (!Guid.TryParse(expectedRunId, out runId) || runId == Guid.Empty ||
                runId.ToString("D") != expectedRunId)
                throw new ArgumentException("Expected a canonical run GUID.", "expectedRunId");
            if (String.IsNullOrWhiteSpace(raw))
                throw new InvalidOperationException("The effective staged database connection setting is missing.");

            var builder = new SqlConnectionStringBuilder(raw);
            if (String.IsNullOrWhiteSpace(builder.DataSource) || String.IsNullOrWhiteSpace(builder.InitialCatalog))
                throw new InvalidOperationException("The database endpoint and catalog must be explicit.");

            if (!IsLocalEndpoint(builder.DataSource))
                throw new InvalidOperationException("Only an explicitly local SQL Server or LocalDB endpoint is allowed.");
            if (!String.IsNullOrWhiteSpace(builder.AttachDBFilename) || builder.UserInstance)
                throw new InvalidOperationException("Attached database files and user instances are not allowed.");
            if (!builder.IntegratedSecurity || !String.IsNullOrWhiteSpace(builder.UserID) ||
                !String.IsNullOrWhiteSpace(builder.Password))
                throw new InvalidOperationException("The staged local database target must use integrated authentication.");

            var expectedCatalog = CatalogPrefix + runId.ToString("N");
            if (!String.Equals(builder.InitialCatalog, expectedCatalog, StringComparison.Ordinal))
                throw new InvalidOperationException("The catalog must be the generated catalog owned by this run.");

            // Never include the raw connection string in reports: it may contain credentials.
            var fingerprint = String.Concat("Server=", builder.DataSource, ";Catalog=", builder.InitialCatalog);
            return new SafetyPreflightResult(builder.DataSource, builder.InitialCatalog, fingerprint);
        }

        private static bool IsLocalEndpoint(string dataSource)
        {
            var endpoint = dataSource.Trim();
            if (endpoint.StartsWith("(localdb)\\", StringComparison.OrdinalIgnoreCase))
                return Regex.IsMatch(endpoint, @"^\(localdb\)\\[A-Za-z0-9_-]+$",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            var host = endpoint;
            var comma = host.IndexOf(',');
            if (comma >= 0)
            {
                var port = host.Substring(comma + 1);
                int parsedPort;
                if (!Int32.TryParse(port, out parsedPort) || parsedPort < 1 || parsedPort > 65535)
                    return false;
                host = host.Substring(0, comma);
            }

            var instance = host.IndexOf('\\');
            if (instance >= 0)
            {
                if (!Regex.IsMatch(host.Substring(instance + 1), @"^[A-Za-z0-9_-]+$"))
                    return false;
                host = host.Substring(0, instance);
            }

            return host.Equals(".", StringComparison.OrdinalIgnoreCase)
                || host.Equals("(local)", StringComparison.OrdinalIgnoreCase)
                || host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                || host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)
                || host.Equals("::1", StringComparison.OrdinalIgnoreCase);
        }
    }
}
