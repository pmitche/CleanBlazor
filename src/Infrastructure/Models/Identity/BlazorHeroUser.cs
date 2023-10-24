using System.ComponentModel.DataAnnotations.Schema;
using BlazorHero.CleanArchitecture.Domain.Contracts;
using BlazorHero.CleanArchitecture.Domain.Contracts.Chat;
using BlazorHero.CleanArchitecture.Shared.Models.Chat;
using Microsoft.AspNetCore.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

public class BlazorHeroUser : IdentityUser<string>, IChatUser, IAuditableEntity<string>
{
    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }
    public bool IsActive { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<ChatHistory<BlazorHeroUser>> ChatHistoryFromUsers { get; set; } =
        new HashSet<ChatHistory<BlazorHeroUser>>();

    public virtual ICollection<ChatHistory<BlazorHeroUser>> ChatHistoryToUsers { get; set; } =
        new HashSet<ChatHistory<BlazorHeroUser>>();
    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [Column(TypeName = "text")] public string ProfilePictureDataUrl { get; set; }
}
