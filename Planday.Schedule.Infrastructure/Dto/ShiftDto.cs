namespace Planday.Schedule.Infrastructure.Dto;

public record ShiftDto(long Id, long? EmployeeId, string Start, string End);