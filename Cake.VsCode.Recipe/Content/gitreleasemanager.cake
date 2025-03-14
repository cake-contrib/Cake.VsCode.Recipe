///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.CreateReleaseNotesTask = Task("Create-Release-Notes")
    .Does(() => RequireTool(BuildParameters.PreferDotNetGlobalToolUsage ? ToolSettings.GitReleaseManagerGlobalTool : ToolSettings.GitReleaseManagerTool, () => {
        if(BuildParameters.CanUseGitReleaseManager)
        {
            GitReleaseManagerCreate(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, new GitReleaseManagerCreateSettings {
                Milestone         = BuildParameters.Version.Milestone,
                Name              = BuildParameters.Version.Milestone,
                TargetCommitish   = BuildParameters.MasterBranchName,
                Prerelease        = false
            });
        }
        else
        {
            Warning("Unable to use GitReleaseManager, as necessary credentials are not available");
        }
    })
);

BuildParameters.Tasks.ExportReleaseNotesTask = Task("Export-Release-Notes")
    .WithCriteria(() => BuildParameters.ShouldDownloadMilestoneReleaseNotes || BuildParameters.ShouldDownloadFullReleaseNotes)
    .WithCriteria(() => !BuildParameters.IsLocalBuild || BuildParameters.PrepareLocalRelease)
    .WithCriteria(() => !BuildParameters.IsPullRequest || BuildParameters.PrepareLocalRelease)
    .WithCriteria(() => BuildParameters.IsMainRepository || BuildParameters.PrepareLocalRelease)
    .WithCriteria(() => BuildParameters.IsMasterBranch || BuildParameters.IsReleaseBranch || BuildParameters.IsHotFixBranch || BuildParameters.PrepareLocalRelease)
    .WithCriteria(() => BuildParameters.IsTagged || BuildParameters.PrepareLocalRelease)
    .Does(() => RequireTool(BuildParameters.PreferDotNetGlobalToolUsage ? ToolSettings.GitReleaseManagerGlobalTool : ToolSettings.GitReleaseManagerTool, () => {
        if(BuildParameters.CanUseGitReleaseManager)
        {
            if(BuildParameters.ShouldDownloadMilestoneReleaseNotes)
            {
                GitReleaseManagerExport(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.MilestoneReleaseNotesFilePath, new GitReleaseManagerExportSettings {
                    TagName         = BuildParameters.Version.Milestone
                });
            }

            if(BuildParameters.ShouldDownloadFullReleaseNotes)
            {
                GitReleaseManagerExport(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.FullReleaseNotesFilePath);
            }
        }
        else
        {
            Warning("Unable to use GitReleaseManager, as necessary credentials are not available");
        }
    })
);

BuildParameters.Tasks.PublishGitHubReleaseTask = Task("Publish-GitHub-Release")
    .IsDependentOn("Create-Chocolatey-Package")
    .WithCriteria(() => BuildParameters.ShouldPublishGitHub)
    .Does(() => RequireTool(BuildParameters.PreferDotNetGlobalToolUsage ? ToolSettings.GitReleaseManagerGlobalTool : ToolSettings.GitReleaseManagerTool, () => {
        if(BuildParameters.CanUseGitReleaseManager)
        {
            // Concatenating FilePathCollections should make sure we get unique FilePaths
            foreach(var package in GetFiles(BuildParameters.Paths.Directories.Packages + "/**/*") +
                                   GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/*") +
                                   GetFiles(BuildParameters.Paths.Directories.Build + "/*.vsix"))
            {
                GitReleaseManagerAddAssets(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.Version.Milestone, package.ToString());
            }

            GitReleaseManagerClose(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.Version.Milestone);
        }
        else
        {
            Warning("Unable to use GitReleaseManager, as necessary credentials are not available");
        }
    })
)
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-GitHub-Release Task failed, but continuing with next Task...");
    publishingError = true;
});

BuildParameters.Tasks.CreateDefaultLabelsTask = Task("Create-Default-Labels")
    .Does(() => RequireTool(BuildParameters.PreferDotNetGlobalToolUsage ? ToolSettings.GitReleaseManagerGlobalTool : ToolSettings.GitReleaseManagerTool, () => {
        if(BuildParameters.CanUseGitReleaseManager)
        {
            GitReleaseManagerLabel(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName);
        }
        else
        {
            Warning("Unable to use GitReleaseManager, as necessary credentials are not available");
        }
    })
);
