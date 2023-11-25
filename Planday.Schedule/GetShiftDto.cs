namespace Planday.Schedule
{
    public class GetShiftDto
    {
        public long Id { get; set; }
        public long? EmployeeId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
