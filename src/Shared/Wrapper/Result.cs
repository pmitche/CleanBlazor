namespace BlazorHero.CleanArchitecture.Shared.Wrapper;

public class Result : IResult
{
    public List<string> Messages { get; set; } = new();

    public bool Succeeded { get; set; }

    public static IResult Fail() => new Result { Succeeded = false };

    public static IResult Fail(string message) =>
        new Result { Succeeded = false, Messages = new List<string> { message } };

    public static IResult Fail(List<string> messages) => new Result { Succeeded = false, Messages = messages };

    public static Task<IResult> FailAsync() => Task.FromResult(Fail());

    public static Task<IResult> FailAsync(string message) => Task.FromResult(Fail(message));

    public static Task<IResult> FailAsync(List<string> messages) => Task.FromResult(Fail(messages));

    public static IResult Success() => new Result { Succeeded = true };

    public static IResult Success(string message) =>
        new Result { Succeeded = true, Messages = new List<string> { message } };

    public static Task<IResult> SuccessAsync() => Task.FromResult(Success());

    public static Task<IResult> SuccessAsync(string message) => Task.FromResult(Success(message));
}

public class Result<T> : Result, IResult<T>
{
    public T Data { get; set; }

    public static new Result<T> Fail() => new() { Succeeded = false };

    public static new Result<T> Fail(string message) =>
        new() { Succeeded = false, Messages = new List<string> { message } };

    public static new Result<T> Fail(List<string> messages) => new() { Succeeded = false, Messages = messages };

    public static new Task<Result<T>> FailAsync() => Task.FromResult(Fail());

    public static new Task<Result<T>> FailAsync(string message) => Task.FromResult(Fail(message));

    public static new Task<Result<T>> FailAsync(List<string> messages) => Task.FromResult(Fail(messages));

    public static new Result<T> Success() => new() { Succeeded = true };

    public static new Result<T> Success(string message) =>
        new() { Succeeded = true, Messages = new List<string> { message } };

    public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };

    public static Result<T> Success(T data, string message) =>
        new() { Succeeded = true, Data = data, Messages = new List<string> { message } };

    public static Result<T> Success(T data, List<string> messages) =>
        new() { Succeeded = true, Data = data, Messages = messages };

    public static new Task<Result<T>> SuccessAsync() => Task.FromResult(Success());

    public static new Task<Result<T>> SuccessAsync(string message) => Task.FromResult(Success(message));

    public static Task<Result<T>> SuccessAsync(T data) => Task.FromResult(Success(data));

    public static Task<Result<T>> SuccessAsync(T data, string message) => Task.FromResult(Success(data, message));
}
