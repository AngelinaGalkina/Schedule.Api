using Planday.Schedule.Models;

namespace Planday.Schedule.Validators;

public interface ICreateShiftValidator
{
    public void Validate(Shift newShift);
}
