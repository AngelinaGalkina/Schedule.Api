using Microsoft.AspNetCore.Mvc;
using Planday.Schedule.Api.Dto;
using Planday.Schedule.Api.Models;
using Planday.Schedule.Models;
using Planday.Schedule.Services;

namespace Planday.Schedule.Api.Controllers
{
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
        public async Task<ActionResult<Response<ShiftDto>>> GetShift(int id)
        {
            var response = await _shiftService.ShiftById(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Response<List<ShiftDto>>>> AddShift(Shift newShift)
        {
            var response = await _shiftService.AddShift(newShift);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPatch("{employeeId}-{shiftId}")]
        public async Task<ActionResult<Response<ShiftDto>>> AssignShiftToEmployee(int employeeId, int shiftId)
        {
            var response = await _shiftService.AssignShiftToEmployee(employeeId, shiftId);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}

