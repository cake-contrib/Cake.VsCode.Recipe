public class BuildVersion
{
    public string Version { get; private set; }
    public string SemVersion { get; private set; }
    public string Milestone { get; private set; }
    public string CakeVersion { get; private set; }
    public string InformationalVersion { get; private set; }
    public string FullSemVersion { get; private set; }

    public static BuildVersion CalculatingSemanticVersion(
        ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        string version = null;
        string semVersion = null;
        string milestone = null;
        string informationalVersion = null;
        string fullSemVersion = null;
        GitVersion assertedVersions = null;

        if (BuildParameters.ShouldRunGitVersion)
        {
            context.Information("Calculating Semantic Version...");
            if (!BuildParameters.IsLocalBuild || BuildParameters.IsPublishBuild || BuildParameters.IsReleaseBuild || BuildParameters.PrepareLocalRelease)
            {
                if(!BuildParameters.IsPublicRepository && BuildParameters.IsRunningOnAppVeyor)
                {
                    context.GitVersion(new GitVersionSettings{
                        OutputType = GitVersionOutput.BuildServer,
                        NoFetch = true
                    });
                } else {
                    context.GitVersion(new GitVersionSettings{
                        OutputType = GitVersionOutput.BuildServer
                    });
                }

                version = context.EnvironmentVariable("GitVersion_MajorMinorPatch");
                semVersion = context.EnvironmentVariable("GitVersion_LegacySemVerPadded");
                informationalVersion = context.EnvironmentVariable("GitVersion_InformationalVersion");
                milestone = string.Concat(version);
            }

            if(!BuildParameters.IsPublicRepository && BuildParameters.IsRunningOnAppVeyor)
            {
                assertedVersions = context.GitVersion(new GitVersionSettings{
                        OutputType = GitVersionOutput.Json,
                        NoFetch = true
                });
            } else {
                assertedVersions = context.GitVersion(new GitVersionSettings{
                        OutputType = GitVersionOutput.Json,
                });
            }

            version = assertedVersions.MajorMinorPatch;
            semVersion = assertedVersions.LegacySemVerPadded;
            informationalVersion = assertedVersions.InformationalVersion;
            milestone = string.Concat(version);
            fullSemVersion = assertedVersions.FullSemVer;

            context.Information("Calculated Semantic Version: {0}", semVersion);
        }

        var cakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString();

        return new BuildVersion
        {
            Version = version,
            SemVersion = semVersion,
            Milestone = milestone,
            CakeVersion = cakeVersion,
            InformationalVersion = informationalVersion,
            FullSemVersion = fullSemVersion
        };
    }
}
