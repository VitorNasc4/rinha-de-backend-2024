using api.DTOs;
using api.Models;
using AutoMapper;

namespace api.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateTranscaoDTO, Transacao>();
        }
    }
}