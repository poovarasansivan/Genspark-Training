using FitnessTracking.Models.DTOs;

namespace FitnessTracking.Misc
{
    public static class ResponseHandler
    {
        public static ApiSuccessResponseDto<T> Success<T>(T data, string message = "Success")
        {
            return new ApiSuccessResponseDto<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Erors = null
            };
        }

        public static ApiSuccessResponseDto<T> Success<T>(string message = "Success")
        {
            return new ApiSuccessResponseDto<T>
            {
                Success = true,
                Message = message,
                Data = default!,
                Erors = null
            };
        }

        public static ApiSuccessResponseDto<T> Error<T>(string message, object? errors = null)
        {
            return new ApiSuccessResponseDto<T>
            {
                Success = false,
                Message = message,
                Data = default!,
                Erors = errors
            };
        }
    }
}