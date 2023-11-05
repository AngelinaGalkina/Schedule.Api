using Planday.Schedule.Api.Dto;
using Planday.Schedule.Api.Models;

namespace Planday.Schedule.Api.Services
{
    public interface IShiftService
    {
        Task<ServiceResponse<GetShiftDto>> GetShiftById(int id);
        Task<ServiceResponse<List<GetShiftDto>>> AddShift(GetShiftDto newShift);
    }
}
