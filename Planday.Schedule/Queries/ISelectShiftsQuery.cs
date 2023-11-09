using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planday.Schedule.Queries
{
    public interface ISelectShiftsQuery
    {
        Task<IReadOnlyCollection<Shift>> AllShifts();
        Task<Shift> GetShiftById(long id);
    }    
}

