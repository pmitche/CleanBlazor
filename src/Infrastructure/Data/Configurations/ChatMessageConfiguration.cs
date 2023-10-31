using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorHero.CleanArchitecture.Infrastructure.Data.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage<BlazorHeroUser>>
{
    public void Configure(EntityTypeBuilder<ChatMessage<BlazorHeroUser>> builder)
    {
        builder.ToTable("ChatMessages");

        builder.Property(p => p.Message)
            .HasMaxLength(8000);

        builder.HasOne(d => d.FromUser)
            .WithMany(p => p.ChatMessagesFromUsers)
            .HasForeignKey(d => d.FromUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(d => d.ToUser)
            .WithMany(p => p.ChatMessagesToUsers)
            .HasForeignKey(d => d.ToUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasQueryFilter(p => !p.FromUser.Deleted && !p.ToUser.Deleted);
    }
}
