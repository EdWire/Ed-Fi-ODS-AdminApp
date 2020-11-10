// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
#if NET48
using EdFi.Ods.Common;
using EdFi.Ods.Common.Utils.Extensions;
#else
using EdFi.Common;
using EdFi.Common.Utils.Extensions;
#endif

namespace EdFi.Ods.AdminApp.Management.Database
{
    public static class EntityFramework6DatabaseModelBuilderExtensions
    {
        public static DbModelBuilder ApplyDatabaseServerSpecificConventions(this DbModelBuilder modelBuilder)
        {
            if (!DatabaseProviderHelper.PgSqlProvider) return modelBuilder;

            modelBuilder.Conventions.Add<ForeignKeyLowerCaseNamingConvention>();
            modelBuilder.Conventions.Add<TableLowerCaseNamingConvention>();
            modelBuilder.Properties().Configure(c => c.HasColumnName(c.ClrPropertyInfo.Name.ToLowerInvariant()));
            return modelBuilder;
        }

        private class TableLowerCaseNamingConvention : IStoreModelConvention<EntitySet>
        {
            public void Apply(EntitySet entitySet, DbModel model)
            {
                Preconditions.ThrowIfNull(entitySet, nameof(entitySet));
                Preconditions.ThrowIfNull(model, nameof(model));

                entitySet.Table = entitySet.Table.ToLowerInvariant();
            }
        }

        private class ForeignKeyLowerCaseNamingConvention : IStoreModelConvention<AssociationType>
        {
            public void Apply(AssociationType association, DbModel model)
            {
                Preconditions.ThrowIfNull(association, nameof(association));
                Preconditions.ThrowIfNull(model, nameof(model));

                if (!association.IsForeignKey)
                {
                    return;
                }

                association.Constraint.FromProperties.ForEach(PropertyNamesToLowerInvariant);
                association.Constraint.ToProperties.ForEach(PropertyNamesToLowerInvariant);

                void PropertyNamesToLowerInvariant(EdmProperty property) =>
                    property.Name = property.Name.ToLowerInvariant();
            }
        }
    }
}
