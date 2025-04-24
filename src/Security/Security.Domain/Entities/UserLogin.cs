namespace Security.Domain.Entities;

public class UserLogin
{
    public string Token { get; set; }
    public int UserId { get; set; }
    public DateTime LoginDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string ClientIP { get; set; }
    public string UserAgent { get; set; }
}