﻿using Humanizer;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Newtonsoft.Json;

using R8.Lib.Localization;
using R8.Lib.MethodReturn;

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace R8.EntityFrameworkCore
{
    public static class EntityBaseExtensions
    {
        /// <summary>
        /// Saves changes that occurred in <see cref="TModel"/> to Database
        /// </summary>
        /// <typeparam name="TModel">A generic type of <see cref="IResponseBaseDatabase"/>.</typeparam>
        /// <typeparam name="TDbContext">A generic type of <see cref="DbContextBase"/>.</typeparam>
        /// <param name="model">A model that contains trackable data.</param>
        /// <param name="dbContext">An instance of DbContext</param>
        public static void SaveDatabase<TModel, TDbContext>(this TModel model, TDbContext dbContext)
            where TModel : IResponseBaseDatabase where TDbContext : DbContextBase
        {
            model.Save = dbContext.SaveChanges();
        }

        /// <summary>
        /// Saves changes that occurred in <see cref="TModel"/> to Database
        /// </summary>
        /// <typeparam name="TModel">A generic type of <see cref="IResponseBaseDatabase"/>.</typeparam>
        /// <typeparam name="TDbContext">A generic type of <see cref="DbContextBase"/>.</typeparam>
        /// <param name="model">A model that contains trackable data.</param>
        /// <param name="dbContext">An instance of DbContext</param>
        /// <returns>A <see cref="Task"/> value that representing asynchronous operation.</returns>
        public static async Task SaveDatabaseAsync<TModel, TDbContext>(this TModel model, TDbContext dbContext, CancellationToken cancellationToken = default)
            where TModel : IResponseBaseDatabase where TDbContext : DbContextBase
        {
            model.Save = await dbContext.SaveChangesAsync(cancellationToken);
        }

        internal static string GetTableName(this EntityEntry entry)
        {
            return entry.Context.GetTableName(entry.Entity.GetType());
        }

        internal static string GetTableName<TDbContext>(this TDbContext context, Type entity) where TDbContext : DbContext
        {
            var entityType = context.Model.FindEntityType(entity);
            return entityType.GetTableName();
        }

        internal static PropertyBuilder<CultureInfo> HasCultureConversion(this PropertyBuilder<CultureInfo> property)
        {
            return property.HasConversion(
                    x => x.Name,
                    v => !string.IsNullOrEmpty(v) ? CultureInfo.GetCultureInfo(v) : null);
        }

        internal static EntityTypeBuilder<TEntity> ApplyConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IEntityBase
        {
            builder.ToTable(typeof(TEntity).Name.Pluralize());
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.RowVersion).IsRowVersion();
            builder.HasQueryFilter(entity => !entity.IsDeleted);
            return builder;
        }

        public static void ConfigureAuditCollection<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IEntityBase
        {
            var tableName = builder.Metadata.AsEntityType().GetTableName();
            var pluralizeAudit = nameof(Audit).Pluralize();
            builder.OwnsMany(x => x.Audits, action =>
            {
                action.WithOwner().HasForeignKey(x => x.RowId);
                action.IsMemoryOptimized();
                action.ToTable($"{tableName}_{pluralizeAudit}");
                action.HasKey(x => x.Id);
                action.Property(x => x.Id).ValueGeneratedOnAdd();
                action.Property(x => x.Culture).HasCultureConversion();
                action.Property(x => x.NewValues).HasJsonConversion();
                action.Property(x => x.OldValues).HasJsonConversion();
            });
        }

        public const string LocalizedNameColumn = "Name";

        internal static PropertyBuilder<LocalizerContainer> HasLocalizerContainerConversion(this PropertyBuilder<LocalizerContainer> property)
        {
            property.HasConversion(
                v => v.Serialize(),
                v => LocalizerContainer.Deserialize(v))
                .Metadata.SetValueComparer(
                new ValueComparer<LocalizerContainer>(
                    (l, r) => l.Serialize() == r.Serialize(),
                    v => v == null ? 0 : v.Serialize().GetHashCode(),
                    v => LocalizerContainer.Deserialize(v.Serialize())));

            return property;
        }

        internal static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> property)
        {
            property.HasConversion(
                x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<T>(x))
                .Metadata.SetValueComparer(new ValueComparer<T>(
                    (l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r),
                    v => v == null ? 0 : JsonConvert.SerializeObject(v).GetHashCode(),
                    v => JsonConvert.DeserializeObject<T>(
                        JsonConvert.SerializeObject(v))));
            return property;
        }

        public static void HasDateTimeUtcConversion(this ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                foreach (var property in entityType.GetProperties())
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                        property.SetValueConverter(converter);
        }
    }
}