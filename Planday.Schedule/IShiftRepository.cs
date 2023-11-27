using Planday.Schedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planday.Schedule
{
    public interface IShiftRepository
    {
        Task<long?> InsertShift(ShiftBase shift);
        Task<Shift?> ShiftById(long id);
        Task<IReadOnlyCollection<long?>> EmployeeByShiftId(long id);
        Task<IReadOnlyCollection<Shift>> OverlappingShifts(long employeeId, DateTime newStart, DateTime newEnd);
        Task<Shift?> UpdateEmployeeId(long shiftId, long newEmployeeId);
    }
}
