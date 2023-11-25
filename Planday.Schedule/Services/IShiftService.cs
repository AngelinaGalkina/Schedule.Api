using Planday.Schedule.Models;

namespace Planday.Schedule.Services
{
    public interface IShiftService
    {
        Task<ShiftByIdServiceResponse<GetShiftDto>> ShiftById(int id);
        Task<ServiceResponse<GetShiftDto>> AddShift(CreateShiftDto newShift);
        Task<ServiceResponse<GetShiftDto>> AssignShiftToEmployee(long employeeId, long shiftId);
    }
}
