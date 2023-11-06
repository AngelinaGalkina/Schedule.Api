using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planday.Schedule.Queries
{
    public interface IUpdateShiftsQuery
    {
        Task<bool> AddShift(Shift shift);
    }    
}

