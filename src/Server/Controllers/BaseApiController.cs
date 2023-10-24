using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlazorHero.CleanArchitecture.Server.Controllers;

/// <summary>
///     Abstract BaseApi Controller Class
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender _mediatorInstance;
    protected ISender Mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<ISender>();
}
