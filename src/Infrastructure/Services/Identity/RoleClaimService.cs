using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Data;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Infrastructure.Services.Identity;

public class RoleClaimService : IRoleClaimService
{
    private readonly BlazorHeroContext _db;
    private readonly IStringLocalizer<RoleClaimService> _localizer;
    private readonly IMapper _mapper;

    public RoleClaimService(
        IStringLocalizer<RoleClaimService> localizer,
        IMapper mapper,
        BlazorHeroContext db)
    {
        _localizer = localizer;
        _mapper = mapper;
        _db = db;
    }

    public async Task<Result<List<RoleClaimResponse>>> GetAllAsync()
    {
        List<BlazorHeroRoleClaim> roleClaims = await _db.RoleClaims.ToListAsync();
        var roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);
        return roleClaimsResponse;
    }

    public async Task<int> GetCountAsync()
    {
        var count = await _db.RoleClaims.CountAsync();
        return count;
    }

    public async Task<Result<RoleClaimResponse>> GetByIdAsync(int id)
    {
        BlazorHeroRoleClaim roleClaim = await _db.RoleClaims
            .SingleOrDefaultAsync(x => x.Id == id);
        var roleClaimResponse = _mapper.Map<RoleClaimResponse>(roleClaim);
        return roleClaimResponse;
    }

    public async Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId)
    {
        List<BlazorHeroRoleClaim> roleClaims = await _db.RoleClaims
            .Include(x => x.Role)
            .Where(x => x.RoleId == roleId)
            .ToListAsync();
        var roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);
        return roleClaimsResponse;
    }

    public async Task<Result<string>> SaveAsync(RoleClaimRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RoleId))
        {
            return Result.Fail<string>(_localizer["Role is required."]);
        }

        if (request.Id == 0)
        {
            BlazorHeroRoleClaim existingRoleClaim =
                await _db.RoleClaims
                    .SingleOrDefaultAsync(x =>
                        x.RoleId == request.RoleId && x.ClaimType == request.Type && x.ClaimValue == request.Value);
            if (existingRoleClaim != null)
            {
                return Result.Fail<string>(_localizer["Similar Role Claim already exists."]);
            }

            var roleClaim = _mapper.Map<BlazorHeroRoleClaim>(request);
            await _db.RoleClaims.AddAsync(roleClaim);
            await _db.SaveChangesAsync();
            return Result.Ok<string>(string.Format(_localizer["Role Claim {0} created."], request.Value));
        }
        else
        {
            BlazorHeroRoleClaim existingRoleClaim =
                await _db.RoleClaims
                    .Include(x => x.Role)
                    .SingleOrDefaultAsync(x => x.Id == request.Id);
            if (existingRoleClaim == null)
            {
                return Result.Fail<string>(_localizer["Role Claim does not exist."]);
            }

            existingRoleClaim.ClaimType = request.Type;
            existingRoleClaim.ClaimValue = request.Value;
            existingRoleClaim.Group = request.Group;
            existingRoleClaim.Description = request.Description;
            existingRoleClaim.RoleId = request.RoleId;
            _db.RoleClaims.Update(existingRoleClaim);
            await _db.SaveChangesAsync();
            return Result.Ok<string>(string.Format(_localizer["Role Claim {0} for Role {1} updated."],
                request.Value,
                existingRoleClaim.Role.Name));
        }
    }

    public async Task<Result<string>> DeleteAsync(int id)
    {
        BlazorHeroRoleClaim existingRoleClaim = await _db.RoleClaims
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (existingRoleClaim == null)
        {
            return Result.Fail<string>(_localizer["Role Claim does not exist."]);
        }

        _db.RoleClaims.Remove(existingRoleClaim);
        await _db.SaveChangesAsync();
        return Result.Ok<string>(string.Format(_localizer["Role Claim {0} for {1} Role deleted."],
            existingRoleClaim.ClaimValue,
            existingRoleClaim.Role.Name));
    }
}
