using BlazorHero.CleanArchitecture.Application.Serialization;
using BlazorHero.CleanArchitecture.Domain.Contracts;
using BlazorHero.CleanArchitecture.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorHero.CleanArchitecture.Infrastructure.Configuration;

public class EntityExtendedAttributeConfiguration : IEntityTypeConfiguration<IEntityExtendedAttribute>
{
    public void Configure(EntityTypeBuilder<IEntityExtendedAttribute> builder) =>
        // This Converter will perform the conversion to and from Json to the desired type
        builder
            .Property(e => e.Json)
            .HasJsonConversion(new SystemTextJsonSerializer());
}
