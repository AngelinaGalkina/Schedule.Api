using Planday.Schedule.Models;
using Planday.Schedule.Queries.Select;

namespace Planday.Schedule.Validators;

public class AssignShiftToEmployeeValidator : IAssignShiftToEmployeeValidator
{
    private readonly ISelectShiftsQuery _selectShiftsQuery;

    public AssignShiftToEmployeeValidator(ISelectShiftsQuery selectShiftsQuery)
    {
        _selectShiftsQuery = selectShiftsQuery;
    }

    public async Task ValidateAsync(Shift shift, long employeeId, long shiftId)
    {
        // Validation for the condition:  an employee cannot be assigned
        // to a shift in a time where that employee is already working
        var newShiftStart = shift.Start;
        var newShiftEnd = shift.End;
        var oveplappingShift = await _selectShiftsQuery.OverlappingShifts(employeeId, newShiftStart, newShiftEnd);

        if (oveplappingShift.Count != 0)
        {
            throw new Exception($"This employee: {employeeId} has overlapping shifts");
        }

        // You cannot assign the same shift to two or more employees
        var employeeIdsCheck = await _selectShiftsQuery.EmployeeByShiftId(shiftId);

        foreach (var employeeIdCheck in employeeIdsCheck)
        {
            if (employeeIdCheck != null)
            {
                throw new Exception($"This shift already has employee, employee id: {employeeIdCheck}");
            }
        }
    }
}