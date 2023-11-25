using Planday.Schedule.Models;

namespace Planday.Schedule.Validations
{
    public interface IAssignShiftToEmployeeValidations
    {
        public Task<bool> ValidateAsync(Shift shift, long employeeId, long shiftId);
    }
}
