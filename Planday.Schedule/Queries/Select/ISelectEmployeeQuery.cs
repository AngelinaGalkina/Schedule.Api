using System.Collections.Generic;
using System.Threading.Tasks;
using Planday.Schedule.Models;

namespace Planday.Schedule.Queries.Select
{
    public interface ISelectEmployeeQuery
    {
        Task<Employee> GetEmployeeById(long? id);
    }
}

