using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class SelectShiftsQuery : ISelectShiftsQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        private const string Sql = @"SELECT Id, EmployeeId, Start, End FROM Shift;";

        public SelectShiftsQuery(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }
    
        public async Task<IReadOnlyCollection<Shift>> AllShifts()
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var sqlResponse = await sqlConnection.QueryAsync<Shift>(Sql);
            var shifts = sqlResponse.Select(x => 
                new Shift(x.Id, x.EmployeeId, x.Start, x.End));
        
            return shifts.ToList();
        }

        public async Task<Shift> GetShiftById(long id)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var sqlResponse = await sqlConnection.QueryAsync<Shift>(Sql);
            var shift = sqlResponse.FirstOrDefault(x => x.Id == id);

            return shift ?? throw new ArgumentException($"Shift with ID {id} not found.");
        }
    }
}

