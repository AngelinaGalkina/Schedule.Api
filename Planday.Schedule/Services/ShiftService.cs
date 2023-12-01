using Planday.Schedule.Domain;
using Planday.Schedule.Models;
using Planday.Schedule.Repositories;
using Planday.Schedule.Validators;

namespace Planday.Schedule.Services;

public class ShiftService : IShiftService
{
    private readonly IShiftRepository _shiftRepository;

    private readonly IEmployeeRepository _employeeRepository;

    private readonly ICreateShiftValidator _createShiftValidations;
    
    private readonly IAssignShiftToEmployeeValidator _assignShiftToEmployeeValidations;

    public ShiftService(IShiftRepository shiftRepository, 
        IEmployeeRepository employeeRepository, 
        ICreateShiftValidator createShiftValidations,
        IAssignShiftToEmployeeValidator assignShiftToEmployeeValidations)
    {
        _shiftRepository = shiftRepository;
        _employeeRepository = employeeRepository;
        _createShiftValidations = createShiftValidations;
        _assignShiftToEmployeeValidations = assignShiftToEmployeeValidations;
    }

    public async Task<EmployeeShift> ShiftById(long id)
    {
        var shift = await _shiftRepository.ShiftById(id);

        if (shift == null)
        {
            throw new Exception($"Couldn't find shift with id: {id}");
        }

        var shiftWithEmployeeInfo = ShiftWithEmployeeInfo(shift);

        return shiftWithEmployeeInfo ?? new EmployeeShift(shift.Id, shift.Employee, shift.Start, shift.End);
    }

    public async Task<Shift> CreateShift(ShiftBase newShift)
    {
        _createShiftValidations.Validate(newShift);

        var newShiftId = await _shiftRepository.InsertShift(newShift);

        if (newShiftId == null)
        {
            throw new Exception("Couldn't insert shift");
        }

        // casting to long since value will never be null.
        var createdShift = await _shiftRepository.ShiftById((long)newShiftId);

        if (createdShift == null)
        {
            throw new Exception($"Couldn't find created shift with id: {newShiftId}");
        }

        return createdShift;
    }

    public async Task<Shift> AssignShiftToEmployee(long employeeId, long shiftId)
    {
        var shift = await _shiftRepository.ShiftById(shiftId);

        if (shift == null)
        {
            throw new Exception($"Couldn't find shift with id: {shiftId}");
        }

        var employee = await _employeeRepository.EmployeeById(employeeId);

        if (employee == null)
        {
            throw new Exception($"Couldn't find employee with id: {employeeId}");
        }

        await _assignShiftToEmployeeValidations.ValidateAsync(shift, employeeId, shiftId);

        var updatedShift = await _shiftRepository.UpdateEmployeeId(shiftId, employeeId);

        if (updatedShift == null)
        {
            throw new Exception($"Couldn't update employee with id: {employeeId}");
        }

        return updatedShift;
    }

    private EmployeeShift? ShiftWithEmployeeInfo(Shift shift)
    {
        if (shift.Employee.Id != null)
        {
            var employeeInfo = _employeeRepository.EmployeeEmail((long)shift.Employee.Id);

            if (employeeInfo != null)
            {
                return new EmployeeShift(shift.Id, shift.Employee, shift.Start, shift.End)
                {
                    EmployeeName = employeeInfo.Name,
                    EmployeeEmail = employeeInfo.Email
                };
            }
        }

        return null;
    }
}