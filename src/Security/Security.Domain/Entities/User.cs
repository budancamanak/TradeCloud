using Common.Core.Enums;
using Common.Security.Enums;

namespace Security.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Identity { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Status Status { get; set; } = Status.Active;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}