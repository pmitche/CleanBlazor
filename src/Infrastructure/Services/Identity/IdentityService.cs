using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CleanBlazor.Application.Abstractions.Infrastructure.Services.Identity;
using CleanBlazor.Application.Configuration;
using CleanBlazor.Contracts.Identity;
using CleanBlazor.Infrastructure.Models.Identity;
using CleanBlazor.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CleanBlazor.Infrastructure.Services.Identity;

public class IdentityService : ITokenService
{
    private readonly AppConfiguration _appConfig;
    private readonly IStringLocalizer<IdentityService> _localizer;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<AppConfiguration> appConfig,
        SignInManager<ApplicationUser> signInManager,
        IStringLocalizer<IdentityService> localizer)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _appConfig = appConfig.Value;
        _signInManager = signInManager;
        _localizer = localizer;
    }

    public async Task<Result<TokenResponse>> LoginAsync(TokenRequest model)
    {
        ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Result.Fail<TokenResponse>(_localizer["User Not Found."]);
        }

        if (!user.IsActive)
        {
            return Result.Fail<TokenResponse>(_localizer["User Not Active. Please contact the administrator."]);
        }

        if (!user.EmailConfirmed)
        {
            return Result.Fail<TokenResponse>(_localizer["E-Mail not confirmed."]);
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordValid)
        {
            return Result.Fail<TokenResponse>(_localizer["Invalid Credentials."]);
        }

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        await _userManager.UpdateAsync(user);

        var token = await GenerateJwtAsync(user);
        var response = new TokenResponse
        {
            Token = token, RefreshToken = user.RefreshToken, UserImageUrl = user.ProfilePictureDataUrl
        };
        return response;
    }

    public async Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model)
    {
        if (model is null)
        {
            return Result.Fail<TokenResponse>(_localizer["Invalid Client Token."]);
        }

        ClaimsPrincipal userPrincipal = GetPrincipalFromExpiredToken(model.Token);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null)
        {
            return Result.Fail<TokenResponse>(_localizer["Email Not Found"]);
        }

        ApplicationUser user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return Result.Fail<TokenResponse>(_localizer["User Not Found."]);
        }

        if (user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return Result.Fail<TokenResponse>(_localizer["Invalid Client Token."]);
        }

        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        user.RefreshToken = GenerateRefreshToken();
        await _userManager.UpdateAsync(user);

        var response = new TokenResponse
        {
            Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
        };
        return response;
    }

    private async Task<string> GenerateJwtAsync(ApplicationUser user)
    {
        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        return token;
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
        IList<string> roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
            ApplicationRole thisRole = await _roleManager.FindByNameAsync(role);
            IList<Claim> allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
            permissionClaims.AddRange(allPermissionsForThisRoles);
        }

        IEnumerable<Claim> claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

        return claims;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(2),
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        ClaimsPrincipal principal =
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException(_localizer["Invalid token"]);
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_appConfig.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}
