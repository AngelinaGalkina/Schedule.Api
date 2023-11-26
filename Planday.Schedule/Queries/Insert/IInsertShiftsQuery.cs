using Planday.Schedule.Models;

namespace Planday.Schedule.Queries.Insert;

public interface IInsertShiftsQuery
{
    Task<long?> InsertShift(ShiftBase shift);
}

