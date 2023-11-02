using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Domain.Abstractions;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using Microsoft.AspNetCore.Identity;

namespace BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;

public class ApplicationUser : IdentityUser<string>, IUser, IChatUser, IAuditableEntity, ISoftDeletableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfilePictureDataUrl { get; set; }
    public bool IsActive { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<ChatMessage<ApplicationUser>> ChatMessagesFromUsers { get; set; } =
        new HashSet<ChatMessage<ApplicationUser>>();

    public virtual ICollection<ChatMessage<ApplicationUser>> ChatMessagesToUsers { get; set; } =
        new HashSet<ChatMessage<ApplicationUser>>();
    public string CreatedBy { get; }
    public DateTime CreatedOn { get; }
    public string LastModifiedBy { get; }
    public DateTime? LastModifiedOn { get; }
    public string DeletedBy { get; private set; }
    public DateTime? DeletedOn { get; private set; }
    public bool Deleted { get; private set; }

    /// <inheritdoc />
    public void Restore()
    {
        Deleted = false;
        DeletedBy = null;
        DeletedOn = null;
    }
}
