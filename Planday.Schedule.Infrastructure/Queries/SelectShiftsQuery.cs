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

        public async Task<Shift> GetShiftById(long? id)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var sqlResponse = await sqlConnection.QueryAsync<AddShift>(Sql);

            var shifts = sqlResponse.Select(x =>
              new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));

            var shift = shifts.FirstOrDefault(x => x.Id == id);

            return shift;
        }

        public async Task<IReadOnlyCollection<Shift>> OverlappingShifts(long? employeeId, DateTime newStart, DateTime newEnd)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var sqlTextStart = $"SELECT * FROM Shift WHERE EmployeeId = {employeeId} AND ( ";
            var sqlConditionAnd = $"(Start <= '{newStart}' AND End > '{newEnd}') OR ";
            var sqlConditionOr = $"(Start < '{newStart}' AND End >= '{newEnd}')  OR ";
            var sqlTextEnd = $"('{newStart}' <= Start AND '{newEnd}' >= End) ); ";
            var sqlText = $"{sqlTextStart}{sqlConditionAnd}{sqlConditionOr}{sqlTextEnd}";
            var sqlResponse = await sqlConnection.QueryAsync<AddShift>(sqlText);

            var shifts = sqlResponse.Select(x =>
              new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));
            
            return shifts.ToList();
        }


        public async Task<IReadOnlyCollection<long?>> GetEmployeeByShiftId(long? id)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var sqlText = $"SELECT * FROM Shift WHERE Id = {id};";

            var sqlResponse = await sqlConnection.QueryAsync<AddShift>(Sql);

            var shifts = sqlResponse.Select(x =>
              new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));

            var employeeIdList = new List<long?>();

            foreach (var shift in shifts)
            {
                employeeIdList.Add(shift.EmployeeId);
            }

            return employeeIdList;
        }
    }
}

