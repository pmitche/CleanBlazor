﻿using BlazorHero.CleanArchitecture.Application.Features.Dashboards.Queries;
using BlazorHero.CleanArchitecture.Contracts.Dashboard;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorHero.CleanArchitecture.Server.Controllers.v1;

public class DashboardController : BaseApiController
{
    /// <summary>
    ///     Get Dashboard Data
    /// </summary>
    /// <returns>Status 200 OK </returns>
    [Authorize(Policy = Permissions.Dashboards.View)]
    [HttpGet]
    public async Task<IActionResult> GetDataAsync()
    {
        Result<DashboardDataResponse> result = await Sender.Send(new GetDashboardDataQuery());
        return Ok(result);
    }
}
