namespace BackendWeb.API.Domain.Entities;

public class Professor
{
    public int IdProfessor { get; set; }
    public int IdUser { get; set; }

    public User User { get; set; } = null!;
}