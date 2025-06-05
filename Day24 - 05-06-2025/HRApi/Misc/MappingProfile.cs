using AutoMapper;
using HRApi.Models;
using HRApi.Models.DTOs.FileHandlingDtos;

namespace HRApi.Misc
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserAddRequestDto, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())  // Ignore mapping for Password
                .ForMember(dest => dest.HashKey, opt => opt.Ignore());  // Ignore HashKey as well
        }
    }

}