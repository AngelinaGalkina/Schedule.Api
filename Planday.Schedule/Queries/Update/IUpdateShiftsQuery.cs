using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planday.Schedule.Queries.Update
{
    public interface IUpdateShiftsQuery
    {
        Task<Shift?> UpdateEmployeeId(long shiftId, long newEmployeeId);
    }
}

