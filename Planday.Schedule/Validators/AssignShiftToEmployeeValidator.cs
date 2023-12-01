using Planday.Schedule.Domain;
using Planday.Schedule.Repositories;

namespace Planday.Schedule.Validators;

public class AssignShiftToEmployeeValidator : IAssignShiftToEmployeeValidator
{
    private readonly IShiftRepository _shiftRepository;

    public AssignShiftToEmployeeValidator(IShiftRepository shiftRepository)
    {
        _shiftRepository = shiftRepository;
    }

    public async Task ValidateAsync(Shift shift, long employeeId, long shiftId)
    {
        // Validation for the condition:  an employee cannot be assigned
        // to a shift in a time where that employee is already working
        var newShiftStart = shift.Start;
        var newShiftEnd = shift.End;
        var oveplappingShift = await _shiftRepository.OverlappingShifts(employeeId, newShiftStart, newShiftEnd);

        if (oveplappingShift.Count != 0)
        {
            throw new Exception($"This employee: {employeeId} has overlapping shifts");
        }

        // You cannot assign the same shift to two or more employees
        var employeeIdsCheck = await _shiftRepository.EmployeeByShiftId(shiftId);

        foreach (var employeeIdCheck in employeeIdsCheck)
        {
            if (employeeIdCheck != null)
            {
                throw new Exception($"This shift already has employee, employee id: {employeeIdCheck}");
            }
        }
    }
}