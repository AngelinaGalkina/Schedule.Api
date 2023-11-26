using Planday.Schedule.Models;

namespace Planday.Schedule.Validators;

public class CreateShiftValidator : ICreateShiftValidator
{
    public void Validate(ShiftBase newShift)
    {
        var result = newShift.Start.CompareTo(newShift.End);

        if (result > 0)
        {
            throw new Exception("Can't create this shift, End time is bigger then start time");
        }
    }
}