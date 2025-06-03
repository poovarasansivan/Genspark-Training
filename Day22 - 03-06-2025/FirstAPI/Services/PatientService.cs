using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FirstAPI.Interfaces;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace FirstAPI.Services
{
    public class PatientService : IPatientService
    {
        private readonly IMapper _mapper;
        private readonly IEncryptionService _encryptionService;
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<int, Patient> _patientRepository;

        public PatientService(IMapper mapper,
                              IRepository<string, User> userRepository,
                              IEncryptionService encryptionService,
                              IRepository<int, Patient> patientRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _patientRepository = patientRepository;
        }

        public async Task<Patient> GetPatientByName(string name)
        {
            return await Task.FromResult(new Patient { Name = name });
        }

        public async Task<Patient> AddPatient(PatientAddRequestDto patientDto)
        {
            try
            {
                var user = _mapper.Map<PatientAddRequestDto, User>(patientDto);
                
                var encryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = patientDto.Password
                });

                user.Password = encryptedData.EncryptedData;
                user.HashKey = encryptedData.HashKey;
                user.Role = "Patient";
                user = await _userRepository.Add(user);

                var newPatient = _mapper.Map<PatientAddRequestDto, Patient>(patientDto);
                newPatient.User = user;
                if (newPatient.User == null)
                    throw new Exception("User data is null");
                newPatient = await _patientRepository.Add(newPatient);

                if (newPatient == null)
                    throw new Exception("Could not add patient data");

                return newPatient;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while adding patient: {ex.Message}");
            }
        }
    }
}
