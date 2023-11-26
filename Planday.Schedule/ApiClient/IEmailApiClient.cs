using Planday.Schedule.Models;

namespace Planday.Schedule.ApiClient;

public interface IEmailApiClient
{
    public EmployeeInfo? EmployeeEmail(long employeeId);
}

