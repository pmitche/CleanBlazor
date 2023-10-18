using BlazorHero.CleanArchitecture.Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

public class BlazorHeroRoleClaim : IdentityRoleClaim<string>, IAuditableEntity<int>
{
    public BlazorHeroRoleClaim()
    {
    }

    public BlazorHeroRoleClaim(string roleClaimDescription = null, string roleClaimGroup = null)
    {
        Description = roleClaimDescription;
        Group = roleClaimGroup;
    }

    public string Description { get; set; }
    public string Group { get; set; }
    public virtual BlazorHeroRole Role { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}
