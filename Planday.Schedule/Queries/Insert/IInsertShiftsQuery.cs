using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planday.Schedule.Queries.Insert
{
    public interface IInsertShiftsQuery
    {
        Task<long?> InsertShift(AddShiftDto shift);
    }
}

