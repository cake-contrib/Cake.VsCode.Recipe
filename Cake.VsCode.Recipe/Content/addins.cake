///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin nuget:?package=MagicChunks&version=2.0.0.119
#addin nuget:?package=Cake.Figlet&version=1.3.1
#addin nuget:?package=Cake.Git&version=0.22.0
#addin nuget:?package=Cake.Incubator&version=5.1.0
#addin nuget:?package=Cake.Kudu&version=0.11.0
#addin nuget:?package=Cake.MicrosoftTeams&version=0.9.0
#addin nuget:?package=Cake.Npm&version=0.17.0
#addin nuget:?package=Cake.Slack&version=0.13.0
#addin nuget:?package=Cake.Twitter&version=0.10.1
#addin nuget:?package=Cake.VsCode&version=0.11.1
#addin nuget:?package=Cake.Wyam&version=2.2.9

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
