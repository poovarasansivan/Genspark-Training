using WebApi.Models;
using WebApi.Interfaces;
using WebApi.Models.DTOs.DoctorSpecialities;
using Microsoft.EntityFrameworkCore;
using WebApi.Contexts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace WebApi.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly ClinicContext _context;

        public DoctorService(ClinicContext context)
        {
            _context = context;
        }

        public async Task<Doctor> GetDoctByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Doctor name cannot be empty.");

            return await _context.Doctors.FirstOrDefaultAsync(d => d.Name == name);
        }

        public async Task<ICollection<Doctor>> GetDoctorsBySpeciality(string speciality)
        {
            if (string.IsNullOrWhiteSpace(speciality))
                throw new ArgumentException("Speciality cannot be empty.");

            return await _context.Doctors
                .Where(d => d.DoctorSpecialities.Any(ds => ds.Speciality.Name == speciality))
                .ToListAsync();
        }

        public async Task<Doctor> AddDoctor(DoctorAddRequestDto doctorDto)
        {
            if (doctorDto == null)
                throw new ArgumentNullException(nameof(doctorDto));

            if (string.IsNullOrWhiteSpace(doctorDto.Name))
                throw new ArgumentException("Doctor name cannot be empty.");

            var existingDoctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Name == doctorDto.Name);

            if (existingDoctor != null)
                throw new InvalidOperationException($"Doctor with name '{doctorDto.Name}' already exists.");

            var doctor = new Doctor
            {
                Name = doctorDto.Name,
                YearsOfExperience = doctorDto.YearsOfExperience,
                Status = "Active"
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return doctor;
        }
    }
}
