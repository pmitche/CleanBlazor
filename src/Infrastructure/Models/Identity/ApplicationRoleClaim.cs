using BlazorHero.CleanArchitecture.Domain.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

public class ApplicationRoleClaim : IdentityRoleClaim<string>, IAuditableEntity
{
    public ApplicationRoleClaim()
    {
    }

    public ApplicationRoleClaim(string roleClaimDescription = null, string roleClaimGroup = null)
    {
        Description = roleClaimDescription;
        Group = roleClaimGroup;
    }

    public string Description { get; set; }
    public string Group { get; set; }
    public virtual ApplicationRole Role { get; set; }
    public string CreatedBy { get; }
    public DateTime CreatedOn { get; }
    public string LastModifiedBy { get; }
    public DateTime? LastModifiedOn { get; }
}
