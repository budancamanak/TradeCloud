using Common.Core.Enums;
using Common.Core.Extensions;

namespace Common.Application.Services;

public static class CacheKeyGenerator
{
    // market
    public static string PluginKey(DateTime start, DateTime end, int tickerId, Timeframe timeframe) =>
        $"Prices:T{tickerId}.TF{timeframe.GetMilliseconds()}.S{start.TotalMilliseconds()}.E{end.TotalMilliseconds()}";

    public static string AvailableTickers() => $"Tickers:A";
    public static string TickerKey(int tickerId) => $"Tickers:T{tickerId}";
    public static string TickerKey(string symbol) => $"Tickers:TS{symbol}";
    public static string ExchangeKey(int exchangeId) => $"Exchanges:E{exchangeId}";

    // worker
    public static string AvailablePlugins() => $"AvailablePlugins";
    public static string AvailablePluginKey(string guid) => $"AvailablePlugins:P{guid}";
    public static string ActivePluginKey(int pluginId) => $"ActivePlugins:P{pluginId}";
    public static string ActiveAnalysisKey(int analysisId) => $"ActiveAnalysis:A{analysisId}";
    public static string ActivePluginParamsKey(int pluginId) => $"ActivePluginParams:P{pluginId}";
    public static string ActivePluginCountKey() => "ActivePluginCount";

    // backend
    public static string UserTrackingListKey(int id) => $"UserTrackList:TL{id}";
    public static string UserIdListKey() => "UserListId:List";
}