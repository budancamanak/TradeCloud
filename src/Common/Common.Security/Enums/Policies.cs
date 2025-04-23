using Common.Core.Models;

namespace Common.Security.Enums;
// [Flags]
// public enum Permission
// {
//     ExecuteAll,
//     RunAnalysis
// }

public sealed class Policies : Enumeration<Policies>
{
    public static readonly Policies CanRunHighRiskAlgos = new Policies(1, nameof(CanRunHighRiskAlgos));
    public static readonly Policies CanExecuteLiveTrade = new Policies(2, nameof(CanExecuteLiveTrade));
    public static readonly Policies CanManageTeam = new Policies(3, nameof(CanManageTeam));
    public static readonly Policies CanAccessPremiumData = new Policies(4, nameof(CanAccessPremiumData));
    public static readonly Policies IsScriptOwner = new Policies(5, nameof(IsScriptOwner));
    public static readonly Policies CanAuditResults = new Policies(6, nameof(CanAuditResults));
    public static readonly Policies CanOwnTrackList = new Policies(7, nameof(CanOwnTrackList));

    private Policies(int value, string name) : base(value, name)
    {
    }
}

public static class Policiesx
{
    public static class TrackList
    {
        public const string FullAccess = Write + "." + Read;
        public const string Write = "Policy.Tracklist.ReadWrite";
        public const string Read = "Policy.Tracklist.ReadOnly";
    }

    public static class AnalysisExecution
    {
        public const string FullAccess = Write + "." + Read;
        public const string Write = "Policy.AnalysisExecution.ReadWrite";
        public const string Read = "Policy.AnalysisExecution.ReadOnly";
    }
}