namespace BackendWeb.API.Domain.Entities;

public class User
{
    public int IdUser { get; set; }
    public string EmailUser { get; set; } = string.Empty;
    public string PasswordUser { get; set; } = string.Empty;
    public string NameUser { get; set; } = string.Empty;
    public string LastnameUser { get; set; } = string.Empty;

    public Student? Student { get; set; }
    public Professor? Professor { get; set; }

}