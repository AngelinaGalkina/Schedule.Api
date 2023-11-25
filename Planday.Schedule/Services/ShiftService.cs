using Newtonsoft.Json;
using Planday.Schedule.ApiClient;
using Planday.Schedule.Models;
using Planday.Schedule.Queries.Insert;
using Planday.Schedule.Queries.Select;
using Planday.Schedule.Queries.Update;
using Planday.Schedule.ResponseModels;
using RestSharp;

namespace Planday.Schedule.Services
{
    public class ShiftService : IShiftService
    {
        private readonly ISelectShiftsQuery _selectShiftsQuery;

        private readonly IUpdateShiftsQuery _updateShiftsQuery;

        private readonly ISelectEmployeeQuery _selectEmployeeQuery;

        private readonly IInsertShiftsQuery _insertShiftsQuery;
        private readonly IEmailApiHandler _emailApiHandler;

        public ShiftService(ISelectShiftsQuery selectShiftsQuery,
            IUpdateShiftsQuery updateShiftsQuery,
            ISelectEmployeeQuery selectEmployeeQuery,
            IInsertShiftsQuery insertShiftsQuery,
            IEmailApiHandler emailApiHandler)
        {
            _selectShiftsQuery = selectShiftsQuery;
            _updateShiftsQuery = updateShiftsQuery;
            _selectEmployeeQuery = selectEmployeeQuery;
            _insertShiftsQuery = insertShiftsQuery;
            _emailApiHandler = emailApiHandler;
        }


        public async Task<ShiftByIdServiceResponse<Shift>> ShiftById(int id)
        {
            var shiftByIdServiceResponse = new ShiftByIdServiceResponse<Shift>
            {
                Success = true,
                Message = "Shift retrieved successfully"
            };
            var shift = await _selectShiftsQuery.ShiftById(id);

            if (shift == null)
            {
                shiftByIdServiceResponse.Success = false;
                shiftByIdServiceResponse.Message = "Couldn't get shift by id";

                return shiftByIdServiceResponse;
            }

            if (shift.EmployeeId == null)
            {
                shiftByIdServiceResponse.EmployeeEmail = "Employee id is null";

                return shiftByIdServiceResponse;
            }

            var employeeEmail = _emailApiHandler.EmployeeEmail((long)shift.EmployeeId);

            if (string.IsNullOrEmpty(employeeEmail))
            {
                shiftByIdServiceResponse.EmployeeEmail = "This employee doesn't have email";
               
                return shiftByIdServiceResponse;
            }

            shiftByIdServiceResponse.EmployeeEmail = employeeEmail;
            shiftByIdServiceResponse.Data = shift;

            return shiftByIdServiceResponse;
        }

        public async Task<ServiceResponse<Shift>> AddShift(Shift newShift)
        {
            var serviceResponse = new ServiceResponse<Shift>();
            var result = newShift.Start.CompareTo(newShift.End);

            if (result > 0)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Start time is grater than end time";

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
            var serviceResponse = new ServiceResponse<Shift>();
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

            // Validation for the condition:  an employee cannot be assigned
            // to a shift in a time where that employee is already working
            var newShiftStart = shift.Start;
            var newShiftEnd = shift.End;
            var oveplappingShift = await _selectShiftsQuery.OverlappingShifts(employeeId, newShiftStart, newShiftEnd);

            if (oveplappingShift.Count != 0)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "There is an overlapping shift";

                return serviceResponse;
            }

            // You cannot assign the same shift to two or more employees
            var employeeIdsCheck = await _selectShiftsQuery.GetEmployeeByShiftId(shiftId);

            foreach (var employeeIdCheck in employeeIdsCheck)
            {
                if (employeeIdCheck != null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "This shift already has employee";

                    return serviceResponse;
                }
            }

            var updatedShift = await _updateShiftsQuery.UpdateEmployeeId(shiftId, employeeId);

            serviceResponse.Data = updatedShift;

            return serviceResponse;
        }

        
    }
}