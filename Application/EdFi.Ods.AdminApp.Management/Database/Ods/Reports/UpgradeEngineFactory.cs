// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Common.Configuration;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.Extensions.Options;
using System.Reflection;
using DbUp;
using DbUp.Builder;
using DbUp.Engine;

namespace EdFi.Ods.AdminApp.Management.Database.Ods.Reports
{
    public interface IUpgradeEngineFactory
    {
        UpgradeEngine Create(ReportsConfig config);
    }

    public class UpgradeEngineFactory : IUpgradeEngineFactory
    {
        private static IOptions<AppSettings> _appSettings;
        public UpgradeEngineFactory(IOptions<AppSettings> appSettings)
        {
                _appSettings = appSettings;

        }

        public UpgradeEngine Create(ReportsConfig config)
        {
            return UpgradeEngineBuilder(config.ConnectionString)
                .WithScriptsEmbeddedInAssemblies(new []{Assembly.GetExecutingAssembly()}, 
                    filter => filter.Contains($"{config.ScriptFolder}."))
                .LogToAutodetectedLog()
                .Build();
        }

        private static UpgradeEngineBuilder UpgradeEngineBuilder(string connectionString)
        {
            var isPostgreSql = ApiConfigurationConstants.PostgreSQL.Equals(_appSettings.Value.DatabaseEngine, StringComparison.InvariantCultureIgnoreCase);

            if (isPostgreSql)
            {
                return DeployChanges.To
                    .PostgresqlDatabase(connectionString)
                    .JournalToPostgresqlTable("public", "DeployJournal");
            }

            return DeployChanges.To
                .SqlDatabase(connectionString)
                .JournalToSqlTable("dbo", "DeployJournal");
        }
    }
}
