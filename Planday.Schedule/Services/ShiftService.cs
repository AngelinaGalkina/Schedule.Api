using AutoMapper;
using Newtonsoft.Json;
using Planday.Schedule.Queries.Insert;
using Planday.Schedule.Queries.Select;
using Planday.Schedule.Queries.Update;
using Planday.Schedule.Models;
using RestSharp;
using System.Net;
using static System.Net.WebRequestMethods;

namespace Planday.Schedule.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IMapper _mapper;

        private readonly ISelectShiftsQuery _selectShiftsQuery;

        private readonly IUpdateShiftsQuery _updateShiftsQuery;

        private readonly ISelectEmployeeQuery _selectEmployeeQuery;

        private readonly IInsertShiftsQuery _insertShiftsQuery;

        public ShiftService(IMapper mapper,
            ISelectShiftsQuery selectShiftsQuery,
            IUpdateShiftsQuery updateShiftsQuery,
            ISelectEmployeeQuery selectEmployeeQuery,
            IInsertShiftsQuery insertShiftsQuery)
        {
            _mapper = mapper;
            _selectShiftsQuery = selectShiftsQuery;
            _updateShiftsQuery = updateShiftsQuery;
            _selectEmployeeQuery = selectEmployeeQuery;
            _insertShiftsQuery = insertShiftsQuery;
        }


        public async Task<ShiftByIdServiceResponse<GetShiftDto>> ShiftById(int id)
        {
            var shiftByIdServiceResponse = new ShiftByIdServiceResponse<GetShiftDto>();
            var shift = await _selectShiftsQuery.ShiftById(id);

            shiftByIdServiceResponse.Data = _mapper.Map<GetShiftDto>(shift);

            if (shift.EmployeeId == null)
            {
                shiftByIdServiceResponse.EmployeeEmail = "This employee doesn't have email";

                return shiftByIdServiceResponse;
            }

            var employeeEmail = EmployeeEmail((long)shift.EmployeeId);

            shiftByIdServiceResponse.EmployeeEmail = employeeEmail;

            return shiftByIdServiceResponse;
        }

        public async Task<ServiceResponse<GetShiftDto>> AddShift(CreateShiftDto newShift)
        {
            var serviceResponse = new ServiceResponse<GetShiftDto>();
            var result = newShift.Start.CompareTo(newShift.End);

            if (result > 0)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Start time is grater than end time";

                return serviceResponse;
            }

            var shift = _mapper.Map<AddShiftDto>(newShift);

            try
            {
                var newShiftId = await _insertShiftsQuery.InsertShift(shift);

                if (newShiftId == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Couldn't add new shift, something went wrong";

                    return serviceResponse;
                }

                var addedShift = await _selectShiftsQuery.ShiftById(newShiftId);
                var shiftDto = _mapper.Map<GetShiftDto>(addedShift);

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
            var serviceResponse = new ServiceResponse<GetShiftDto>();
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
            var shiftDto = _mapper.Map<GetShiftDto>(updatedShift);

            serviceResponse.Data = shiftDto;

            return serviceResponse;
        }

        private string EmployeeEmail(long employeeId)
        {
            var url = $"http://planday-employee-api-techtest.westeurope.azurecontainer.io:5000/employee/{employeeId}";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Get);

            request.AddHeader("accept", "*");
            request.AddHeader("Authorization", "8e0ac353-5ef1-4128-9687-fb9eb8647288");

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var responseData = response.Content;

                if (responseData == null)
                {
                    return null;
                }

                // Deserialize the JSON string into a dynamic object
                dynamic jsonObject = JsonConvert.DeserializeObject(responseData);

                // Access the "email" property
                var email = jsonObject.email;

                return email;
            }
            else
            {
                return null;
            }
        }
    }
}