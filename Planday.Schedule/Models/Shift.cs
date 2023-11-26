namespace Planday.Schedule.Models;

public class Shift : ShiftBase
{
    public Shift(long id, long? employeeId, DateTime start, DateTime end) : base(employeeId, start, end) 
    {
        Id = id;
    }

    public long Id { get; set; }
}

