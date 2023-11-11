
namespace Planday.Schedule
{
    public class AddShift
    {
        public long Id { get; set; }
        public long? EmployeeId { get; set; }
        public string Start { get; }
        public string End { get; }
    }
}

