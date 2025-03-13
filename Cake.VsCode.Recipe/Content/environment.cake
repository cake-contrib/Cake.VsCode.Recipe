public static class Environment
{
    public static string GithubTokenVariable { get; private set; }
    public static string ChocolateyApiKeyVariable { get; private set; }
    public static string ChocolateySourceUrlVariable { get; private set; }
    public static string SlackTokenVariable { get; private set; }
    public static string SlackChannelVariable { get; private set; }
    public static string TwitterConsumerKeyVariable { get; private set; }
    public static string TwitterConsumerSecretVariable { get; private set; }
    public static string TwitterAccessTokenVariable { get; private set; }
    public static string TwitterAccessTokenSecretVariable { get; private set; }
    public static string AppVeyorApiTokenVariable { get; private set; }
    public static string MicrosoftTeamsWebHookUrlVariable { get; private set; }
    public static string WyamAccessTokenVariable { get; private set; }
    public static string WyamDeployRemoteVariable { get; private set; }
    public static string WyamDeployBranchVariable { get; private set; }
    public static string MarketplaceTokenVariable { get; private set; }

    public static void SetVariableNames(
        string githubTokenVariable = null,
        string chocolateyApiKeyVariable = null,
        string chocolateySourceUrlVariable = null,
        string slackTokenVariable = null,
        string slackChannelVariable = null,
        string twitterConsumerKeyVariable = null,
        string twitterConsumerSecretVariable = null,
        string twitterAccessTokenVariable = null,
        string twitterAccessTokenSecretVariable = null,
        string appVeyorApiTokenVariable = null,
        string microsoftTeamsWebHookUrlVariable = null,
        string wyamAccessTokenVariable = null,
        string wyamDeployRemoteVariable = null,
        string wyamDeployBranchVariable = null,
        string marketplaceTokenVariable = null)
    {
        GithubTokenVariable = githubTokenVariable ?? "GITHUB_PAT";
        ChocolateyApiKeyVariable = chocolateyApiKeyVariable ?? "CHOCOLATEY_API_KEY";
        ChocolateySourceUrlVariable = chocolateySourceUrlVariable ?? "CHOCOLATEY_SOURCE";
        SlackTokenVariable = slackTokenVariable ?? "SLACK_TOKEN";
        SlackChannelVariable = slackChannelVariable ?? "SLACK_CHANNEL";
        TwitterConsumerKeyVariable = twitterConsumerKeyVariable ?? "TWITTER_CONSUMER_KEY";
        TwitterConsumerSecretVariable = twitterConsumerSecretVariable ?? "TWITTER_CONSUMER_SECRET";
        TwitterAccessTokenVariable = twitterAccessTokenVariable ?? "TWITTER_ACCESS_TOKEN";
        TwitterAccessTokenSecretVariable = twitterAccessTokenSecretVariable ?? "TWITTER_ACCESS_TOKEN_SECRET";
        AppVeyorApiTokenVariable = appVeyorApiTokenVariable ?? "APPVEYOR_API_TOKEN";
        MicrosoftTeamsWebHookUrlVariable = microsoftTeamsWebHookUrlVariable ?? "MICROSOFTTEAMS_WEBHOOKURL";
        WyamAccessTokenVariable = wyamAccessTokenVariable ?? "WYAM_ACCESS_TOKEN";
        WyamDeployRemoteVariable = wyamDeployRemoteVariable ?? "WYAM_DEPLOY_REMOTE";
        WyamDeployBranchVariable = wyamDeployBranchVariable ?? "WYAM_DEPLOY_BRANCH";
        MarketplaceTokenVariable = marketplaceTokenVariable ?? "VSMARKETPLACE_TOKEN";
    }
}
