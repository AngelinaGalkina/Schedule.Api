using Planday.Schedule.Models;
using Planday.Schedule.Queries.Select;

namespace Planday.Schedule.Validators
{
    public class AssignShiftToEmployeeValidator : IAssignShiftToEmployeeValidator
    {
        private readonly ISelectShiftsQuery _selectShiftsQuery;

        public AssignShiftToEmployeeValidator(ISelectShiftsQuery selectShiftsQuery)
        {
            _selectShiftsQuery = selectShiftsQuery;
        }

        public async Task<bool> ValidateAsync(Shift shift, long employeeId, long shiftId)
        {
            // Validation for the condition:  an employee cannot be assigned
            // to a shift in a time where that employee is already working
            var newShiftStart = shift.Start;
            var newShiftEnd = shift.End;
            var oveplappingShift = await _selectShiftsQuery.OverlappingShifts(employeeId, newShiftStart, newShiftEnd);

            if (oveplappingShift.Count != 0)
            {
                return false;
            }

            // You cannot assign the same shift to two or more employees
            var employeeIdsCheck = await _selectShiftsQuery.GetEmployeeByShiftId(shiftId);

            foreach (var employeeIdCheck in employeeIdsCheck)
            {
                if (employeeIdCheck != null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}