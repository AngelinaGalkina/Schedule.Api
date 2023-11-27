namespace Planday.Schedule.Domain;

public class Employee
{
    public Employee(long id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public long Id { get; }
    public string Name { get; }
    public string Email { get; }
}

