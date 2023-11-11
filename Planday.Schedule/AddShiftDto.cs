
namespace Planday.Schedule
{
    public class AddShiftDto
    {
        public AddShiftDto(long? employeeId, string start, string end)
        {
            EmployeeId = employeeId;
            Start = start;
            End = end;
        }

        public long? EmployeeId { get; set; }
        public string Start { get; }
        public string End { get; }
    }
}

