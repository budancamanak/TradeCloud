namespace Common.Core.Models;

public abstract class IntegrationEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}