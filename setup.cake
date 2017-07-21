#load "nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease"

Environment.SetVariableNames();

BuildParameters.SetParameters(
    context: Context,
    buildSystem: BuildSystem,
    sourceDirectoryPath: "./src",
    title: "Cake.Transifex",
    repositoryOwner: "WormieCorp",
    repositoryName: "Cake.Transifex",
    appVeyorAccountName: "AdmiringWorm",
    shouldRunDotNetCorePack: true,
    shouldBuildNugetSourcePackage: true,
    solutionFilePath: "./Cake.Transifex.sln",
    shouldExecuteGitLink: false // Currently gitlink fails since we uses a custom path to the solution file
);

ToolSettings.SetToolSettings(
    context: Context,
    dupFinderExcludePattern: new string[] {
        BuildParameters.RootDirectoryPath + "/src/*.Tests/**/*.cs"
    },
     dupFinderExcludeFilesByStartingCommentSubstring: new string[] {
         "<auto-generated>"
     },
     testCoverageFilter: "+[Cake.Transifex*]* -[*.Tests]*",
     testCoverageExcludeByAttribute: "*.ExcludeFromCodeCoverage*",
     testCoverageExcludeByFile: "*Designer.cs;*.g.cs;*.g.i.cs"
);

BuildParameters.PrintParameters(Context);

// We want to override the creation of release notes
// when the following is true:
// We are running on appveyor
// We are inside the Main Repository
// We are on the main branch
// And we're not using a tagged branch
if (BuildParameters.IsRunningOnAppVeyor) {
    BuildParameters.Tasks.CreateReleaseNotesTask
        .WithCriteria(() => BuildParameters.IsMainRepository
            && BuildParameters.IsMasterBranch
            && !BuildParameters.IsTagged);
    BuildParameters.Tasks.ExportReleaseNotesTask
        .IsDependentOn(BuildParameters.Tasks.CreateReleaseNotesTask);
}

Build.RunDotNetCore();
