using Microsoft.Extensions.Logging;

namespace Common.Logging.Events.Backend;

public static class TrackListLogEvents
{
    public static readonly EventId AddTickerToTrackList = new EventId(1, "AddTickerToTrackListRequest");
    public static readonly EventId ListAvailableTickers = new EventId(2, "ListAvailableTickersRequest");
    public static readonly EventId ListUserTrackList = new EventId(3, "ListUserTrackListRequest");
    public static readonly EventId RemoveUserTrackList = new EventId(4, "RemoveUserTrackListRequest");
}