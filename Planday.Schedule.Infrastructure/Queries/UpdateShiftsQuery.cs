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

        public async Task<bool> AddShift(Shift shift)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            await sqlConnection.OpenAsync();
            await using var transaction = sqlConnection.BeginTransaction();

            var sqlText = shift.EmployeeId == null ? SqlInsertStartNoEmployeeId : SqlInsertStartWithEmployeeId;

            await using var command = new SqliteCommand(sqlText, sqlConnection, transaction);

            if (shift.EmployeeId != null)
            {
                command.Parameters.AddWithValue("@EmployeeId", shift.EmployeeId);
            }

            command.Parameters.AddWithValue("@Start", shift.Start);
            command.Parameters.AddWithValue("@End", shift.End);

            try
            {
                int rowsAffected = await command.ExecuteNonQueryAsync();
                transaction.Commit();
                
                return true;
            }
            catch (Exception ex)
            {
                // Handle exceptions here, such as logging or throwing custom exceptions.
                // Rollback the transaction on failure.
                transaction.Rollback();
                
                return false;
            }
        }
    }
}

