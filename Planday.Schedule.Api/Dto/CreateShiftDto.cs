namespace Planday.Schedule.Api.Dto
{
    public class CreateShiftDto
    {
        public long? EmployeeId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
