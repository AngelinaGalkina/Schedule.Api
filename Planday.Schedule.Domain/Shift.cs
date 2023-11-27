namespace Planday.Schedule.Domain;


public class Shift
{
    public Shift(long id, Employee employee, DateTime start, DateTime end)
    {
        Id = id;
        Employee = employee;
        Start = start;
        End = end;
    }

    public Employee Employee { get; set; }

    public long Id { get; set; }
    public DateTime Start { get; }
    public DateTime End { get; }
}
