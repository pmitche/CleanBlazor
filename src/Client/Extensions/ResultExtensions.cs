using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Extensions;

public static class ResultExtensions
{
    public static void HandleWithSnackBar(
        this Result result,
        ISnackbar snackBar,
        Action<IReadOnlyList<string>> onSuccess)
    {
        if (result.IsSuccess)
        {
            onSuccess(result.Messages);
        }
        else
        {
            snackBar.Error(result.Messages);
        }
    }

    public static void HandleWithSnackBar<T>(
        this Result<T> result,
        ISnackbar snackBar,
        Action<IReadOnlyList<string>, T> onSuccess)
    {
        if (result.IsSuccess)
        {
            onSuccess(result.Messages, result.Data);
        }
        else
        {
            snackBar.Error(result.Messages);
        }
    }

    public static void HandleWithSnackBar<T>(
        this PaginatedResult<T> result,
        ISnackbar snackBar,
        Action<PaginatedResult<T>> onSuccess)
    {
        if (result.IsSuccess)
        {
            onSuccess(result);
        }
        else
        {
            snackBar.Error(result.Messages);
        }
    }

    public static async Task HandleWithSnackBarAsync(
        this Result result,
        ISnackbar snackBar,
        Func<IReadOnlyList<string>, Task> onSuccess)
    {
        if (result.IsSuccess)
        {
            await onSuccess(result.Messages);
        }
        else
        {
            snackBar.Error(result.Messages);
        }
    }

    public static async Task HandleWithSnackBarAsync<T>(
        this Result<T> result,
        ISnackbar snackBar,
        Func<IReadOnlyList<string>, T, Task> onSuccess)
    {
        if (result.IsSuccess)
        {
            await onSuccess(result.Messages, result.Data);
        }
        else
        {
            snackBar.Error(result.Messages);
        }
    }

    // public static async Task HandleLol(this Task<Result> resultTask, ISnackbar snackbar)
}
