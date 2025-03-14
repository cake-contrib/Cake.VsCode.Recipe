///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin nuget:?package=MagicChunks&version=2.0.0.119
#addin nuget:?package=Cake.Figlet&version=2.0.1
#addin nuget:?package=Cake.Git&version=1.1.0
#addin nuget:?package=Cake.Incubator&version=6.0.0
#addin nuget:?package=Cake.Kudu&version=1.0.1
#addin nuget:?package=Cake.MicrosoftTeams&version=1.0.1
#addin nuget:?package=Cake.Npm&version=1.0.0
#addin nuget:?package=Cake.Slack&version=1.0.1
#addin nuget:?package=Cake.Twitter&version=1.0.0
#addin nuget:?package=Cake.VsCode&version=0.11.1
#addin nuget:?package=Cake.Wyam&version=2.2.12

Action<string, IDictionary<string, string>> RequireAddin = (code, envVars) => {
    var script = MakeAbsolute(File(string.Format("./{0}.cake", Guid.NewGuid())));
    try
    {
        System.IO.File.WriteAllText(script.FullPath, code);
        var arguments = new Dictionary<string, string>();

        if(BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient") != null) {
            arguments.Add("nuget_useinprocessclient", BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient"));
        }

        if(BuildParameters.CakeConfiguration.GetValue("Settings_SkipVerification") != null) {
            arguments.Add("settings_skipverification", BuildParameters.CakeConfiguration.GetValue("Settings_SkipVerification"));
        }

        CakeExecuteScript(script,
            new CakeSettings
            {
                EnvironmentVariables = envVars,
                Arguments = arguments
            });
    }
    finally
    {
        if (FileExists(script))
        {
            DeleteFile(script);
        }
    }
};
