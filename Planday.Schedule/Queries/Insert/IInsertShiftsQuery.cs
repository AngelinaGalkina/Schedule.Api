using System.Collections.Generic;
using System.Threading.Tasks;
using Planday.Schedule.Models;

namespace Planday.Schedule.Queries.Insert
{
    public interface IInsertShiftsQuery
    {
        Task<long?> InsertShift(Shift shift);
    }
}

