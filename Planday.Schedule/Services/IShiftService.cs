using Planday.Schedule.Models;
using Planday.Schedule.ResponseModels;

namespace Planday.Schedule.Services
{
    public interface IShiftService
    {
        Task<ShiftByIdServiceResponse<Shift>> ShiftById(int id);
        Task<ServiceResponse<Shift>> AddShift(Shift newShift);
        Task<ServiceResponse<Shift>> AssignShiftToEmployee(long employeeId, long shiftId);
    }
}
