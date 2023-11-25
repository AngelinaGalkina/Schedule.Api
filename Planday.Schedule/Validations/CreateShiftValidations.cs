using Planday.Schedule.Models;

namespace Planday.Schedule.Validations
{
    public class CreateShiftValidations : ICreateShiftValidations
    {
        public bool Validate(Shift newShift)
        {
            var result = newShift.Start.CompareTo(newShift.End);

            if (result > 0)
            {
                return false;
            }

            return true;
        }
    }
}