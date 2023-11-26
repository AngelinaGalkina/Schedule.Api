using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Models;
using Planday.Schedule.Queries.Insert;

namespace Planday.Schedule.Infrastructure.Queries.Insert;

public class InsertShiftsQuery : IInsertShiftsQuery
{
    private readonly IConnectionStringProvider _connectionStringProvider;

    public InsertShiftsQuery(IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }


    private const string SqlInsertStartWithEmployeeId = @"INSERT INTO Shift (EmployeeId, Start, End) VALUES (@EmployeeId, @Start, @End)";
    private const string SqlInsertStartNoEmployeeId = @"INSERT INTO Shift (Start, End) VALUES (@Start, @End)";

    public async Task<long?> InsertShift(Shift shift)
    {
        await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

        await sqlConnection.OpenAsync();
        await using var transaction = sqlConnection.BeginTransaction();

        var sqlText = shift.EmployeeId == null
            ? $"{SqlInsertStartNoEmployeeId}"
            : $"{SqlInsertStartWithEmployeeId}";

        await using var command = new SqliteCommand(sqlText, sqlConnection, transaction);

        if (shift.EmployeeId != null)
        {
            command.Parameters.AddWithValue("@EmployeeId", shift.EmployeeId);
        }

        command.Parameters.AddWithValue("@Start", shift.Start);
        command.Parameters.AddWithValue("@End", shift.End);

        await command.ExecuteNonQueryAsync();

        // Execute the command and get the last inserted ID
        command.CommandText = "SELECT last_insert_rowid();";

        // This value can be null.
        var newShiftId = (long?)command.ExecuteScalar();

        transaction.Commit();

        return newShiftId;
    }
}


