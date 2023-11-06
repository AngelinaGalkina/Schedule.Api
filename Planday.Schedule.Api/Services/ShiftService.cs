using AutoMapper;
using Planday.Schedule.Api.Dto;
using Planday.Schedule.Api.Models;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Infrastructure.Queries;

namespace Planday.Schedule.Api.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IMapper Mapper;

        private readonly IConnectionStringProvider _connectionStringProvider;

        public ShiftService(IMapper mapper, IConnectionStringProvider connectionStringProvider)
        {
            Mapper = mapper;
            _connectionStringProvider = connectionStringProvider;
        }


        public async Task<ServiceResponse<GetShiftDto>> GetShiftById(int id)
        {
            var selectShiftsQuery = new SelectShiftsQuery(_connectionStringProvider);
            var serviceResponse = new ServiceResponse<GetShiftDto>();
            var shift = selectShiftsQuery.GetShiftById(id);

            serviceResponse.Data = Mapper.Map<GetShiftDto>(shift);

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetShiftDto>> AddShift(GetShiftDto newShift)
        {
            var updateShiftsQuery = new UpdateShiftsQuery(_connectionStringProvider);
            var selectShiftsQuery = new SelectShiftsQuery(_connectionStringProvider);
            var serviceResponse = new ServiceResponse<GetShiftDto>();
            var result = newShift.Start.CompareTo(newShift.End);

            if (result > 0)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Start time is grater than end time";

                return serviceResponse;
            }

            var shift = Mapper.Map<Shift>(newShift);

            try
            {
                var ok = updateShiftsQuery.AddShift(shift);

                if (!ok.IsCompletedSuccessfully)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Couldn't add new shift, something went wrong";

                    return serviceResponse;
                }

                var addedShift = selectShiftsQuery.GetShiftById(shift.Id);
                var shiftDto = Mapper.Map<GetShiftDto>(addedShift);

                serviceResponse.Data = shiftDto;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}