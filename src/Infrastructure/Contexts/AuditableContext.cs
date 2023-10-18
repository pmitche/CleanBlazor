using BlazorHero.CleanArchitecture.Application.Enums;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Audit;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BlazorHero.CleanArchitecture.Infrastructure.Contexts;

public abstract class AuditableContext : IdentityDbContext<BlazorHeroUser, BlazorHeroRole, string,
    IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, BlazorHeroRoleClaim,
    IdentityUserToken<string>>
{
    protected AuditableContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Audit> AuditTrails { get; set; }

    public virtual async Task<int> SaveChangesAsync(string userId = null, CancellationToken cancellationToken = new())
    {
        List<AuditEntry> auditEntries = OnBeforeSaveChanges(userId);
        var result = await base.SaveChangesAsync(cancellationToken);
        await OnAfterSaveChanges(auditEntries, cancellationToken);
        return result;
    }

    private List<AuditEntry> OnBeforeSaveChanges(string userId)
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();
        foreach (EntityEntry entry in ChangeTracker.Entries())
        {
            if (ShouldSkipEntry(entry))
            {
                continue;
            }

            var auditEntry = new AuditEntry(entry) { TableName = entry.Entity.GetType().Name, UserId = userId };
            auditEntries.Add(auditEntry);
            ProcessProperties(entry, auditEntry);
        }

        foreach (AuditEntry auditEntry in auditEntries.Where(e => !e.HasTemporaryProperties))
        {
            AuditTrails.Add(auditEntry.ToAudit());
        }

        return auditEntries.Where(e => e.HasTemporaryProperties).ToList();
    }

    private static bool ShouldSkipEntry(EntityEntry entry) => entry.Entity is Audit ||
                                                              entry.State == EntityState.Detached ||
                                                              entry.State == EntityState.Unchanged;

    private static void ProcessProperties(EntityEntry entry, AuditEntry auditEntry)
    {
        foreach (PropertyEntry property in entry.Properties)
        {
            var propertyName = property.Metadata.Name;
            if (property.IsTemporary)
            {
                auditEntry.TemporaryProperties.Add(property);
                continue;
            }

            if (property.Metadata.IsPrimaryKey())
            {
                auditEntry.KeyValues[propertyName] = property.CurrentValue;
                continue;
            }

            ProcessEntryState(entry.State, property, auditEntry, propertyName);
        }
    }

    private static void ProcessEntryState(EntityState state, PropertyEntry property, AuditEntry auditEntry, string propertyName)
    {
        switch (state)
        {
            case EntityState.Added:
                auditEntry.AuditType = AuditType.Create;
                auditEntry.NewValues[propertyName] = property.CurrentValue;
                break;

            case EntityState.Deleted:
                auditEntry.AuditType = AuditType.Delete;
                auditEntry.OldValues[propertyName] = property.OriginalValue;
                break;

            case EntityState.Modified:
                if (property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false)
                {
                    auditEntry.ChangedColumns.Add(propertyName);
                    auditEntry.AuditType = AuditType.Update;
                    auditEntry.OldValues[propertyName] = property.OriginalValue;
                    auditEntry.NewValues[propertyName] = property.CurrentValue;
                }

                break;
        }
    }

    private Task OnAfterSaveChanges(List<AuditEntry> auditEntries, CancellationToken cancellationToken = new())
    {
        if (auditEntries == null || auditEntries.Count == 0)
        {
            return Task.CompletedTask;
        }

        foreach (AuditEntry auditEntry in auditEntries)
        {
            foreach (PropertyEntry prop in auditEntry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                }
                else
                {
                    auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                }
            }

            AuditTrails.Add(auditEntry.ToAudit());
        }

        return SaveChangesAsync(cancellationToken);
    }
}
