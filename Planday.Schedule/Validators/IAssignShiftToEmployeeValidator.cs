using Planday.Schedule.Models;

namespace Planday.Schedule.Validators
{
    public interface IAssignShiftToEmployeeValidator
    {
        public Task<bool> ValidateAsync(Shift shift, long employeeId, long shiftId);
    }
}
