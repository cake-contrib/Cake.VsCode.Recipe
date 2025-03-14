public class GitHubCredentials
{
    public string Token { get; private set; }

    public GitHubCredentials(string token)
    {
        Token = token;
    }
}

public class MicrosoftTeamsCredentials
{
    public string WebHookUrl { get; private set;}

    public MicrosoftTeamsCredentials(string webHookUrl)
    {
        WebHookUrl = webHookUrl;
    }
}

public class SlackCredentials
{
    public string Token { get; private set; }
    public string Channel { get; private set; }

    public SlackCredentials(string token, string channel)
    {
        Token = token;
        Channel = channel;
    }
}

public class TwitterCredentials
{
    public string ConsumerKey { get; private set; }
    public string ConsumerSecret { get; private set; }
    public string AccessToken { get; private set; }
    public string AccessTokenSecret { get; private set; }

    public TwitterCredentials(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
    {
        ConsumerKey = consumerKey;
        ConsumerSecret = consumerSecret;
        AccessToken = accessToken;
        AccessTokenSecret = accessTokenSecret;
    }
}

public class ChocolateyCredentials
{
    public string ApiKey { get; private set; }
    public string SourceUrl { get; private set; }

    public ChocolateyCredentials(string apiKey, string sourceUrl)
    {
        ApiKey = apiKey;
        SourceUrl = sourceUrl;
    }
}

public class AppVeyorCredentials
{
    public string ApiToken { get; private set; }

    public AppVeyorCredentials(string apiToken)
    {
        ApiToken = apiToken;
    }
}

public class WyamCredentials
{
    public string AccessToken { get; private set; }
    public string DeployRemote { get; private set; }
    public string DeployBranch { get; private set; }

    public WyamCredentials(string accessToken, string deployRemote, string deployBranch)
    {
        AccessToken = accessToken;
        DeployRemote = deployRemote;
        DeployBranch = deployBranch;
    }
}

public class MarketplaceCredentials
{
    public string Token { get; private set; }

    public MarketplaceCredentials(string token)
    {
        Token = token;
    }
}

public static GitHubCredentials GetGitHubCredentials(ICakeContext context)
{
    return new GitHubCredentials(
        context.EnvironmentVariable(Environment.GithubTokenVariable));
}

public static MicrosoftTeamsCredentials GetMicrosoftTeamsCredentials(ICakeContext context)
{
    return new MicrosoftTeamsCredentials(
        context.EnvironmentVariable(Environment.MicrosoftTeamsWebHookUrlVariable));
}

public static SlackCredentials GetSlackCredentials(ICakeContext context)
{
    return new SlackCredentials(
        context.EnvironmentVariable(Environment.SlackTokenVariable),
        context.EnvironmentVariable(Environment.SlackChannelVariable));
}

public static TwitterCredentials GetTwitterCredentials(ICakeContext context)
{
    return new TwitterCredentials(
        context.EnvironmentVariable(Environment.TwitterConsumerKeyVariable),
        context.EnvironmentVariable(Environment.TwitterConsumerSecretVariable),
        context.EnvironmentVariable(Environment.TwitterAccessTokenVariable),
        context.EnvironmentVariable(Environment.TwitterAccessTokenSecretVariable));
}

public static ChocolateyCredentials GetChocolateyCredentials(ICakeContext context)
{
    return new ChocolateyCredentials(
        context.EnvironmentVariable(Environment.ChocolateyApiKeyVariable),
        context.EnvironmentVariable(Environment.ChocolateySourceUrlVariable));
}

public static AppVeyorCredentials GetAppVeyorCredentials(ICakeContext context)
{
    return new AppVeyorCredentials(
        context.EnvironmentVariable(Environment.AppVeyorApiTokenVariable));
}

public static WyamCredentials GetWyamCredentials(ICakeContext context)
{
    return new WyamCredentials(
        context.EnvironmentVariable(Environment.WyamAccessTokenVariable),
        context.EnvironmentVariable(Environment.WyamDeployRemoteVariable),
        context.EnvironmentVariable(Environment.WyamDeployBranchVariable));
}

public static MarketplaceCredentials GetMarketplaceCredentials(ICakeContext context)
{
    return new MarketplaceCredentials(
        context.EnvironmentVariable(Environment.MarketplaceTokenVariable)
    );
}
