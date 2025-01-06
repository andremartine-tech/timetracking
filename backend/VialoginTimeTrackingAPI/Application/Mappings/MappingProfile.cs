using AutoMapper;
using Application.DTOs;
using Core.Entities;

namespace Application.Mappings
{
    /// <summary>
    /// Configura o mapeamento entre entidades e DTOs usando AutoMapper.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeamento entre User e UserDto
            CreateMap<User, UserDto>();

            // Mapeamento entre TimeLog e TimeLogDto
            CreateMap<TimeLog, TimeLogDto>()
                .ForMember(dest => dest.TimestampIn, opt => opt.MapFrom(src => src.TimestampIn))
                .ForMember(dest => dest.TimestampOut, opt => opt.MapFrom(src => src.TimestampOut))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            // (Opcional) Mapeamento reverso para cenários onde um DTO deve ser convertido de volta para a entidade.
            CreateMap<UserDto, User>();
            CreateMap<TimeLogDto, TimeLog>();
        }
    }
}
