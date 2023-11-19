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

        public async Task<ServiceResponse<GetShiftDto>> AssignShiftToEmployee(long employeeId, long shiftId)
        {
            var selectShiftsQuery = new SelectShiftsQuery(_connectionStringProvider);
            var selectEmployeeQuery = new SelectEmployeeQuery(_connectionStringProvider);
            var serviceResponse = new ServiceResponse<GetShiftDto>();
            var shift = await selectShiftsQuery.GetShiftById(shiftId);

            if (shift == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "This shift doesn't exists";

                return serviceResponse;
            }

            var employee = await selectEmployeeQuery.GetEmployeeById(employeeId);

            if (employee == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "This employee doesn't exists";

                return serviceResponse;
            }

            // Validation for the condition:  an employee cannot be assigned
            // to a shift in a time where that employee is already working
            var newShiftStart = shift.Start;
            var newShiftEnd = shift.End;
            var newShiftEmployeeId = shift.EmployeeId;
            var oveplappingShift = await selectShiftsQuery.OverlappingShifts(newShiftEmployeeId, newShiftStart, newShiftEnd);

            if (oveplappingShift.Count != 0)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "There is an overlapping shift";

                return serviceResponse;
            }

            // You cannot assign the same shift to two or more employees
            var employeeIdsCheck = await selectShiftsQuery.GetEmployeeByShiftId(shiftId);

            foreach (var employeeIdCheck in employeeIdsCheck)
            {
                if (employeeIdCheck != null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "This shift already has employee";

                    return serviceResponse;
                }
            }

            ////serviceResponse.Data = Mapper.Map<GetShiftDto>(shift);

            return serviceResponse;
        }
        
    }
}