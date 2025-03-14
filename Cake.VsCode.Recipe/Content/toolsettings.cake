public static class ToolSettings
{
    static ToolSettings()
    {
        SetToolPreprocessorDirectives();
    }

    public static string GitReleaseManagerTool { get; private set; }
    public static string GitVersionTool { get; private set; }
    public static string KuduSyncTool { get; private set; }
    public static string WyamTool { get; private set; }

    public static string GitReleaseManagerGlobalTool { get; private set; }
    public static string GitVersionGlobalTool { get; private set; }
    public static string WyamGlobalTool { get; private set; }
    public static string KuduSyncGlobalTool { get; private set; }

    public static void SetToolPreprocessorDirectives(
        string gitReleaseManagerTool = "#tool nuget:?package=GitReleaseManager&version=0.19.0",
        // This is specifically pinned to 5.0.1 as later versions break compatibility with Unix.
        string gitVersionTool = "#tool nuget:?package=GitVersion.CommandLine&version=5.0.1",
        string kuduSyncTool = "#tool nuget:?package=KuduSync.NET&version=1.5.4",
        string wyamTool = "#tool nuget:?package=Wyam&version=2.2.9",
        string gitReleaseManagerGlobalTool = "#tool dotnet:?package=GitReleaseManager.Tool&version=0.19.0",
        string gitVersionGlobalTool = "#tool dotnet:?package=GitVersion.Tool&version=5.12.0",
        string wyamGlobalTool = "#tool dotnet:?package=Wyam.Tool&version=2.2.9",
        // This is using an unofficial build of kudusync so that we can have a .Net Global tool version.  This was generated from this PR: https://github.com/projectkudu/KuduSync.NET/pull/27
        string kuduSyncGlobalTool = "#tool dotnet:https://www.myget.org/F/cake-contrib/api/v3/index.json?package=KuduSync.Tool&version=1.5.4-g3916ad7218"
    )
    {
        GitReleaseManagerTool = gitReleaseManagerTool;
        GitVersionTool = gitVersionTool;
        KuduSyncTool = kuduSyncTool;
        WyamTool = wyamTool;
        GitVersionGlobalTool = gitVersionGlobalTool;
        GitReleaseManagerGlobalTool = gitReleaseManagerGlobalTool;
        WyamGlobalTool = wyamGlobalTool;
        KuduSyncGlobalTool = kuduSyncGlobalTool;
    }

    public static void SetToolSettings(
        ICakeContext context
    )
    {
        context.Information("Setting up tools...");
    }
}
