using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorHero.CleanArchitecture.Infrastructure.Data.Configurations.Identity;

public class BlazorHeroRoleConfiguration : IEntityTypeConfiguration<BlazorHeroRole>
{
    public void Configure(EntityTypeBuilder<BlazorHeroRole> builder)
    {
        builder.ToTable("Roles", "Identity");
    }
}
