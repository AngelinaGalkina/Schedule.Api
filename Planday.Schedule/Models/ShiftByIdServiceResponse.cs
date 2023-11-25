namespace Planday.Schedule.Models
{
    public class ShiftByIdServiceResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
