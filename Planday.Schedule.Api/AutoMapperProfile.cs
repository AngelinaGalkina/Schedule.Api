using AutoMapper;
using Planday.Schedule.Infrastructure.Dto;
using Planday.Schedule.Models;

namespace Planday.Schedule
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Shift, ShiftDto>();
            CreateMap<ShiftDto, Shift>();
        }
    }
}
