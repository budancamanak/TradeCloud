using Common.Core.Models;

namespace Common.Messaging.Events.PluginExecution;

public class PluginComputationEvent(int pluginId, int rowId,string key, double output, DateTime dateTime) : IntegrationEvent
{
    public int PluginId => pluginId;
    public string Key => key;
    public int RowId => rowId;
    public double Output => output;
    public DateTime DateTime => dateTime;

    public override string ToString()
    {
        return $"Plugin[{pluginId}] @ {dateTime} {key}({rowId}) : {output}";
    }
}