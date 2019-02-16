BuildParameters.Tasks.CreateChocolateyPackagesTask = Task("Create-Chocolatey-Packages")
    .IsDependentOn("Clean")
    .WithCriteria(() => BuildParameters.IsRunningOnWindows)
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.ChocolateyNuspecDirectory))
    .Does(() =>
{
    // TODO: Automatically update the release notes in the nuspec file
    // TODO: Automatically update the description from the Readme.md file

    // TODO: This should be made a parameter
    var nuspecFile = File("./chocolatey/" + BuildParameters.Title + ".nuspec");

    EnsureDirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages);
    var packageFile = MakeAbsolute((FilePath)(BuildParameters.Title + "-" + BuildParameters.Version.SemVersion + ".vsix"));
    CopyFile("LICENSE", "./chocolatey/tools/LICENSE.txt");
    var files = GetFiles("./chocolatey/tools/**/*").Select(f => new ChocolateyNuSpecContent {
                  Source = MakeAbsolute((FilePath)f).ToString(),
                  Target = "tools"
                }).ToList();
    files.Add(new ChocolateyNuSpecContent { Source = packageFile.ToString(), Target = "tools/" + BuildParameters.Title + ".vsix" });

    ChocolateyPack(nuspecFile, new ChocolateyPackSettings {
        Version = BuildParameters.Version.SemVersion,
        OutputDirectory = BuildParameters.Paths.Directories.ChocolateyPackages,
        WorkingDirectory = "./chocolatey",
        Files = files.ToArray()
    });
});

BuildParameters.Tasks.PublishChocolateyPackagesTask = Task("Publish-Chocolatey-Packages")
    .IsDependentOn("Package")
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
    Information("Publish-Chocolatey-Packages Task failed, but continuing with next Task...");
    publishingError = true;
});
