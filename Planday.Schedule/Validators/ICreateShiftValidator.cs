using Planday.Schedule.Models;

namespace Planday.Schedule.Validators
{
    public interface ICreateShiftValidator
    {
        public bool Validate(Shift newShift);
    }
}
