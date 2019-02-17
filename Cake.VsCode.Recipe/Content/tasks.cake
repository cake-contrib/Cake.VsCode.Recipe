public class BuildTasks
{
    public CakeTaskBuilder PrintAppVeyorEnvironmentVariablesTask { get; set; }
    public CakeTaskBuilder UploadAppVeyorArtifactsTask { get; set; }
    public CakeTaskBuilder ClearAppVeyorCacheTask { get; set; }
    public CakeTaskBuilder ShowInfoTask { get; set; }
    public CakeTaskBuilder CleanTask { get; set; }
    public CakeTaskBuilder DefaultTask { get; set; }
    public CakeTaskBuilder AppVeyorTask { get; set; }
    public CakeTaskBuilder ReleaseNotesTask { get; set; }
    public CakeTaskBuilder LabelsTask { get; set; }
    public CakeTaskBuilder ClearCacheTask { get; set; }
    public CakeTaskBuilder PreviewTask { get; set; }
    public CakeTaskBuilder PublishDocsTask { get; set; }
    public CakeTaskBuilder CreateChocolateyPackageTask { get; set; }
    public CakeTaskBuilder PublishChocolateyPackageTask { get; set; }
    public CakeTaskBuilder CreateReleaseNotesTask { get; set; }
    public CakeTaskBuilder ExportReleaseNotesTask { get; set; }
    public CakeTaskBuilder PublishGitHubReleaseTask { get; set; }
    public CakeTaskBuilder CreateDefaultLabelsTask { get; set; }
    public CakeTaskBuilder CleanDocumentationTask { get; set; }
    public CakeTaskBuilder DeployGraphDocumentation {get; set;}
    public CakeTaskBuilder PublishDocumentationTask { get; set; }
    public CakeTaskBuilder PreviewDocumentationTask { get; set; }
    public CakeTaskBuilder ForcePublishDocumentationTask { get; set; }
    public CakeTaskBuilder NpmInstallTask { get; set; }
    public CakeTaskBuilder InstallTypeScriptTask { get; set; }
    public CakeTaskBuilder InstallVsceTask { get; set; }
    public CakeTaskBuilder UpdateProjectJsonVersionTask { get; set; }
    public CakeTaskBuilder PackageExtensionTask { get; set; }
    public CakeTaskBuilder PublishExtensionTask { get; set; }
}
