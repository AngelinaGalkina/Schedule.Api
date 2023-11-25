using Newtonsoft.Json;
using Planday.Schedule.ApiClient;
using Planday.Schedule.Models;
using Planday.Schedule.Queries.Insert;
using Planday.Schedule.Queries.Select;
using Planday.Schedule.Queries.Update;
using Planday.Schedule.ResponseModels;
using Planday.Schedule.Validations;
using RestSharp;

namespace Planday.Schedule.Services
{
    public class ShiftService : IShiftService
    {
        private readonly ISelectShiftsQuery _selectShiftsQuery;

        private readonly IUpdateShiftsQuery _updateShiftsQuery;

        private readonly ISelectEmployeeQuery _selectEmployeeQuery;

        private readonly IInsertShiftsQuery _insertShiftsQuery;

        private readonly IEmailApiClient _emailApiHandler;
        private readonly ICreateShiftValidations _createShiftValidations;
        private readonly IAssignShiftToEmployeeValidations _assignShiftToEmployeeValidations;


        public ShiftService(ISelectShiftsQuery selectShiftsQuery,
            IUpdateShiftsQuery updateShiftsQuery,
            ISelectEmployeeQuery selectEmployeeQuery,
            IInsertShiftsQuery insertShiftsQuery,
            IEmailApiClient emailApiHandler,
            ICreateShiftValidations createShiftValidations,
            IAssignShiftToEmployeeValidations assignShiftToEmployeeValidations)
        {
            _selectShiftsQuery = selectShiftsQuery;
            _updateShiftsQuery = updateShiftsQuery;
            _selectEmployeeQuery = selectEmployeeQuery;
            _insertShiftsQuery = insertShiftsQuery;
            _emailApiHandler = emailApiHandler;
            _createShiftValidations = createShiftValidations;
            _assignShiftToEmployeeValidations = assignShiftToEmployeeValidations;
        }


        public async Task<ShiftByIdServiceResponse<Shift>> ShiftById(int id)
        {
            var shiftByIdServiceResponse = new ShiftByIdServiceResponse<Shift>
            {
                Success = true,
                Message = "Shift retrieved successfully"
            };
            try
            {
                var shift = await _selectShiftsQuery.ShiftById(id);

                if (shift == null)
                {
                    shiftByIdServiceResponse.Success = false;
                    shiftByIdServiceResponse.Message = "Couldn't get shift by id";

                    return shiftByIdServiceResponse;
                }


                if (shift.EmployeeId != null)
                {
                    var employeeEmail = _emailApiHandler.EmployeeEmail((long)shift.EmployeeId);

                    shiftByIdServiceResponse.EmployeeEmail = employeeEmail;
                    shiftByIdServiceResponse.Data = shift;
                }

            }
            catch (Exception ex)
            {
                shiftByIdServiceResponse.Success = false;
                shiftByIdServiceResponse.Message = ex.Message;
            }

            return shiftByIdServiceResponse;
        }

        public async Task<ServiceResponse<Shift>> CreateShift(Shift newShift)
        {
            var serviceResponse = new ServiceResponse<Shift>
            {
                Success = true,
                Message = "Shift created successfully"
            };
            var ok = _createShiftValidations.Validate(newShift);

            if (!ok)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Couldn't add new shift";

                return serviceResponse;
            }

            try
            {
                var newShiftId = await _insertShiftsQuery.InsertShift(newShift);

                if (newShiftId == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Couldn't add new shift, something went wrong";

                    return serviceResponse;
                }

                var addedShift = await _selectShiftsQuery.ShiftById(newShiftId);

                serviceResponse.Data = addedShift;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<Shift>> AssignShiftToEmployee(long employeeId, long shiftId)
        {
            var serviceResponse = new ServiceResponse<Shift>
            {
                Success = true,
                Message = "Assign Shift successfully"
            };
            var shift = await _selectShiftsQuery.ShiftById(shiftId);

            if (shift == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "This shift doesn't exists";

                return serviceResponse;
            }

            var employee = await _selectEmployeeQuery.GetEmployeeById(employeeId);

            if (employee == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "This employee doesn't exists";

                return serviceResponse;
            }

            var ok = await _assignShiftToEmployeeValidations.ValidateAsync(shift, employeeId, shiftId);

            if (!ok)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Couldn't assign shift to employee";

                return serviceResponse;
            }

            var updatedShift = await _updateShiftsQuery.UpdateEmployeeId(shiftId, employeeId);

            serviceResponse.Data = updatedShift;

            return serviceResponse;
        }
    }
}