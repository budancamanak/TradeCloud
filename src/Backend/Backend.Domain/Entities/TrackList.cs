namespace Backend.Domain.Entities;

public class TrackList
{
    public int UserId { get; set; }
    public int TickerId { get; set; }

    public override string ToString()
    {
        return $"U:{UserId}-T{TickerId}";
    }
}