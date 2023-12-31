﻿using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Domain;
using Planday.Schedule.Infrastructure.Dto;
using Planday.Schedule.Infrastructure.Providers;
using Planday.Schedule.Models;
using Planday.Schedule.Repositories;
using System.Text;

namespace Planday.Schedule.Infrastructure.Repository;

public class ShiftRepository : IShiftRepository
{
    private readonly IConnectionStringProvider _connectionStringProvider;
    
    private readonly IEmployeeRepository _employeeRepository;

    public ShiftRepository(IConnectionStringProvider connectionStringProvider, IEmployeeRepository employeeRepository)
    {
        _connectionStringProvider = connectionStringProvider;
        _employeeRepository = employeeRepository;
    }


    private const string SqlInsertStartWithEmployeeId = @"INSERT INTO Shift (EmployeeId, Start, End) VALUES (@EmployeeId, @Start, @End)";

    private const string SqlInsertStartNoEmployeeId = @"INSERT INTO Shift (Start, End) VALUES (@Start, @End)";

    public async Task<long?> InsertShift(ShiftBase shift)
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

        var employee = await _employeeRepository.EmployeeById(shiftDto.EmployeeId);
        var shift = new Shift(
                shiftDto.Id,
                employee,
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
        var shifts = new List<Shift>();

        foreach (var shiftDto in shiftDtos)
        {
            var employee = await _employeeRepository.EmployeeById(shiftDto.EmployeeId);

            shifts.Add(new Shift(shiftDto.Id, employee, DateTime.Parse(shiftDto.Start), DateTime.Parse(shiftDto.End)));
        }

        return shifts;
    }

    public async Task<IReadOnlyCollection<long?>> EmployeeByShiftId(long id)
    {
        await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());

        var sqlText = $"SELECT * FROM Shift WHERE Id = {id};";
        var shiftDtos = await sqlConnection.QueryAsync<ShiftDto>(sqlText);

        var shifts = new List<Shift>();

        foreach (var shiftDto in shiftDtos)
        {
            var employee = await _employeeRepository.EmployeeById(shiftDto.EmployeeId);

            shifts.Add(new Shift(shiftDto.Id, employee, DateTime.Parse(shiftDto.Start), DateTime.Parse(shiftDto.End)));
        }

        var employeeIdList = shifts.Select(x => x.Employee?.Id).ToList();

        return employeeIdList;
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
        var shiftDtos = await sqlConnection.QueryAsync<ShiftDto>(selectSql);
        var shifts = new List<Shift>();

        foreach (var shiftDto in shiftDtos)
        {
            var employee = await _employeeRepository.EmployeeById(shiftDto.EmployeeId);

            shifts.Add(new Shift(shiftDto.Id, employee, DateTime.Parse(shiftDto.Start), DateTime.Parse(shiftDto.End)));
        }

        return shifts.FirstOrDefault();
    }
}
