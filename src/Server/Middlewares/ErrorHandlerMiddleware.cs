using System.Net;
using System.Text.Json;
using CleanBlazor.Application.Exceptions;
using CleanBlazor.Shared.Wrapper;

namespace CleanBlazor.Server.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            HttpResponse response = context.Response;
            response.ContentType = "application/json";
            var responseModel = Result.Fail(error.Message);

            response.StatusCode = error switch
            {
                ApiException =>
                    // custom application error
                    (int)HttpStatusCode.BadRequest,
                KeyNotFoundException =>
                    // not found error
                    (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var result = JsonSerializer.Serialize(responseModel);
            await response.WriteAsync(result);
        }
    }
}
