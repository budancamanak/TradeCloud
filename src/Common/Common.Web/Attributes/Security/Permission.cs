namespace Common.Web.Attributes.Security;

[Flags]
public enum Permission
{
    ExecuteAll,
    RunAnalysis
}

public static class Policies
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