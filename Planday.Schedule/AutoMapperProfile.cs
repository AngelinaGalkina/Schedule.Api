using AutoMapper;

namespace Planday.Schedule
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Shift, GetShiftDto>();
            CreateMap<GetShiftDto, Shift>();
            CreateMap<AddShiftDto, GetShiftDto>();
            CreateMap<GetShiftDto, AddShiftDto>();
            CreateMap<AddShiftDto, CreateShiftDto>();
            CreateMap<CreateShiftDto, AddShiftDto>();
        }
    }
}
