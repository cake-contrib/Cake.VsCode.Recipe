public static class BuildParameters
{
    private static string _gitterMessage;
    private static string _microsoftTeamsMessage;
    private static string _twitterMessage;

    public static string Target { get; private set; }
    public static string Configuration { get; private set; }
    public static Cake.Core.Configuration.ICakeConfiguration CakeConfiguration { get; private set; }
    public static bool IsLocalBuild { get; private set; }
    public static bool IsRunningOnUnix { get; private set; }
    public static bool IsRunningOnWindows { get; private set; }
    public static bool IsRunningOnAppVeyor { get; private set; }
    public static bool IsPullRequest { get; private set; }
    public static bool IsMainRepository { get; private set; }
    public static bool IsPublicRepository {get; private set; }
    public static bool IsMasterBranch { get; private set; }
    public static bool IsDevelopBranch { get; private set; }
    public static bool IsReleaseBranch { get; private set; }
    public static bool IsHotFixBranch { get; private set ; }
    public static bool IsTagged { get; private set; }
    public static bool IsPublishBuild { get; private set; }
    public static bool IsReleaseBuild { get; private set; }
    public static bool PrepareLocalRelease { get; set; }
    public static bool TreatWarningsAsErrors { get; set; }
    public static string MasterBranchName { get; private set; }
    public static string DevelopBranchName { get; private set; }
    public static string TypeScriptVersionNumber { get; private set; }
    public static string VsceVersionNumber { get; private set; }
    public static string MarketplacePublisher { get; private set; }
    public static string ChocolateyPackagingFolderName { get; private set; }
    public static string ChocolateyPackagingPackageId { get; private set; }

    public static string GitterMessage
    {
        get
        {
            if(_gitterMessage == null)
            {
                return "@/all Version " + Version.SemVersion + " of the " + Title + " VS Code Extension has just been released, https://marketplace.visualstudio.com/items?itemName=" + MarketplacePublisher + "." + Title + ".  Full release notes: https://github.com/" + RepositoryOwner + "/" + RepositoryName + "/releases/tag/" + Version.SemVersion;
            }
            else
            {
                return _gitterMessage;
            }
        }

        set {
            _gitterMessage = value;
        }
    }

    public static string MicrosoftTeamsMessage
    {
        get
        {
            if(_microsoftTeamsMessage == null)
            {
                return "Version " + Version.SemVersion + " of the " + Title + " VS Code Extension has just been released, https://marketplace.visualstudio.com/items?itemName=" + MarketplacePublisher + "." + Title + ".  Full release notes: https://github.com/" + RepositoryOwner + "/" + RepositoryName + "/releases/tag/" + Version.SemVersion;
            }
            else
            {
                return _microsoftTeamsMessage;
            }
        }

        set
        {
            _microsoftTeamsMessage = value;
        }
    }

    public static string TwitterMessage
    {
        get
        {
            if(_twitterMessage == null)
            {
                return "Version " + Version.SemVersion + " of the " + Title + " VS Code Extension has just been released, https://marketplace.visualstudio.com/items?itemName=" + MarketplacePublisher + "." + Title + ". @code  Full release notes: https://github.com/" + RepositoryOwner + "/" + RepositoryName + "/releases/tag/" + Version.SemVersion;
            }
            else
            {
                return _twitterMessage;
            }
        }

        set
        {
            _twitterMessage = value;
        }
    }

    public static GitHubCredentials GitHub { get; private set; }
    public static MicrosoftTeamsCredentials MicrosoftTeams { get; private set; }
    public static GitterCredentials Gitter { get; private set; }
    public static SlackCredentials Slack { get; private set; }
    public static TwitterCredentials Twitter { get; private set; }
    public static ChocolateyCredentials Chocolatey { get; private set; }
    public static AppVeyorCredentials AppVeyor { get; private set; }
    public static WyamCredentials Wyam { get; private set; }
    public static MarketplaceCredentials Marketplace { get; private set; }
    public static BuildVersion Version { get; private set; }
    public static BuildPaths Paths { get; private set; }
    public static BuildTasks Tasks { get; set; }
    public static DirectoryPath RootDirectoryPath { get; private set; }
    public static string Title { get; private set; }
    public static string RepositoryOwner { get; private set; }
    public static string RepositoryName { get; private set; }
    public static string AppVeyorAccountName { get; private set; }
    public static string AppVeyorProjectSlug { get; private set; }

    public static bool ShouldPostToGitter { get; private set; }
    public static bool ShouldPostToSlack { get; private set; }
    public static bool ShouldPostToTwitter { get; private set; }
    public static bool ShouldPostToMicrosoftTeams { get; private set; }
    public static bool ShouldDownloadMilestoneReleaseNotes { get; private set;}
    public static bool ShouldDownloadFullReleaseNotes { get; private set;}
    public static bool ShouldNotifyBetaReleases { get; private set; }

    public static FilePath MilestoneReleaseNotesFilePath { get; private set; }
    public static FilePath FullReleaseNotesFilePath { get; private set; }

    public static bool ShouldPublishChocolatey { get; private set; }
    public static bool ShouldPublishGitHub { get; private set; }
    public static bool ShouldPublishExtension { get; private set; }
    public static bool ShouldGenerateDocumentation { get; private set; }
    public static bool ShouldRunGitVersion { get; private set; }

    public static DirectoryPath WyamRootDirectoryPath { get; private set; }
    public static DirectoryPath WyamPublishDirectoryPath { get; private set; }
    public static FilePath WyamConfigurationFile { get; private set; }
    public static string WyamRecipe { get; private set; }
    public static string WyamTheme { get; private set; }
    public static string WebHost { get; private set; }
    public static string WebLinkRoot { get; private set; }
    public static string WebBaseEditUrl { get; private set; }

    public static IBuildProvider BuildProvider { get; private set; }

    static BuildParameters()
    {
        Tasks = new BuildTasks();
    }

    public static bool CanUseGitReleaseManager
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.GitHub.UserName) &&
                !string.IsNullOrEmpty(BuildParameters.GitHub.Password);
        }
    }

    public static bool CanPostToGitter
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.Gitter.Token) &&
                !string.IsNullOrEmpty(BuildParameters.Gitter.RoomId);
        }
    }

    public static bool CanPostToSlack
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.Slack.Token) &&
                !string.IsNullOrEmpty(BuildParameters.Slack.Channel);
        }
    }

    public static bool CanPostToTwitter
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.Twitter.ConsumerKey) &&
                !string.IsNullOrEmpty(BuildParameters.Twitter.ConsumerSecret) &&
                !string.IsNullOrEmpty(BuildParameters.Twitter.AccessToken) &&
                !string.IsNullOrEmpty(BuildParameters.Twitter.AccessTokenSecret);
        }
    }

    public static bool CanPostToMicrosoftTeams
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.MicrosoftTeams.WebHookUrl);
        }
    }

    public static bool CanUseWyam
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.Wyam.AccessToken) &&
                !string.IsNullOrEmpty(BuildParameters.Wyam.DeployRemote) &&
                !string.IsNullOrEmpty(BuildParameters.Wyam.DeployBranch);
        }
    }

    public static bool CanPublishToChocolatey
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.Chocolatey.ApiKey) &&
                !string.IsNullOrEmpty(BuildParameters.Chocolatey.SourceUrl);
        }
    }

    public static void SetBuildVersion(BuildVersion version)
    {
        Version  = version;
    }

    public static void SetBuildPaths(BuildPaths paths)
    {
        Paths = paths;
    }

    public static void PrintParameters(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        context.Information("Printing Build Parameters...");
        context.Information("IsLocalBuild: {0}", IsLocalBuild);
        context.Information("IsPullRequest: {0}", IsPullRequest);
        context.Information("IsMainRepository: {0}", IsMainRepository);
        context.Information("IsPublicRepository: {0}", IsPublicRepository);
        context.Information("IsTagged: {0}", IsTagged);
        context.Information("IsMasterBranch: {0}", IsMasterBranch);
        context.Information("IsDevelopBranch: {0}", IsDevelopBranch);
        context.Information("IsReleaseBranch: {0}", IsReleaseBranch);
        context.Information("IsHotFixBranch: {0}", IsHotFixBranch);
        context.Information("TreatWarningsAsErrors: {0}", TreatWarningsAsErrors);
        context.Information("ShouldPostToGitter: {0}", ShouldPostToGitter);
        context.Information("ShouldPostToSlack: {0}", ShouldPostToSlack);
        context.Information("ShouldPostToTwitter: {0}", ShouldPostToTwitter);
        context.Information("ShouldPostToMicrosoftTeams: {0}", ShouldPostToMicrosoftTeams);
        context.Information("ShouldDownloadFullReleaseNotes: {0}", ShouldDownloadFullReleaseNotes);
        context.Information("ShouldDownloadMilestoneReleaseNotes: {0}", ShouldDownloadMilestoneReleaseNotes);
        context.Information("ShouldNotifyBetaReleases: {0}", ShouldNotifyBetaReleases);
        context.Information("ShouldGenerateDocumentation: {0}", ShouldGenerateDocumentation);
        context.Information("ShouldRunGitVersion: {0}", ShouldRunGitVersion);
        context.Information("IsRunningOnUnix: {0}", IsRunningOnUnix);
        context.Information("IsRunningOnWindows: {0}", IsRunningOnWindows);
        context.Information("IsRunningOnAppVeyor: {0}", IsRunningOnAppVeyor);
        context.Information("RepositoryOwner: {0}", RepositoryOwner);
        context.Information("RepositoryName: {0}", RepositoryName);
        context.Information("PrepareLocalRelease: {0}", PrepareLocalRelease);

        context.Information("WyamRootDirectoryPath: {0}", WyamRootDirectoryPath);
        context.Information("WyamPublishDirectoryPath: {0}", WyamPublishDirectoryPath);
        context.Information("WyamConfigurationFile: {0}", WyamConfigurationFile);
        context.Information("WyamRecipe: {0}", WyamRecipe);
        context.Information("WyamTheme: {0}", WyamTheme);
        context.Information("Wyam Deploy Branch: {0}", Wyam.DeployBranch);
        context.Information("Wyam Deploy Remote: {0}", Wyam.DeployRemote);
        context.Information("WebHost: {0}", WebHost);
        context.Information("WebLinkRoot: {0}", WebLinkRoot);
        context.Information("WebBaseEditUrl: {0}", WebBaseEditUrl);
        context.Information("TypeScriptVersionNumber: {0}", TypeScriptVersionNumber);
        context.Information("VsceVersionNumber: {0}", VsceVersionNumber);
        context.Information("MarketplacePublisher: {0}", MarketplacePublisher);
        context.Information("ChocolateyPackagingFolderName: {0}", ChocolateyPackagingFolderName);
        context.Information("ChocolateyPackagingPackageId: {0}", ChocolateyPackagingPackageId);
    }

    public static void SetParameters(
        ICakeContext context,
        BuildSystem buildSystem,
        string title,
        DirectoryPath rootDirectoryPath = null,
        string repositoryOwner = null,
        string repositoryName = null,
        string appVeyorAccountName = null,
        string appVeyorProjectSlug = null,
        bool shouldPostToGitter = true,
        bool shouldPostToSlack = true,
        bool shouldPostToTwitter = true,
        bool shouldPostToMicrosoftTeams = false,
        bool shouldDownloadMilestoneReleaseNotes = false,
        bool shouldDownloadFullReleaseNotes = false,
        bool shouldNotifyBetaReleases = false,
        FilePath milestoneReleaseNotesFilePath = null,
        FilePath fullReleaseNotesFilePath = null,
        bool shouldPublishChocolatey = true,
        bool shouldPublishGitHub = true,
        bool shouldPublishExtension = true,
        bool shouldGenerateDocumentation = true,
        bool? shouldRunGitVersion = null,
        string gitterMessage = null,
        string microsoftTeamsMessage = null,
        string twitterMessage = null,
        DirectoryPath wyamRootDirectoryPath = null,
        DirectoryPath wyamPublishDirectoryPath = null,
        FilePath wyamConfigurationFile = null,
        string wyamRecipe = null,
        string wyamTheme = null,
        string webHost = null,
        string webLinkRoot = null,
        string webBaseEditUrl = null,
        bool isPublicRepository = true,
        bool treatWarningsAsErrors = true,
        string masterBranchName = "master",
        string developBranchName = "develop",
        string typeScriptVersionNumber = "2.9.2",
        string vsceVersionNumber = "1.43.0",
        string marketPlacePublisher = "gep13",
        string chocolateyPackagingFolderName = "chocolatey",
        string chocolateyPackagingPackageId = null
        )
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        BuildProvider = GetBuildProvider(context, buildSystem);

        Title = title;
        RootDirectoryPath = rootDirectoryPath ?? context.MakeAbsolute(context.Environment.WorkingDirectory);
        RepositoryOwner = repositoryOwner ?? string.Empty;
        RepositoryName = repositoryName ?? Title;
        AppVeyorAccountName = appVeyorAccountName ?? RepositoryOwner.Replace("-", "").ToLower();
        AppVeyorProjectSlug = appVeyorProjectSlug ?? Title.Replace(".", "-").ToLower();

        GitterMessage = gitterMessage;
        MicrosoftTeamsMessage = microsoftTeamsMessage;
        TwitterMessage = twitterMessage;

        WyamRootDirectoryPath = wyamRootDirectoryPath ?? context.MakeAbsolute(context.Directory("docs"));
        WyamPublishDirectoryPath = wyamPublishDirectoryPath ?? context.MakeAbsolute(context.Directory("BuildArtifacts/temp/_PublishedDocumentation"));
        WyamConfigurationFile = wyamConfigurationFile ?? context.MakeAbsolute((FilePath)"config.wyam");
        WyamRecipe = wyamRecipe ?? "Docs";
        WyamTheme = wyamTheme ?? "Samson";
        WebHost = webHost ?? string.Format("{0}.github.io", repositoryOwner);
        WebLinkRoot = webLinkRoot ?? title;
        WebBaseEditUrl = webBaseEditUrl ?? string.Format("https://github.com/{0}/{1}/tree/develop/docs/input/", repositoryOwner, title);

        ShouldPostToGitter = shouldPostToGitter;
        ShouldPostToSlack = shouldPostToSlack;
        ShouldPostToTwitter = shouldPostToTwitter;
        ShouldPostToMicrosoftTeams = shouldPostToMicrosoftTeams;
        ShouldDownloadFullReleaseNotes = shouldDownloadFullReleaseNotes;
        ShouldDownloadMilestoneReleaseNotes = shouldDownloadMilestoneReleaseNotes;
        ShouldNotifyBetaReleases = shouldNotifyBetaReleases;
        ShouldRunGitVersion = shouldRunGitVersion ?? context.IsRunningOnWindows();

        MilestoneReleaseNotesFilePath = milestoneReleaseNotesFilePath ?? RootDirectoryPath.CombineWithFilePath("CHANGELOG.md");
        FullReleaseNotesFilePath = fullReleaseNotesFilePath ?? RootDirectoryPath.CombineWithFilePath("ReleaseNotes.md");

        Target = context.Argument("target", "Default");
        Configuration = context.Argument("configuration", "Release");
        PrepareLocalRelease = context.Argument("prepareLocalRelease", false);
        CakeConfiguration = context.GetConfiguration();
        MasterBranchName = masterBranchName;
        DevelopBranchName = developBranchName;
        TypeScriptVersionNumber = typeScriptVersionNumber;
        VsceVersionNumber = vsceVersionNumber;
        MarketplacePublisher = marketPlacePublisher;
        ChocolateyPackagingFolderName = chocolateyPackagingFolderName;
        ChocolateyPackagingPackageId = chocolateyPackagingPackageId ?? title;
        IsLocalBuild = buildSystem.IsLocalBuild;
        IsRunningOnUnix = context.IsRunningOnUnix();
        IsRunningOnWindows = context.IsRunningOnWindows();
        IsRunningOnAppVeyor = buildSystem.AppVeyor.IsRunningOnAppVeyor;
        IsPullRequest = BuildProvider.PullRequest.IsPullRequest;
        IsMainRepository = StringComparer.OrdinalIgnoreCase.Equals(string.Concat(repositoryOwner, "/", repositoryName), BuildProvider.Repository.Name);
        IsPublicRepository = isPublicRepository;
        IsMasterBranch = StringComparer.OrdinalIgnoreCase.Equals(masterBranchName, BuildProvider.Repository.Branch);
        IsDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals(developBranchName, BuildProvider.Repository.Branch);
        IsReleaseBranch = BuildProvider.Repository.Branch.StartsWith("release", StringComparison.OrdinalIgnoreCase);
        IsHotFixBranch = BuildProvider.Repository.Branch.StartsWith("hotfix", StringComparison.OrdinalIgnoreCase);
        IsTagged = (
            BuildProvider.Repository.Tag.IsTag &&
            !string.IsNullOrWhiteSpace(BuildProvider.Repository.Tag.Name)
        );
        TreatWarningsAsErrors = treatWarningsAsErrors;
        GitHub = GetGitHubCredentials(context);
        MicrosoftTeams = GetMicrosoftTeamsCredentials(context);
        Gitter = GetGitterCredentials(context);
        Slack = GetSlackCredentials(context);
        Twitter = GetTwitterCredentials(context);
        Chocolatey = GetChocolateyCredentials(context);
        AppVeyor = GetAppVeyorCredentials(context);
        Wyam = GetWyamCredentials(context);
        Marketplace = GetMarketplaceCredentials(context);
        IsPublishBuild = new [] {
            "Create-Release-Notes"
        }.Any(
            releaseTarget => StringComparer.OrdinalIgnoreCase.Equals(releaseTarget, Target)
        );
        IsReleaseBuild = new [] {
            "Publish-Chocolatey-Packages",
            "Publish-GitHub-Release"
        }.Any(
            publishTarget => StringComparer.OrdinalIgnoreCase.Equals(publishTarget, Target)
        );

        SetBuildPaths(BuildPaths.GetPaths(context));

        ShouldPublishChocolatey = (!IsLocalBuild &&
                                    !IsPullRequest &&
                                    IsMainRepository &&
                                    (IsMasterBranch || IsReleaseBranch || IsHotFixBranch) &&
                                    IsTagged &&
                                    shouldPublishChocolatey);

        ShouldPublishGitHub = (!IsLocalBuild &&
                                !IsPullRequest &&
                                IsMainRepository &&
                                (IsMasterBranch || IsReleaseBranch || IsHotFixBranch) &&
                                IsTagged &&
                                shouldPublishGitHub);

        ShouldGenerateDocumentation = (!IsLocalBuild &&
                                !IsPullRequest &&
                                IsMainRepository &&
                                (IsMasterBranch || IsDevelopBranch) &&
                                shouldGenerateDocumentation);

        ShouldPublishExtension = (!IsLocalBuild &&
                                !IsPullRequest &&
                                IsMainRepository &&
                                IsMasterBranch &&
                                IsTagged &&
                                shouldPublishExtension);
    }
}
