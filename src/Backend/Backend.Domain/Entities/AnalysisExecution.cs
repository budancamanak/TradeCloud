using Common.Core.Enums;

namespace Backend.Domain.Entities;

/***
 * This class will be parent for PluginExecution
 * This will be created by user input
 * Paramset might contain range inputs
 * Backend will create required PluginExecutions
 * Backend will create params for each plugin execution using paramset of this class
 * All duplicate fields between this and pluginexecution should be cleared.
 * Common fields(duplicates) should be cached from this class
 * PluginExecution should retrieve them from cache.
 * User will give this id to start plugin
 *
 *
 * Workflow -> user will provide values to create analysis execution
 *          -> backend will generate cartesian product of plugin parameters
 *          -> backend will create plugin executions for each plugin parameters
 *          -> user will provide analysis id to run
 *          -> backend will fetch for waiting plugins of analysis
 *          -> backend will cache required fields for plugins to run
 *          -> backend will trigger worker to start each waiting plugin
 *
 *          -> worker will fetch fields and data from cache to start plugin
 *          -> worker will start the plugin
 *          -> each output will be registered with plugin execution id
 *          -> when a plugin finishes, worker will trigger backend to update analysis status(progress)
 *          -> analysis progress is : activeCount(# running plugin)/totalCount(# of cartesian product)
 *
 *
 *          -> when all plugins finished for an analysis, backend will trigger notification about status
 *
 * # Remarks
 *      -> Calculate & store plugin execution times.
 *      -> Might need to have a summary table to store summary of analysis
 *      -> Might display summary as first output to user(on UI)
 *      -> When detailed results are requested: PluginId should be provided
 *      -> PluginOutputs can be displayed to user in a tabbed view or smth
 *      -> When viewing outputs on a chart, UI can switch between plugins.
 *
 *      -> Try not to recompute anything for the same values.
 *         For example: GoldenDeathCross (50-55, 190-195) -> 25 different plugins to run.
 *              50,190 | 51,190 | 52,190 | 53,190 | 54,190 | 55,190
 *              50,191 | 51,191 | 52,191 | 53,191 | 54,191 | 55,191
 *              50,192 | 51,192 | 52,192 | 53,192 | 54,192 | 55,192
 *              50,193 | 51,193 | 52,193 | 53,193 | 54,193 | 55,193
 *              50,194 | 51,194 | 52,194 | 53,194 | 54,194 | 55,194
 *              50,195 | 51,195 | 52,195 | 53,195 | 54,195 | 55,195
 *          When 50SMA computed, cache it. Next plugin that wants 50SMA will use it from cache
 *          When 190SMA computed, cache it.
 *
 *      # Put a cache ability to BasePlugin.
 *          1: Remap all functions of TradingLibrary and store within the maps -> automatic
 *          2: Give users to cache individually.
 *          3: Create a taLibCompute method that takes an action of TradingLibrary method call.
 *              Execute the action in taLibCompute method. Cache output.
 *              if that action was already cached, use from the cache.
 *              Eg: var sma21 = quotes.GetSma(21).ToList();
 *              Eg: var sma21 = taLibCompute(()=>quotes.GetSma(21));
 *                              will check if quotes.GetSma(21) exists on cache. returns it if so.
 *                              will run quotes.GetSma(21). cache it. returns it.
 */

public class AnalysisExecution
{
    public int Id { get; set; }
    public string PluginIdentifier { get; set; }
    public int TickerId { get; set; }
    public Timeframe Timeframe { get; set; }
    public double Progress { get; set; } = 0;
    public string ParamSet { get; set; }
    public string TradingParams { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public PluginStatus Status
    {
        get
        {
            if (PluginExecutions.Count == 0) return PluginStatus.Init;
            if (PluginExecutions.All(f => f.Status == PluginStatus.Init)) return PluginStatus.Init;
            if (PluginExecutions.All(f => f.Status == PluginStatus.Success)) return PluginStatus.Success;
            if (PluginExecutions.All(f => f.Status == PluginStatus.Failure)) return PluginStatus.Failure;
            return PluginStatus.Running;
        }
    }

    public virtual ICollection<PluginExecution> PluginExecutions { get; set; } = [];

    public override string ToString()
    {
        return $"[{Id}] TickerId:{TickerId} {Timeframe} [{PluginIdentifier}]";
    }
}