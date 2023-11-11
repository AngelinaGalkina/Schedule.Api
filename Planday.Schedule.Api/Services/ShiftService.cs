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
            var shift = await selectShiftsQuery.GetShiftById(id);

            serviceResponse.Data = Mapper.Map<GetShiftDto>(shift);

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetShiftDto>> AddShift(CreateShiftDto newShift)
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

            var shift = Mapper.Map<AddShiftDto>(newShift);

            try
            {
                var newShiftId = await updateShiftsQuery.AddShift(shift);

                if (newShiftId == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Couldn't add new shift, something went wrong";

                    return serviceResponse;
                }

                var addedShift = await selectShiftsQuery.GetShiftById(newShiftId);
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

        public async Task<ServiceResponse<GetShiftDto>> AssignShiftToEmployee(int employeeId, int shiftId)
        {
            var selectShiftsQuery = new SelectShiftsQuery(_connectionStringProvider);
            var serviceResponse = new ServiceResponse<GetShiftDto>();
            ////var shift = await selectShiftsQuery.GetShiftById(id);

            ////serviceResponse.Data = Mapper.Map<GetShiftDto>(shift);

            return serviceResponse;
        }
        
    }
}