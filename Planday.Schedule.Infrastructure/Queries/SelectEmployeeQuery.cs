using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class SelectEmployeeQuery : ISelectEmployeeQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        private const string Sql = @"SELECT Id, Name FROM Employee;";

        public SelectEmployeeQuery(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }
    
        public async Task<Employee> GetEmployeeById(long? id)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var sqlResponse = await sqlConnection.QueryAsync<EmployeeDto>(Sql);

            var employees = sqlResponse.Select(x =>
              new Employee(x.Id, x.Name));

            var employee = employees.FirstOrDefault(x => x.Id == id);

            return employee;
        }
    }
}

