using Planday.Schedule.Domain;

namespace Planday.Schedule.Models;

public class EmployeeShift : Shift
{
    public EmployeeShift(long id, Employee employee, DateTime start, DateTime end)
        : base(id, employee, start, end)
    {
    }

    public string? EmployeeName { get; set; }

    public string? EmployeeEmail { get; set; }
}
