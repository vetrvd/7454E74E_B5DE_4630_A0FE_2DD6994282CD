using AutoMapper;
using _7454E74E_B5DE_4630_A0FE_2DD6994282CD.Model.Dto;

namespace _7454E74E_B5DE_4630_A0FE_2DD6994282CD.Model
{
    /// <summary>
    /// mapping profile to projection
    /// </summary>
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserDto, User>();
            CreateMap<NoteDto, Note>();
            CreateMap<Note, NoteDto>();
            CreateMap<Note, NoteDto>();
        }
    }
}