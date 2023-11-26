namespace Planday.Schedule.Api.Models;

public class Response<T>
{
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}
