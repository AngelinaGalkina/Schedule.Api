﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planday.Schedule.Queries.Select
{
    public interface ISelectShiftsQuery
    {
        Task<IReadOnlyCollection<Shift>> AllShifts();
        Task<Shift> ShiftById(long? id);
        Task<IReadOnlyCollection<long?>> GetEmployeeByShiftId(long? id);
        Task<IReadOnlyCollection<Shift>> OverlappingShifts(long? employeeId, DateTime newStart, DateTime newEnd);
    }
}
