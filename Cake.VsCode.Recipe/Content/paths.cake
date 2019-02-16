public class BuildPaths
{
    public BuildFiles Files { get; private set; }
    public BuildDirectories Directories { get; private set; }

    public static BuildPaths GetPaths(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        // Directories
        var buildDirectoryPath             = "./BuildArtifacts";
        var tempBuildDirectoryPath         = buildDirectoryPath + "/temp";
        var publishedDocumentationDirectory= buildDirectoryPath + "/Documentation";

        var chocolateyNuspecDirectory = "./nuspec/chocolatey";

        var packagesDirectory = buildDirectoryPath + "/Packages";
        var chocolateyPackagesOutputDirectory = packagesDirectory + "/Chocolatey";

        // Files
        var repoFilesPaths = new FilePath[] {
            "LICENSE",
            "README.md"
        };

        var buildDirectories = new BuildDirectories(
            buildDirectoryPath,
            tempBuildDirectoryPath,
            publishedDocumentationDirectory,
            chocolateyNuspecDirectory,
            chocolateyPackagesOutputDirectory,
            packagesDirectory
            );

        var buildFiles = new BuildFiles(
            context,
            repoFilesPaths,
            );

        return new BuildPaths
        {
            Files = buildFiles,
            Directories = buildDirectories
        };
    }
}

public class BuildFiles
{
    public ICollection<FilePath> RepoFilesPaths { get; private set; }

    public BuildFiles(
        ICakeContext context,
        FilePath[] repoFilesPaths
        )
    {
        RepoFilesPaths = Filter(context, repoFilesPaths);
    }

    private static FilePath[] Filter(ICakeContext context, FilePath[] files)
    {
        // Not a perfect solution, but we need to filter PDB files
        // when building on an OS that's not Windows (since they don't exist there).

        if(!context.IsRunningOnWindows())
        {
            return files.Where(f => !f.FullPath.EndsWith("pdb")).ToArray();
        }

        return files;
    }
}

public class BuildDirectories
{
    public DirectoryPath Build { get; private set; }
    public DirectoryPath TempBuild { get; private set; }
    public DirectoryPath PublishedDocumentation { get; private set; }
    public DirectoryPath ChocolateyNuspecDirectory { get; private set; }
    public DirectoryPath ChocolateyPackages { get; private set; }
    public DirectoryPath Packages { get; private set; }
    public ICollection<DirectoryPath> ToClean { get; private set; }

    public BuildDirectories(
        DirectoryPath build,
        DirectoryPath tempBuild,
        DirectoryPath publishedDocumentation,
        DirectoryPath chocolateyNuspecDirectory,
        DirectoryPath chocolateyPackages,
        DirectoryPath packages
        )
    {
        Build = build;
        TempBuild = tempBuild;
        PublishedDocumentation = publishedDocumentation;
        ChocolateyNuspecDirectory = chocolateyNuspecDirectory;
        ChocolateyPackages = chocolateyPackages;
        Packages = packages;

        ToClean = new[] {
            Build,
            TempBuild
        };
    }
}
