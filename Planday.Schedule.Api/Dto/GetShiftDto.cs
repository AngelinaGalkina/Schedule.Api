namespace Planday.Schedule.Api.Dto
{
    public class GetShiftDto
    {
        ////public int EmployeeId { get; set; }
        ////public DateTime Start { get; set; }
        ////public DateTime End { get; set; }
        ///
        public long Id { get; set; }
        public long? EmployeeId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
