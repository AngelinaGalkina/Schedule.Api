namespace Planday.Schedule.Models;

public class Shift
{
    public Shift(long id, long? employeeId, DateTime start, DateTime end)
    {
        Id = id;
        EmployeeId = employeeId;
        Start = start;
        End = end;
    }

    public long Id { get; set; }
    public long? EmployeeId { get; set; }
    public DateTime Start { get; }
    public DateTime End { get; }
}

