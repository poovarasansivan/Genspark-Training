namespace FitnessTracking.Misc
{
    public class CustomeExceptionHandler : Exception
    {
        public int StatusCode { get; }
        public Dictionary<string, string[]>? Errors { get; }

        public CustomeExceptionHandler(string message, int statusCode = 404, Dictionary<string, string[]>? errors = null) : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}