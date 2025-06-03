using Microsoft.AspNetCore.Authorization;

namespace FirstAPI.Authorization
{
    public class ExperiencedDoctorRequirement : IAuthorizationRequirement
    {
        public float MinimumYearsExperience { get; }

        public ExperiencedDoctorRequirement(float years)
        {
            MinimumYearsExperience = years;
        }
    }
}
