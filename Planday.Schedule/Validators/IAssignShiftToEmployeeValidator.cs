using Planday.Schedule.Domain;

namespace Planday.Schedule.Validators;

public interface IAssignShiftToEmployeeValidator
{
    public Task ValidateAsync(Shift shift, long employeeId, long shiftId);
}
