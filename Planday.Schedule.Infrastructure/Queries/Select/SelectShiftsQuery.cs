using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Dto;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Models;
using Planday.Schedule.Queries.Select;
using System.Text;

namespace Planday.Schedule.Infrastructure.Queries.Select;

public class SelectShiftsQuery : ISelectShiftsQuery
{
    private readonly IConnectionStringProvider _connectionStringProvider;

    public SelectShiftsQuery(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
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

        var shift = new Shift(
                shiftDto.Id,
                shiftDto.EmployeeId,
                DateTime.Parse(shiftDto.Start),
                DateTime.Parse(shiftDto.End));

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

        var shiftDtos = await sqlConnection.QueryAsync<ShiftDto>(sqlText, parameters);

        var shifts = shiftDtos.Select(x => new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));

        return shifts.ToList();
    }


    public async Task<IReadOnlyCollection<long?>> EmployeeByShiftId(long id)
    {
        await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

        var sqlText = $"SELECT * FROM Shift WHERE Id = {id};";
        var shiftDtos = await sqlConnection.QueryAsync<ShiftDto>(sqlText);

        var shifts = shiftDtos.Select(x => new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));
        var employeeIdList = shifts.Select(x => x.EmployeeId).ToList();

        return employeeIdList;
    }
}

