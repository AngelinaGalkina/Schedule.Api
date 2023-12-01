using Dapper;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Planday.Schedule.Domain;
using Planday.Schedule.Infrastructure.Dto;
using Planday.Schedule.Infrastructure.Providers;
using Planday.Schedule.Models;
using Planday.Schedule.Repositories;
using RestSharp;

namespace Planday.Schedule.Infrastructure.Repository;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IConnectionStringProvider _connectionStringProvider;

    private const string Sql = @"SELECT Id, Name FROM Employee;";

    public EmployeeRepository(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    public async Task<Employee> EmployeeById(long? id)
    {
        await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

        var sqlResponse = await sqlConnection.QueryAsync<EmployeeDto>(Sql);

        var employees = sqlResponse.Select(x =>
          new Employee(x.Id, x.Name, null));

        var employee = employees.FirstOrDefault(x => x.Id == id);

        return employee;
    }

    public EmployeeInfo? EmployeeEmail(long employeeId)
    {
        var url = $"http://planday-employee-api-techtest.westeurope.azurecontainer.io:5000/employee/{employeeId}";
        var client = new RestClient(url);
        var request = new RestRequest(url, Method.Get);

        request.AddHeader("accept", "*");
        request.AddHeader("Authorization", "8e0ac353-5ef1-4128-9687-fb9eb8647288"); // move to env variable

        var response = client.Execute(request);

        if (response.IsSuccessful)
        {
            var responseData = response.Content;

            if (responseData == null)
            {
                return null;
            }

            // Deserialize the JSON string into a dynamic object
            dynamic jsonObject = JsonConvert.DeserializeObject(responseData);

            if (jsonObject == null)
            {
                return null;
            }

            // Access the "email" property
            string email = jsonObject.email;
            string name = jsonObject.name;

            var retVal = new EmployeeInfo(name, email);

            return retVal;
        }
        else
        {
            return null;
        }
    }
}