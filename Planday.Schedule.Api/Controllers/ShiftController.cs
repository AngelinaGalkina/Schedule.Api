using Microsoft.AspNetCore.Mvc;
using Planday.Schedule.Api.Dto;
using Planday.Schedule.Api.Models;
using Planday.Schedule.Api.Services;

namespace Planday.Schedule.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService shiftService;

        public ShiftController(IShiftService shiftService)
        {
            this.shiftService = shiftService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetShiftDto>>> GetShift(int id)
        {
            return Ok(await shiftService.GetShiftById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetShiftDto>>>> AddShift(CreateShiftDto newShift)
        {
            var response = await shiftService.AddShift(newShift);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPatch("{employeeId}-{shiftId}")]
        public async Task<ActionResult<ServiceResponse<GetShiftDto>>> AssignShiftToEmployee(int employeeId, int shiftId)
        {
            var response = await shiftService.AssignShiftToEmployee(employeeId, shiftId);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}

