using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Infrastructure.Dto;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Models;
using Planday.Schedule.Queries.Update;

namespace Planday.Schedule.Infrastructure.Queries.Update
{
    public class UpdateShiftsQuery : IUpdateShiftsQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        public UpdateShiftsQuery(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
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
            var sqlResponse = await sqlConnection.QueryAsync<ShiftDto>(selectSql);

            var shift = sqlResponse.Select(x =>
                         new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End))).SingleOrDefault();
            return shift;
        }
    }
}


