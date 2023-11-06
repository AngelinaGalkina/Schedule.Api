namespace Planday.Schedule.Api.Dto
{
    public class GetShiftDto
    {
        public long Id { get; set; }
        public long? EmployeeId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
