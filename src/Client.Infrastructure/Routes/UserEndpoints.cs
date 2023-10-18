namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;

public static class UserEndpoints
{
    public const string GetAll = "api/identity/user";

    public const string Export = "api/identity/user/export";
    public const string Register = "api/identity/user";
    public const string ToggleUserStatus = "api/identity/user/toggle-status";
    public const string ForgotPassword = "api/identity/user/forgot-password";
    public const string ResetPassword = "api/identity/user/reset-password";

    public static string Get(string userId) => $"api/identity/user/{userId}";

    public static string GetUserRoles(string userId) => $"api/identity/user/roles/{userId}";

    public static string ExportFiltered(string searchString) => $"{Export}?searchString={searchString}";
}
