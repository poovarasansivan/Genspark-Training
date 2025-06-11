using System.ComponentModel.DataAnnotations;

namespace FitnessTracking.Helpers
{
    public class EmailValidator : ValidationAttribute
    {
        private readonly string[]? _allowedDomains;
        public EmailValidator(string[] allowedDomains)
        {
            _allowedDomains = allowedDomains;
            ErrorMessage = "Email Domain that you have entered is not allowed to use";
        }

        public override bool IsValid(object? value)
        {
            if (value is string email)
            {
                var parts = email.Split('@');
                if (parts.Length == 2)
                {
                    if (_allowedDomains == null)
                    {
                        throw new Exception("Allowed domains are empty");
                    }
                    return _allowedDomains.Contains(parts[1].ToLower());
                }
            }
            return false;
        }
    }
}