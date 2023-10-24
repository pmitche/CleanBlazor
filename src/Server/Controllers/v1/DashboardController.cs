using BlazorHero.CleanArchitecture.Application.Features.Dashboards.Queries;
using BlazorHero.CleanArchitecture.Application.Features.Dashboards.Queries.GetData;
using BlazorHero.CleanArchitecture.Contracts.Dashboard;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorHero.CleanArchitecture.Server.Controllers.v1;

[ApiController]
public class DashboardController : BaseApiController<DashboardController>
{
    /// <summary>
    ///     Get Dashboard Data
    /// </summary>
    /// <returns>Status 200 OK </returns>
    [Authorize(Policy = Permissions.Dashboards.View)]
    [HttpGet]
    public async Task<IActionResult> GetDataAsync()
    {
        Result<DashboardDataResponse> result = await Mediator.Send(new GetDashboardDataQuery());
        return Ok(result);
    }
}
