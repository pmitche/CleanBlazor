using System.ComponentModel.DataAnnotations.Schema;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Domain.Abstractions;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using Microsoft.AspNetCore.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

public class BlazorHeroUser : IdentityUser<string>, IUser, IChatUser, IAuditableEntity
{
    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }
    public bool IsActive { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<ChatMessage<BlazorHeroUser>> ChatMessagesFromUsers { get; set; } =
        new HashSet<ChatMessage<BlazorHeroUser>>();

    public virtual ICollection<ChatMessage<BlazorHeroUser>> ChatMessagesToUsers { get; set; } =
        new HashSet<ChatMessage<BlazorHeroUser>>();
    public string CreatedBy { get; }
    public DateTime CreatedOn { get; }
    public string LastModifiedBy { get; }
    public DateTime? LastModifiedOn { get; }
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [Column(TypeName = "text")] public string ProfilePictureDataUrl { get; set; }
}
