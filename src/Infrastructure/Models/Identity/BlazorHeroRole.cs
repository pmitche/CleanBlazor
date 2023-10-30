using BlazorHero.CleanArchitecture.Domain.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

public class BlazorHeroRole : IdentityRole, IAuditableEntity
{
    public BlazorHeroRole() {}

    public BlazorHeroRole(string roleName, string roleDescription = null) : base(roleName)
    {
        Description = roleDescription;
    }

    public string Description { get; set; }
    public virtual ICollection<BlazorHeroRoleClaim> RoleClaims { get; set; } = new HashSet<BlazorHeroRoleClaim>();
    public string CreatedBy { get; }
    public DateTime CreatedOn { get; }
    public string LastModifiedBy { get; }
    public DateTime? LastModifiedOn { get; }
}
