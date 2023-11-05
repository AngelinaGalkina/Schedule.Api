using AutoMapper;
using Dapper;
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

        private const string SqlInsertStart = @"INSERT INTO Shift (Id, EmployeeId, Start, End)";

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

            var sqlInsertEnd = $"VALUES({shift.Id}, {shift.EmployeeId}, '{shift.Start}', '{shift.End}')";
            var sqlText = string.Concat(SqlInsertStart, sqlInsertEnd);

            await sqlConnection.OpenAsync();

            var command = sqlConnection.CreateCommand();
            command.CommandText = sqlText;

            int rowsAffected = await command.ExecuteNonQueryAsync();

            var shifts = await sqlConnection.QueryAsync<GetShiftDto>(SqlSelect);

            var shiftsDto = shifts.Select(x =>
                new Shift(x.Id, x.EmployeeId, DateTime.Parse(x.Start), DateTime.Parse(x.End)));

            serviceResponse.Data = shiftsDto.Select(c => Mapper.Map<GetShiftDto>(c)).ToList();
            
            return serviceResponse;
        }
    }
}