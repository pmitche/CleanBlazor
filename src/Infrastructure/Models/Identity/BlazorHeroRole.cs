﻿using BlazorHero.CleanArchitecture.Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

public class BlazorHeroRole : IdentityRole, IAuditableEntity<string>
{
    public BlazorHeroRole() {}

    public BlazorHeroRole(string roleName, string roleDescription = null) : base(roleName)
    {
        Description = roleDescription;
    }

    public string Description { get; set; }
    public virtual ICollection<BlazorHeroRoleClaim> RoleClaims { get; set; } = new HashSet<BlazorHeroRoleClaim>();
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}
