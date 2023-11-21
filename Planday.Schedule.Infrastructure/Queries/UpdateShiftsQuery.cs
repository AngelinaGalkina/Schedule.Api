using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Queries;

namespace Planday.Schedule.Infrastructure.Queries
{
    public class UpdateShiftsQuery : IUpdateShiftsQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        public UpdateShiftsQuery(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }


        private const string SqlInsertStartWithEmployeeId = @"INSERT INTO Shift (EmployeeId, Start, End) VALUES (@EmployeeId, @Start, @End)";
        private const string SqlInsertStartNoEmployeeId = @"INSERT INTO Shift (Start, End) VALUES (@Start, @End)";

        public async Task<long?> AddShift(AddShiftDto shift)
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

            var newShiftId = (long?)command.ExecuteScalar();

            transaction.Commit();

            return newShiftId;
        }

        public async Task<Shift?> UpdateEmployeeId(long shiftId, long newEmployeeId)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            var sqlUpdateEmployeeId = "UPDATE shift SET EmployeeId = @NewEmployeeId WHERE Id = @ShiftId;";

            await sqlConnection.OpenAsync();
            await using var transaction = sqlConnection.BeginTransaction();

            await using var command = new SqliteCommand(sqlUpdateEmployeeId, sqlConnection, transaction);

            command.Parameters.AddWithValue("@NewEmployeeId", newEmployeeId);
            command.Parameters.AddWithValue("@ShiftId", shiftId);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return null;
            }

            transaction.Commit();

            var selectSql = $"SELECT * FROM shift WHERE Id = {shiftId};";
            var sqlResponse = await sqlConnection.QueryAsync<AddShift>(selectSql);

            var shift = sqlResponse.Select(x =>
                         new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End))).SingleOrDefault();
            return shift;
        }
    }
}


