namespace Planday.Schedule.Models;

public class EmployeeShift : Shift
{
    public EmployeeShift(long id, long? employeeId, DateTime start, DateTime end)
        : base(id, employeeId, start, end)
    {
    }

    public string? EmployeeName { get; set; }

    public string? EmployeeEmail { get; set; }
}
