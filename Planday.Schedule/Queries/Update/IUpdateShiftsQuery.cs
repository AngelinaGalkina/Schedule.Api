using Planday.Schedule.Models;

namespace Planday.Schedule.Queries.Update;

public interface IUpdateShiftsQuery
{
    Task<Shift?> UpdateEmployeeId(long shiftId, long newEmployeeId);
}

