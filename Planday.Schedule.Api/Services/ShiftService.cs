using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Api;
using Planday.Schedule.Api.Dto;
using Planday.Schedule.Api.Models;
using Planday.Schedule.Infrastructure.Providers;
using Planday.Schedule.Infrastructure.Providers.Interfaces;

namespace Planday.Schedule.Api.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IMapper Mapper;

        private readonly IHttpContextAccessor HttpContextAccessor;

        private readonly IConnectionStringProvider _connectionStringProvider;

        private const string SqlSelect = @"SELECT Id, EmployeeId, Start, End FROM Shift;";

        private const string SqlInsertStartWithEmployeeId = @"INSERT INTO Shift (EmployeeId, Start, End)";
        
        private const string SqlInsertStartNoEmployeeId = @"INSERT INTO Shift (Start, End)";

        public ShiftService(IMapper mapper, IHttpContextAccessor httpContextAccessor, IConnectionStringProvider connectionStringProvider)
        {
            Mapper = mapper;
            HttpContextAccessor = httpContextAccessor;
            _connectionStringProvider = connectionStringProvider;
        }


        public async Task<ServiceResponse<GetShiftDto>> GetShiftById(int id)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var serviceResponse = new ServiceResponse<GetShiftDto>();
            var shiftDtos = await sqlConnection.QueryAsync<GetShiftDto>(SqlSelect);
            var shifts = shiftDtos.Select(x =>
                new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));
            var requestedShift = shifts.FirstOrDefault(x => x.Id == id);

            serviceResponse.Data = Mapper.Map<GetShiftDto>(requestedShift);

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetShiftDto>>> AddShift(GetShiftDto newShift)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

            var serviceResponse = new ServiceResponse<List<GetShiftDto>>();

            var shift = Mapper.Map<Shift>(newShift);
            var sqlInsertEnd = string.Empty;
            var sqlText = string.Empty;
            var result = shift.Start.CompareTo(shift.End);

            if (result <= 0)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Start time is less or equal than end time";

                return serviceResponse;
            }

            if (shift.Start.Date != shift.End.Date)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Start time and end time are not at the same day";

                return serviceResponse;
            }


            try
            {
                await sqlConnection.OpenAsync();
                using var transaction = sqlConnection.BeginTransaction(); // Begin the transaction

                if (shift.EmployeeId == null)
                {
                    sqlInsertEnd = $"VALUES('{shift.Start}', '{shift.End}')";
                    sqlText = string.Concat(SqlInsertStartNoEmployeeId, sqlInsertEnd);
                }
                else
                {
                    sqlInsertEnd = $"VALUES({shift.EmployeeId}, '{shift.Start}', '{shift.End}')";
                    sqlText = string.Concat(SqlInsertStartWithEmployeeId, sqlInsertEnd);
                }

                var command = sqlConnection.CreateCommand();
                command.CommandText = sqlText;
                command.Transaction = transaction; // Assign the transaction to the command

                int rowsAffected = await command.ExecuteNonQueryAsync();

                // Commit the transaction
                transaction.Commit();


                var shifts = await sqlConnection.QueryAsync<GetShiftDto>(SqlSelect);

                var shiftsDto = shifts.Select(x =>
                    new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));

                serviceResponse.Data = shiftsDto.Select(c => Mapper.Map<GetShiftDto>(c)).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}