using Microsoft.AspNetCore.Authorization;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using System.Security.Claims;

namespace FirstAPI.Authorization
{
    public class ExperiencedDoctorHandler : AuthorizationHandler<ExperiencedDoctorRequirement>
    {
        private readonly IRepository<int, Doctor> _doctorRepository;

        public ExperiencedDoctorHandler(IRepository<int, Doctor> doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ExperiencedDoctorRequirement requirement)
        {
            var emailClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (emailClaim == null)
            {
                Console.WriteLine("Authorization failed: No email claim found.");
                return;
            }

            string doctorEmail = emailClaim.Value;

            var allDoctors = await _doctorRepository.GetAll();
            var doctor = allDoctors.FirstOrDefault(d => d.Email.Equals(doctorEmail, StringComparison.OrdinalIgnoreCase));
            
            if (doctor == null)
            {
                Console.WriteLine($"Authorization failed: No doctor found with email {doctorEmail}");
                return;
            }

            Console.WriteLine($"Doctor found: {doctor.Name}, Years of Experience: {doctor.YearsOfExperience}");

            if (doctor.YearsOfExperience >= requirement.MinimumYearsExperience)
            {
                Console.WriteLine($"Authorization succeeded: Doctor meets experience requirement of {requirement.MinimumYearsExperience} years");
                context.Succeed(requirement);
            }

            else
            {
                Console.WriteLine($"Authorization failed: Doctor has {doctor.YearsOfExperience} years, which is less than required {requirement.MinimumYearsExperience}");
            }
        }

    }
}
