using System.Collections.Generic;
using System.Threading.Tasks;
using Planday.Schedule.Models;

namespace Planday.Schedule.Queries.Select;

public interface ISelectShiftsQuery
{
    Task<Shift?> ShiftById(long id);
    Task<IReadOnlyCollection<long?>> EmployeeByShiftId(long id);
    Task<IReadOnlyCollection<Shift>> OverlappingShifts(long employeeId, DateTime newStart, DateTime newEnd);
}

