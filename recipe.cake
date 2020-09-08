#load nuget:?package=Cake.Recipe&version=1.1.1

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            nuspecFilePath: "Cake.VsCode.Recipe/Cake.VsCode.Recipe.nuspec",
                            title: "Cake.VsCode.Recipe",
                            repositoryOwner: "cake-contrib",
                            repositoryName: "Cake.VsCode.Recipe",
                            appVeyorAccountName: "cakecontrib",
                            shouldRunGitVersion: true,
                            shouldPublishMyGet: false);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context);

BuildParameters.Tasks.CleanTask
    .IsDependentOn("Generate-Version-File");

Task("Generate-Version-File")
    .Does(() => {
        var buildMetaDataCodeGen = TransformText(@"
        public class BuildMetaData
        {
            public static string Date { get; } = ""<%date%>"";
            public static string Version { get; } = ""<%version%>"";
        }",
        "<%",
        "%>"
        )
   .WithToken("date", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
   .WithToken("version", BuildParameters.Version.SemVersion)
   .ToString();

    System.IO.File.WriteAllText(
        "./Cake.VsCode.Recipe/Content/version.cake",
        buildMetaDataCodeGen
    );
});

Build.RunNuGet();
