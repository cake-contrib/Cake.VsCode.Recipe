BuildParameters.Tasks.CreateChocolateyPackageTask = Task("Create-Chocolatey-Package")
    .IsDependentOn("Package-Extension")
    .WithCriteria(() => BuildParameters.IsRunningOnWindows)
    .Does(() =>
{
    var nuspecFile = File("./" + BuildParameters.ChocolateyPackagingFolderName + "/" + BuildParameters.Title + ".nuspec");

    if (FileExists(nuspecFile))
    {
        // TODO: Automatically update the description from the Readme.md file
        var releaseNotes = new string[0];

        if (BuildParameters.ShouldDownloadMilestoneReleaseNotes && FileExists(BuildParameters.MilestoneReleaseNotesFilePath)) {
            // Not completely sure if the correct directory separators will be used.
            releaseNotes = System.IO.File.ReadAllLines(BuildParameters.MilestoneReleaseNotesFilePath.FullPath, System.Text.Encoding.UTF8);
        }
        else if (BuildParameters.ShouldDownloadFullReleaseNotes && FileExists(BuildParameters.FullReleaseNotesFilePath)) {
            releaseNotes = System.IO.File.ReadAllLines(BuildParameters.FullReleaseNotesFilePath.FullPath, System.Text.Encoding.UTF8);
        }

        EnsureDirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages);
        var buildResultDir = BuildParameters.Paths.Directories.Build;
        var packageFile = new FilePath(BuildParameters.Title + "-" + BuildParameters.Version.SemVersion + ".vsix");

        CopyFile("LICENSE", "./" + BuildParameters.ChocolateyPackagingFolderName + "/tools/LICENSE.txt");
        var files = GetFiles("./" + BuildParameters.ChocolateyPackagingFolderName + "/tools/**/*").Select(f => new ChocolateyNuSpecContent {
                    Source = MakeAbsolute((FilePath)f).ToString(),
                    Target = "tools"
                    }).ToList();
        files.Add(new ChocolateyNuSpecContent { Source = MakeAbsolute(buildResultDir.CombineWithFilePath(packageFile)).ToString(), Target = "tools/" + BuildParameters.Title + ".vsix" });

        var settings = new ChocolateyPackSettings {
            Version = BuildParameters.Version.SemVersion,
            OutputDirectory = BuildParameters.Paths.Directories.ChocolateyPackages,
            WorkingDirectory = "./" + BuildParameters.ChocolateyPackagingFolderName,
            Files = files.ToArray()
        };

        if (releaseNotes.Length > 0) {
            settings.ReleaseNotes = releaseNotes;
        }

        ChocolateyPack(nuspecFile, settings);
    }
    else
    {
        Warning("No Chocolatey nuspec file exists, so no Chocolatey package will be created");
    }
});

BuildParameters.Tasks.PublishChocolateyPackageTask = Task("Publish-Chocolatey-Package")
    .IsDependentOn("Create-Chocolatey-Package")
    .WithCriteria(() => BuildParameters.IsRunningOnWindows)
    .WithCriteria(() => BuildParameters.ShouldPublishChocolatey)
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages))
    .Does(() =>
{
    if(BuildParameters.CanPublishToChocolatey)
    {
        var nupkgFiles = GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/**/*.nupkg");

        foreach(var nupkgFile in nupkgFiles)
        {
            // Push the package.
            ChocolateyPush(nupkgFile, new ChocolateyPushSettings {
            ApiKey = BuildParameters.Chocolatey.ApiKey,
            Source = BuildParameters.Chocolatey.SourceUrl
            });
        }
    }
    else
    {
        Warning("Unable to publish to Chocolatey, as necessary credentials are not available");
    }
})
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-Chocolatey-Package Task failed, but continuing with next Task...");
    publishingError = true;
});
