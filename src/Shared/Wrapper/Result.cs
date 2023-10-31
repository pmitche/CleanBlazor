using System.Text.Json.Serialization;

namespace BlazorHero.CleanArchitecture.Shared.Wrapper;

public class Result
{
    /// <summary>
    /// Parameterless constructor is required for deserialization.
    /// </summary>
    public Result() {}
    protected Result(bool isSuccess, IEnumerable<string> errorMessages)
    {
        var messages = (errorMessages ?? Array.Empty<string>()).ToList().AsReadOnly();

        IsSuccess = isSuccess;
        Messages = messages;
    }
    protected Result(bool isSuccess, string message) : this(isSuccess, new []{ message }) {}
    protected Result(bool isSuccess) : this(isSuccess, Array.Empty<string>()) {}

    [JsonInclude]
    public bool IsSuccess { get; private set; }

    public bool IsFailure => !IsSuccess;

    [JsonInclude]
    public IReadOnlyList<string> Messages { get; private set; }

    public static Result Ok() => new(true);
    public static Result<T> Ok<T>(T data) => new(data, true);
    public static Result Ok(string message) => new(true, message);
    public static Result<T> Ok<T>(T data, string message) => new(data, true, message);

    public static Result Fail() => new(false);
    public static Result<T> Fail<T>() => new (default, false);
    public static Result Fail(string message) => new(false, message);
    public static Result<T> Fail<T>(string message) => new(default, false, message);
    public static Result Fail(IEnumerable<string> messages) => new(false, messages);
    public static Result<T> Fail<T>(IEnumerable<string> messages) => new(default, false, messages);
}

public class Result<T> : Result
{
    /// <summary>
    /// Parameterless constructor is required for deserialization.
    /// </summary>
    public Result() {}
    protected internal Result(T data, bool isSuccess, IEnumerable<string> errorMessages) : base(isSuccess, errorMessages)
    {
        Data = data;
    }
    protected internal Result(T data, bool isSuccess, string message) : this(data, isSuccess, new []{ message }) {}
    protected internal Result(T data, bool isSuccess) : this(data, isSuccess, Array.Empty<string>()) {}

    public static implicit operator Result<T>(T data) => Ok(data);

    [JsonInclude]
    public T Data { get; private set; }
}
