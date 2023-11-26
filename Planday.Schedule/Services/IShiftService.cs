using Planday.Schedule.Models;
using Planday.Schedule.ResponseModels;

namespace Planday.Schedule.Services
{
    public interface IShiftService
    {
        Task<EmployeeShift> ShiftById(long id);
        Task<Shift> CreateShift(Shift newShift);
        Task<Shift> AssignShiftToEmployee(long employeeId, long shiftId);
    }
}
