using Planday.Schedule.Models;

namespace Planday.Schedule.Validations
{
    // TODO rename to Validator
    public interface ICreateShiftValidations
    {
        public bool Validate(Shift newShift);
    }
}
