using Planday.Schedule.Domain;
using Planday.Schedule.Models;

namespace Planday.Schedule.Repositories;

public interface IEmployeeRepository
{
    Task<Employee> EmployeeById(long? id);
    EmployeeInfo? EmployeeEmail(long employeeId);
}