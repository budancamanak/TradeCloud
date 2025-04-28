namespace Security.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<Permission> Permissions { get; set; }
    public virtual ICollection<User> Users { get; set; }
}