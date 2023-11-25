using AutoMapper;
using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Dto;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Models;
using Planday.Schedule.Queries.Select;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class SelectShiftsQuery : ISelectShiftsQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly IMapper _mapper;

        public SelectShiftsQuery(IConnectionStringProvider connectionStringProvider, IMapper mapper)
        {
            _connectionStringProvider = connectionStringProvider;
            _mapper = mapper;
        }

        public async Task<Shift?> ShiftById(long? id)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var sqlText = "SELECT Id, EmployeeId, Start, End FROM Shift WHERE Id = @Id;";
            var parameters = new { Id = id };

            var sqlResponse = await sqlConnection.QueryAsync<ShiftDto>(sqlText, parameters);
            var shiftDto = sqlResponse.FirstOrDefault();

            if (shiftDto == null)
            {
                return null;
            }

            var shift = _mapper.Map<Shift>(shiftDto);

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
            var sqlResponse = await sqlConnection.QueryAsync<ShiftDto>(sqlText);

            // TODO consider static on the lambda
            var shifts = sqlResponse.Select(x =>
              new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));
            
            return shifts.ToList();
        }


        public async Task<IReadOnlyCollection<long?>> GetEmployeeByShiftId(long? id)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var sqlText = $"SELECT * FROM Shift WHERE Id = {id};";

            var sqlResponse = await sqlConnection.QueryAsync<ShiftDto>(sqlText);

            var shifts = sqlResponse.Select(x =>
              new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));

            var employeeIdList = new List<long?>();
            /// TODO use select instead of foreach. Plus auto mapper 
            foreach (var shift in shifts)
            {
                employeeIdList.Add(shift.EmployeeId);
            }

            return employeeIdList;
        }
    }
}

