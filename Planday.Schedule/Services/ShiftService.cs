using Planday.Schedule.ApiClient;
using Planday.Schedule.Models;
using Planday.Schedule.Queries.Insert;
using Planday.Schedule.Queries.Select;
using Planday.Schedule.Queries.Update;
using Planday.Schedule.Validators;

namespace Planday.Schedule.Services;

public class ShiftService : IShiftService
{
    private readonly ISelectShiftsQuery _selectShiftsQuery;

    private readonly IUpdateShiftsQuery _updateShiftsQuery;

    private readonly ISelectEmployeeQuery _selectEmployeeQuery;

    private readonly IInsertShiftsQuery _insertShiftsQuery;

    private readonly IEmailApiClient _emailApiHandler;
    
    private readonly ICreateShiftValidator _createShiftValidations;
   
    private readonly IAssignShiftToEmployeeValidator _assignShiftToEmployeeValidations;

    public ShiftService(ISelectShiftsQuery selectShiftsQuery,
        IUpdateShiftsQuery updateShiftsQuery,
        ISelectEmployeeQuery selectEmployeeQuery,
        IInsertShiftsQuery insertShiftsQuery,
        IEmailApiClient emailApiHandler,
        ICreateShiftValidator createShiftValidations,
        IAssignShiftToEmployeeValidator assignShiftToEmployeeValidations)
    {
        _selectShiftsQuery = selectShiftsQuery;
        _updateShiftsQuery = updateShiftsQuery;
        _selectEmployeeQuery = selectEmployeeQuery;
        _insertShiftsQuery = insertShiftsQuery;
        _emailApiHandler = emailApiHandler;
        _createShiftValidations = createShiftValidations;
        _assignShiftToEmployeeValidations = assignShiftToEmployeeValidations;
    }

    public async Task<EmployeeShift> ShiftById(long id)
    {
        var shift = await _selectShiftsQuery.ShiftById(id);

        if (shift == null)
        {
            throw new Exception($"Couldn't find shift with id: {id}");
        }

        var shiftWithEmployeeInfo = ShiftWithEmployeeInfo(shift);

        return shiftWithEmployeeInfo ?? new EmployeeShift(shift.Id, shift.EmployeeId, shift.Start, shift.End);
    }

    public async Task<Shift> CreateShift(Shift newShift)
    {
        _createShiftValidations.Validate(newShift);

        var newShiftId = await _insertShiftsQuery.InsertShift(newShift);

        if (newShiftId == null)
        {
            throw new Exception($"Couldn't insert shift with id: {newShift.Id}");
        }

        // casting to long since value will never be null.
        var createdShift = await _selectShiftsQuery.ShiftById((long)newShiftId);

        if (createdShift == null)
        {
            throw new Exception($"Couldn't find created shift with id: {newShiftId}");
        }

        return createdShift;
    }

    public async Task<Shift> AssignShiftToEmployee(long employeeId, long shiftId)
    {
        var shift = await _selectShiftsQuery.ShiftById(shiftId);

        if (shift == null)
        {
            throw new Exception($"Couldn't find shift with id: {shiftId}");
        }

        var employee = await _selectEmployeeQuery.EmployeeById(employeeId);

        if (employee == null)
        {
            throw new Exception($"Couldn't find employee with id: {employeeId}");
        }

        await _assignShiftToEmployeeValidations.ValidateAsync(shift, employeeId, shiftId);

        var updatedShift = await _updateShiftsQuery.UpdateEmployeeId(shiftId, employeeId);

        if (updatedShift == null)
        {
            throw new Exception($"Couldn't update employee with id: {employeeId}");
        }

        return updatedShift;
    }

    private EmployeeShift? ShiftWithEmployeeInfo(Shift shift)
    {
        if (shift.EmployeeId != null)
        {
            var employeeInfo = _emailApiHandler.EmployeeEmail((long)shift.EmployeeId);

            if (employeeInfo != null)
            {
                return new EmployeeShift(shift.Id, shift.EmployeeId, shift.Start, shift.End)
                {
                    Name = employeeInfo.Name,
                    Email = employeeInfo.Email
                };
            }
        }

        return null;
    }
}