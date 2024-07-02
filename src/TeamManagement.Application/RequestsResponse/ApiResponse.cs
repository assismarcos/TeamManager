using FluentValidation.Results;

namespace TeamManagement.Application.RequestsResponse;

public class ApiResponse<T>
{
    public T? Response { get; set; }
    public List<string> Errors { get; set; } = new();

    public ApiResponse()
    {
    }

    private ApiResponse(T? response)
    {
        Response = response;
    }

    private ApiResponse(IEnumerable<ValidationFailure> errors)
    {
        errors.ToList().ForEach(x => Errors.Add(x.ErrorMessage));
        Response = default;
    }

    private ApiResponse(IEnumerable<string> errors)
    {
        errors.ToList().ForEach(x => Errors.Add(x));
        Response = default;
    }

    private ApiResponse(string errorMessage)
    {
        Errors.Add(errorMessage);
        Response = default;
    }

    public static ApiResponse<T> CreateResponse(T? response) => new(response);
    public static ApiResponse<T> CreateResponse(IEnumerable<ValidationFailure> errors) => new(errors);
    public static ApiResponse<T> CreateResponse(IEnumerable<string> errors) => new(errors);
    public static ApiResponse<T> CreateResponse(string errorMessage) => new(errorMessage);
    public static ApiResponse<T> Empty() => new(default(T));
}