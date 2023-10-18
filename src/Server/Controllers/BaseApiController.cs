using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlazorHero.CleanArchitecture.Server.Controllers;

/// <summary>
///     Abstract BaseApi Controller Class
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController<T> : ControllerBase
{
    private ILogger<T> _loggerInstance;
    private IMediator _mediatorInstance;
    protected IMediator Mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();
    protected ILogger<T> Logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
}
