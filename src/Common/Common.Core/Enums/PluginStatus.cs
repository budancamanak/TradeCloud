namespace Common.Core.Enums;

/// <summary>
/// Hold the status of the plugin. Has a workflow as below <br />
/// <see cref="Init"/> -> <see cref="Queued"/> -> <see cref="Running"/> -> <see cref="WaitingData"/> -> <see cref="Failure"/>/<see cref="Success"/>
/// </summary>
public enum PluginStatus
{
    // waiting for start. not cached. exists on database
    Init = 0,

    // run requested.
    RunRequested = 1,

    // plugin in queue.
    Queued = 2,

    // plugin running now
    Running = 3,

    // plugin started. waiting for data to continue
    WaitingData = 4,

    // plugin failed
    Failure = 5,

    // plugin finished OK
    Success = 6
}

public static class PluginStatusExtensions
{
    public static PluginStatus ToPluginStatus(this int value)
    {
        return value switch
        {
            0 => PluginStatus.Init,
            1 => PluginStatus.RunRequested,
            2 => PluginStatus.Queued,
            3 => PluginStatus.Running,
            4 => PluginStatus.WaitingData,
            5 => PluginStatus.Failure,
            6 => PluginStatus.Success,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    public static PluginStatus ToPluginStatus(this string value)
    {
        return value switch
        {
            "Init" => PluginStatus.Init,
            "WaitingData" => PluginStatus.WaitingData,
            "Queued" => PluginStatus.Queued,
            "RunRequested" => PluginStatus.RunRequested,
            "Running" => PluginStatus.Running,
            "Failure" => PluginStatus.Failure,
            "Success" => PluginStatus.Success,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    public static string GetStringRepresentation(this PluginStatus status)
    {
        return status switch
        {
            PluginStatus.Init => "Init",
            PluginStatus.WaitingData => "WaitingData",
            PluginStatus.Queued => "Queued",
            PluginStatus.RunRequested => "RunRequested",
            PluginStatus.Running => "Running",
            PluginStatus.Failure => "Failure",
            PluginStatus.Success => "Success",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}