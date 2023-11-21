﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planday.Schedule.Queries
{
    public interface IUpdateShiftsQuery
    {
        Task<long?> AddShift(AddShiftDto shift);
        Task<Shift?> UpdateEmployeeId(long shiftId, long newEmployeeId);
    }    
}

