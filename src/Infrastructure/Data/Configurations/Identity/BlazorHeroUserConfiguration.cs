using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorHero.CleanArchitecture.Infrastructure.Data.Configurations.Identity;

public class BlazorHeroUserConfiguration : IEntityTypeConfiguration<BlazorHeroUser>
{
    public void Configure(EntityTypeBuilder<BlazorHeroUser> builder)
    {
        builder.ToTable("Users", "Identity");

        builder.Property(e => e.Id).ValueGeneratedOnAdd();
    }
}
