namespace Planday.Schedule.Api.Dto;

public class AssignedShiftDto : ShiftDto
{
    public string? EmployeeName { get; set; }
    public string? EmployeeEmail { get; set; }
}
