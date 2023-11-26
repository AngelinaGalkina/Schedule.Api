using Microsoft.AspNetCore.Mvc;
using Planday.Schedule.Api.Dto;
using Planday.Schedule.Api.Models;
using Planday.Schedule.Models;
using Planday.Schedule.Services;

namespace Planday.Schedule.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ShiftController : ControllerBase
{
    private readonly IShiftService _shiftService;

    public ShiftController(IShiftService shiftService)
    {
        _shiftService = shiftService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<AssignedShiftDto>>> GetShift(int id)
    {
        var response = new Response<AssignedShiftDto>();

        try
        {
            var employeeShift = await _shiftService.ShiftById(id);

            response.Data = new AssignedShiftDto
            {
                Id = employeeShift.Id,
                EmployeeId = employeeShift.EmployeeId,
                End = employeeShift.End.ToString(),
                Start = employeeShift.Start.ToString(),
                EmployeeName = employeeShift.EmployeeName,
                EmployeeEmail = employeeShift.EmployeeEmail,
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;

            return BadRequest(response);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Response<ShiftDto>>> CreateShift(CreateShiftDto newShift)
    {
        var response = new Response<ShiftDto>();

        try
        {
            var shiftBase = new ShiftBase(newShift.EmployeeId, DateTime.Parse(newShift.Start), DateTime.Parse(newShift.End));

            var createdShift = await _shiftService.CreateShift(shiftBase);

            response.Data = new ShiftDto
            {
                Id = createdShift.Id,
                EmployeeId = createdShift.EmployeeId,
                End = createdShift.End.ToString(),
                Start = createdShift.Start.ToString(),
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;

            return BadRequest(response);
        }
    }

    [HttpPatch]
    public async Task<ActionResult<Response<ShiftDto>>> AssignShiftToEmployee(int employeeId, int shiftId)
    {
        var response = new Response<ShiftDto>();

        try
        {
            var assignedShift = await _shiftService.AssignShiftToEmployee(employeeId, shiftId);

            response.Data = new ShiftDto
            {
                Id = assignedShift.Id,
                EmployeeId = assignedShift.EmployeeId,
                End = assignedShift.End.ToString(),
                Start = assignedShift.Start.ToString(),
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;

            return BadRequest(response);
        }
    }
}

