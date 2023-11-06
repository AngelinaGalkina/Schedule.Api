using AutoMapper;
using Planday.Schedule.Api.Dto;
using Planday.Schedule.Api.Models;

namespace Planday.Schedule.Api
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Shift, GetShiftDto>();
            CreateMap<GetShiftDto, Shift>();
            ////CreateMap<AddCharacterDto, Character>();
            ////CreateMap<UpdateCharacterDto, Character>();
        }
    }
}
