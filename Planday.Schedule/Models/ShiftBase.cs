namespace Planday.Schedule.Models;

public class ShiftBase
{
    public ShiftBase(long? employeeId, DateTime start, DateTime end)
    { 
        EmployeeId = employeeId;
        Start = start;
        End = end;
    }

    public long? EmployeeId { get; set; }
    public DateTime Start { get; }
    public DateTime End { get; }
}
