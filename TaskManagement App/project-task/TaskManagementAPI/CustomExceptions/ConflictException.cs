using System;
using System.Net;

namespace TaskManagementAPI.CustomExceptions
{
    public class ConflictException : Exception
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.Conflict;

        public ConflictException(string message) : base(message)
        {
        }
    }
}
