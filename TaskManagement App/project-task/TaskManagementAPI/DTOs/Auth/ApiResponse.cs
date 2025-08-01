using TaskManagementAPI.DTOs.TaskItems;

namespace TaskManagementAPI.ApiResponses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }

        public ApiResponse() { }

        public ApiResponse(T data, string message = "Success")
        {
            Success = true;
            Message = message;
            Data = data;
            Errors = null;
        }

        public ApiResponse(string message, Dictionary<string, List<string>> errors)
        {
            Success = false;
            Message = message;
            Data = default;
            Errors = errors;
        }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>(data, message);
        }

        public static ApiResponse<T> ErrorResponse(string message, Dictionary<string, List<string>> errors)
        {
            return new ApiResponse<T>(message, errors);
        }

        internal static object? ErrorResponse(string v)
        {
            throw new NotImplementedException();
        }
    }
}
