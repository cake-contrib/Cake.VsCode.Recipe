///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var publishingError = false;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    Information(Figlet(BuildParameters.Title));

    Information("Starting Setup...");

    if(BuildParameters.IsMasterBranch && (context.Log.Verbosity != Verbosity.Diagnostic)) {
        Information("Increasing verbosity to diagnostic.");
        context.Log.Verbosity = Verbosity.Diagnostic;
    }

    RequireTool(GitVersionTool, () => {
        BuildParameters.SetBuildVersion(
            BuildVersion.CalculatingSemanticVersion(
                context: Context
            )
        );
    });

    Information("Building version {0} of " + BuildParameters.Title + " ({1}, {2}) using version {3} of Cake, and version {4} of Cake.VsCode.Recipe. (IsTagged: {5})",
        BuildParameters.Version.SemVersion,
        BuildParameters.Configuration,
        BuildParameters.Target,
        BuildParameters.Version.CakeVersion,
        BuildMetaData.Version,
        BuildParameters.IsTagged);
});

Teardown(context =>
{
    Information("Starting Teardown...");

    if(context.Successful)
    {
        if(!BuildParameters.IsLocalBuild && !BuildParameters.IsPullRequest && BuildParameters.IsMainRepository && (BuildParameters.IsMasterBranch || ((BuildParameters.IsReleaseBranch || BuildParameters.IsHotFixBranch) && BuildParameters.ShouldNotifyBetaReleases)) && BuildParameters.IsTagged)
        {
            if(BuildParameters.CanPostToTwitter && BuildParameters.ShouldPostToTwitter)
            {
                SendMessageToTwitter();
            }

            if(BuildParameters.CanPostToGitter && BuildParameters.ShouldPostToGitter)
            {
                SendMessageToGitterRoom();
            }

            if(BuildParameters.CanPostToMicrosoftTeams && BuildParameters.ShouldPostToMicrosoftTeams)
            {
                SendMessageToMicrosoftTeams();
            }
        }
    }
    else
    {
        if(!BuildParameters.IsLocalBuild && BuildParameters.IsMainRepository)
        {
            if(BuildParameters.CanPostToSlack && BuildParameters.ShouldPostToSlack)
            {
                SendMessageToSlackChannel("Continuous Integration Build of " + BuildParameters.Title + " just failed :-(");
            }
        }
    }

    // Clear nupkg files from tools directory
    if(DirectoryExists(Context.Environment.WorkingDirectory.Combine("tools")))
    {
        Information("Deleting nupkg files...");
        var nupkgFiles = GetFiles(Context.Environment.WorkingDirectory.Combine("tools") + "/**/*.nupkg");
        DeleteFiles(nupkgFiles);
    }

    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.ShowInfoTask = Task("Show-Info")
    .Does(() =>
{
    Information("Target: {0}", BuildParameters.Target);
    Information("Configuration: {0}", BuildParameters.Configuration);
    Information("PrepareLocalRelease: {0}", BuildParameters.PrepareLocalRelease);
    Information("ShouldDownloadMilestoneReleaseNotes: {0}", BuildParameters.ShouldDownloadMilestoneReleaseNotes);
    Information("ShouldDownloadFullReleaseNotes: {0}", BuildParameters.ShouldDownloadFullReleaseNotes);
    Information("IsLocalBuild: {0}", BuildParameters.IsLocalBuild);
    Information("IsPullRequest: {0}", BuildParameters.IsPullRequest);
    Information("IsMainRepository: {0}", BuildParameters.IsMainRepository);
    Information("IsMasterBranch: {0}", BuildParameters.IsMasterBranch);
    Information("IsReleaseBranch: {0}", BuildParameters.IsReleaseBranch);
    Information("IsHotFixBranch: {0}", BuildParameters.IsHotFixBranch);
    Information("IsTagged: {0}", BuildParameters.IsTagged);

    Information("Build DirectoryPath: {0}", MakeAbsolute(BuildParameters.Paths.Directories.Build));
});

BuildParameters.Tasks.CleanTask = Task("Clean")
    .IsDependentOn("Show-Info")
    .IsDependentOn("Print-AppVeyor-Environment-Variables")
    .Does(() =>
{
    Information("Cleaning...");

    CleanDirectories(BuildParameters.Paths.Directories.ToClean);
});

BuildParameters.Tasks.NpmInstallTask = Task("Npm-Install")
    .Does(() =>
{
    var settings = new NpmInstallSettings();
    settings.LogLevel = NpmLogLevel.Silent;
    NpmInstall(settings);
});

BuildParameters.Tasks.InstallTypeScriptTask = Task("Install-TypeScript")
    .Does(() =>
{
    var settings = new NpmInstallSettings();
    settings.Global = true;
    // TODO: This should likely become a parameter
    settings.AddPackage("typescript", "2.9.2");
    settings.LogLevel = NpmLogLevel.Silent;
    NpmInstall(settings);
});

BuildParameters.Tasks.InstallVsceTask = Task("Install-Vsce")
    .Does(() =>
{
    var settings = new NpmInstallSettings();
    settings.Global = true;
    // TODO: This should likely become a parameter
    settings.AddPackage("vsce", "1.43.0");
    settings.LogLevel = NpmLogLevel.Silent;
    NpmInstall(settings);
});

BuildParameters.Tasks.UpdateProjectJsonVersionTask = Task("Update-Project-Json-Version")
    .WithCriteria(() => !BuildParameters.IsLocalBuild)
    .Does(() =>
{
    var projectToPackagePackageJson = "package.json";
    Information("Updating {0} version -> {1}", projectToPackagePackageJson, BuildParameters.Version.SemVersion);

    TransformConfig(projectToPackagePackageJson, projectToPackagePackageJson, new TransformationCollection {
        { "version", BuildParameters.Version.SemVersion }
    });
});

BuildParameters.Tasks.PackageExtensionTask = Task("Package-Extension")
    .IsDependentOn("Export-Release-Notes")
    .IsDependentOn("Update-Project-Json-Version")
    .IsDependentOn("Npm-Install")
    .IsDependentOn("Install-TypeScript")
    .IsDependentOn("Install-Vsce")
    .IsDependentOn("Clean")
{
    var buildResultDir = MakeAbsolute(BuildParameters.Paths.Directories.Build);
    var packageFile = File(BuildParameters.Title + "-" + BuildParameters.Version.SemVersion + ".vsix");

    VscePackage(new VscePackageSettings() {
        OutputFilePath = buildResultDir + packageFile
    });
})

BuildParameters.Tasks.PublishExtensionTask = Task("Publish-Extension")
    .IsDependentOn("Package-Extension")
    .WithCriteria(() => BuildParameters.ShouldPublishExtension)
    .Does(() =>
{
    var buildResultDir = MakeAbsolute(BuildParameters.Paths.Directories.Build);
    var packageFile = File(BuildParameters.Title + "-" + BuildParameters.Version.SemVersion + ".vsix");

    VscePublish(new VscePublishSettings(){
        PersonalAccessToken = BuildParameters.Marketplace.Token,
        Package = buildResultDir + packageFile
    });
})
.OnError(exception =>
{
    Information("Publish-Extension Task failed, but continuing with next Task...");
    publishingError = true;
});

BuildParameters.Tasks.DefaultTask = Task("Default")
    .IsDependentOn("Package-Extension");

BuildParameters.Tasks.AppVeyorTask = Task("AppVeyor")
    .IsDependentOn("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Publish-GitHub-Release")
    .IsDependentOn("Publish-Extension")
    .IsDependentOn("Publish-Chocolatey-Packages")
    .IsDependentOn("Publish-Documentation")
    .Finally(() =>
{
    if(publishingError)
    {
        throw new Exception("An error occurred during the publishing of " + BuildParameters.Title + ".  All publishing tasks have been attempted.");
    }
});

BuildParameters.Tasks.ReleaseNotesTask = Task("ReleaseNotes")
  .IsDependentOn("Create-Release-Notes");

BuildParameters.Tasks.LabelsTask = Task("Labels")
  .IsDependentOn("Create-Default-Labels");

BuildParameters.Tasks.ClearCacheTask = Task("ClearCache")
  .IsDependentOn("Clear-AppVeyor-Cache");

BuildParameters.Tasks.PreviewTask = Task("Preview")
  .IsDependentOn("Preview-Documentation");

BuildParameters.Tasks.PublishDocsTask = Task("PublishDocs")
    .IsDependentOn("Force-Publish-Documentation");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

public Builder Build
{
    get
    {
        return new Builder(target => RunTarget(target));
    }
}

public class Builder
{
    private Action<string> _action;

    public Builder(Action<string> action)
    {
        _action = action;
    }

    public void Run()
    {
        _action(BuildParameters.Target);
    }
}
