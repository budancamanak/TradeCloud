using Common.Core.Enums;
using Common.Security.Enums;

namespace Security.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Status Status { get; set; } = Status.Active;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public virtual ICollection<UserLogin> UserLogins { get; set; } = [];
    public virtual ICollection<Role> UserRoles { get; set; } = [];

    public override string ToString()
    {
        return $"Id:{Id}, Username:{Username}, Email:{Email}, Status:{Status}";
    }
}