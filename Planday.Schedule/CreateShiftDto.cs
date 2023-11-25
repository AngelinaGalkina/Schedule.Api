
namespace Planday.Schedule
{
    public class CreateShiftDto
    {
        public long? EmployeeId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
