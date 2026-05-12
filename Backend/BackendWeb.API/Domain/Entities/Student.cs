namespace BackendWeb.API.Domain.Entities;

public class Student
{
    public int IdStudent { get; set; }
    public int IdUser { get; set; }

    public User User { get; set; } = null!;
}