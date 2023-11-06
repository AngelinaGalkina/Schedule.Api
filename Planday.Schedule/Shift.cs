
namespace Planday.Schedule
{
    public class Shift
    {
        public Shift(long id, long? employeeId, string start, string end)
        {
            Id = id;
            EmployeeId = employeeId;
            Start = start;
            End = end;
        }

        public long Id { get; set; }
        public long? EmployeeId { get; set; }
        public string Start { get; }
        public string End { get; }
    }
}

