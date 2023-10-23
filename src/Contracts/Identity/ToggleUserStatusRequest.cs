namespace BlazorHero.CleanArchitecture.Contracts.Identity;

public class ToggleUserStatusRequest
{
    public bool ActivateUser { get; set; }
    public string UserId { get; set; }
}
