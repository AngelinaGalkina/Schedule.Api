using Planday.Schedule.Models;

namespace Planday.Schedule.Services;

public interface IShiftService
{
    Task<EmployeeShift> ShiftById(long id);
    Task<Shift> CreateShift(ShiftBase newShift);
    Task<Shift> AssignShiftToEmployee(long employeeId, long shiftId);
}
