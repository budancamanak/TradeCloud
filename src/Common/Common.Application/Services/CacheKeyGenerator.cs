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
    public static string AvailablePluginParamsKey(string identifier) => $"AvailablePluginParams:P{identifier}";
    public static string ActivePluginCountKey() => "ActivePluginCount";

    // backend
    public static string UserTrackingListKey(int id) => $"UserTrackList:TL{id}";
    public static string UserIdListKey() => "UserListId:List";

    public static string UserRoleInfoKey(string userId) => $"UserRoleInfo:UI{userId}";
    public static string UserPermissionsKey(string userId) => $"UserPermissionsKey:UI{userId}";

    public static string UserTokenInfoKey(string token) => $"UserTokenInfoKey:UTI{token}";

    public static string PluginProgressEventConsumer(int pluginId) => $"PluginProgressConsumer:PId:{pluginId}";
    public static string AnalysisProgressEventConsumer(int analysisId) => $"AnalysisProgressConsumer:AId:{analysisId}";
}