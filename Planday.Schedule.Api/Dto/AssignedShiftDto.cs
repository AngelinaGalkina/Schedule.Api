namespace Planday.Schedule.Api.Dto
{
    public class AssignedShiftDto
    {
        public long Id { get; set; }
        public long? EmployeeId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeEmail { get; set; }
    }
}
