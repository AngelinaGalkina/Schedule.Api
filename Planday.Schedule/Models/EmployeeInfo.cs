namespace Planday.Schedule.Models;

public class EmployeeInfo
{
    public EmployeeInfo(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public string Name { get; }
    public string Email { get; }
}
