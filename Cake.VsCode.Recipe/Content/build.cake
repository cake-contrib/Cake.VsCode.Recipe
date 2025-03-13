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

BuildParameters.Tasks.CleanTask = Task("Clean")
    .Does(() =>
{
    Information("Cleaning...");

    CleanDirectories(BuildParameters.Paths.Directories.ToClean);
});

BuildParameters.Tasks.NpmInstallTask = Task("Npm-Install")
    .Does(() =>
{
    if(BuildParameters.IsLocalBuild)
    {
        var settings = new NpmInstallSettings();
        settings.LogLevel = NpmLogLevel.Silent;
        NpmInstall(settings);
    }
    else
    {
        var settings = new NpmCiSettings();
        settings.LogLevel = NpmLogLevel.Silent;
        NpmCi(settings);
    }
});

BuildParameters.Tasks.InstallTypeScriptTask = Task("Install-TypeScript")
    .Does(() =>
{
    var settings = new NpmInstallSettings();
    settings.Global = true;
    settings.AddPackage("typescript", BuildParameters.TypeScriptVersionNumber);
    settings.LogLevel = NpmLogLevel.Silent;
    NpmInstall(settings);
});

BuildParameters.Tasks.InstallVsceTask = Task("Install-Vsce")
    .Does(() =>
{
    var settings = new NpmInstallSettings();
    settings.Global = true;
    settings.AddPackage("vsce", BuildParameters.VsceVersionNumber);
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
    .IsDependentOn("Clean")
    .IsDependentOn("Export-Release-Notes")
    .IsDependentOn("Update-Project-Json-Version")
    .IsDependentOn("Npm-Install")
    .IsDependentOn("Install-TypeScript")
    .IsDependentOn("Install-Vsce")
    .Does(() =>
{
    var buildResultDir = BuildParameters.Paths.Directories.Build;
    var packageFile = new FilePath(BuildParameters.Title + "-" + BuildParameters.Version.SemVersion + ".vsix");
    var outputFilePath = buildResultDir.CombineWithFilePath(packageFile);

    VscePackage(new VscePackageSettings() {
        OutputFilePath = outputFilePath
    });

    if (FileExists(outputFilePath)) {
        BuildParameters.BuildProvider.UploadArtifact(outputFilePath);
    }
});

BuildParameters.Tasks.PublishExtensionTask = Task("Publish-Extension")
    .IsDependentOn("Package-Extension")
    .WithCriteria(() => BuildParameters.ShouldPublishExtension)
    .Does(() =>
{
    var buildResultDir = BuildParameters.Paths.Directories.Build;
    var packageFile = new FilePath(BuildParameters.Title + "-" + BuildParameters.Version.SemVersion + ".vsix");

    VscePublish(new VscePublishSettings(){
        PersonalAccessToken = BuildParameters.Marketplace.Token,
        Package = buildResultDir.CombineWithFilePath(packageFile)
    });
})
.OnError(exception =>
{
    Information("Publish-Extension Task failed, but continuing with next Task...");
    publishingError = true;
});

BuildParameters.Tasks.DefaultTask = Task("Default")
    .IsDependentOn("Create-Chocolatey-Package");

BuildParameters.Tasks.AppVeyorTask = Task("AppVeyor")
    .IsDependentOn("Create-Chocolatey-Package")
    .IsDependentOn("Publish-GitHub-Release")
    .IsDependentOn("Publish-Extension")
    .IsDependentOn("Publish-Chocolatey-Package")
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
