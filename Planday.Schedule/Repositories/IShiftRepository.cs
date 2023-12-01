using Planday.Schedule.Domain;
using Planday.Schedule.Models;

namespace Planday.Schedule.Repositories;

public interface IShiftRepository
{
    Task<long?> InsertShift(ShiftBase shift);
    Task<Shift?> ShiftById(long id);
    Task<IReadOnlyCollection<long?>> EmployeeByShiftId(long id);
    Task<IReadOnlyCollection<Shift>> OverlappingShifts(long employeeId, DateTime newStart, DateTime newEnd);
    Task<Shift?> UpdateEmployeeId(long shiftId, long newEmployeeId);
}
