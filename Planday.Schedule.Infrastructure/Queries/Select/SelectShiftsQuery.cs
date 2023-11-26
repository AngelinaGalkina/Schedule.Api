using AutoMapper;
using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Dto;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Models;
using Planday.Schedule.Queries.Select;
using System.Text;

namespace Planday.Schedule.Infrastructure.Queries.Select
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

        public async Task<Shift?> ShiftById(long id)
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

        public async Task<IReadOnlyCollection<Shift>> OverlappingShifts(long employeeId, DateTime newStart, DateTime newEnd)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var sqlQueryBuilder = new StringBuilder("SELECT * FROM Shift WHERE EmployeeId = @EmployeeId AND (");
            var parameters = new { EmployeeId = employeeId, NewStart = newStart, NewEnd = newEnd };

            // Conditions for overlapping shifts
            sqlQueryBuilder.Append("(Start <= @NewStart AND End > @NewEnd) OR ");
            sqlQueryBuilder.Append("(Start < @NewStart AND End >= @NewEnd) OR ");
            sqlQueryBuilder.Append("(@NewStart <= Start AND @NewEnd >= End)");

            // Complete the SQL query
            sqlQueryBuilder.Append(");");

            var sqlText = sqlQueryBuilder.ToString();
            var sqlResponse = await sqlConnection.QueryAsync<ShiftDto>(sqlText, parameters);

            var shifts = _mapper.Map<List<Shift>>(sqlResponse);

            return shifts;
        }


        public async Task<IReadOnlyCollection<long?>> EmployeeByShiftId(long id)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var sqlText = $"SELECT * FROM Shift WHERE Id = {id};";

            var sqlResponse = await sqlConnection.QueryAsync<ShiftDto>(sqlText);

            var shifts = _mapper.Map<List<Shift>>(sqlResponse);

            // employee id can be null.
            var employeeIdList = shifts.Select(shift => shift.EmployeeId).ToList();

            return employeeIdList;
        }
    }
}

