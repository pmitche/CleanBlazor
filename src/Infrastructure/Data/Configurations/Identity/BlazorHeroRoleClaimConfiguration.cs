using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorHero.CleanArchitecture.Infrastructure.Data.Configurations.Identity;

public class BlazorHeroRoleClaimConfiguration : IEntityTypeConfiguration<BlazorHeroRoleClaim>
{
    public void Configure(EntityTypeBuilder<BlazorHeroRoleClaim> builder)
    {
        builder.ToTable("RoleClaims", "Identity");

        builder.HasOne(d => d.Role)
            .WithMany(p => p.RoleClaims)
            .HasForeignKey(d => d.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
