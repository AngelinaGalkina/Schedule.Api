using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planday.Schedule.Infrastructure.Dto;

// TODO consider record, and namespace without scope braces
public class ShiftDto
{
    public long Id { get; set; }
    public long? EmployeeId { get; set; }
    public string Start { get; }
    public string End { get; }
}
