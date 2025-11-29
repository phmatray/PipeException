using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
    "ci",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    OnPushBranches = ["main", "develop", "feature/*"],
    OnPullRequestBranches = ["main", "develop"],
    InvokedTargets = [nameof(Test), nameof(Pack)],
    FetchDepth = 0,
    EnableGitHubToken = true,
    PublishArtifacts = true)]
[GitHubActions(
    "publish",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    OnPushTags = ["v*"],
    InvokedTargets = [nameof(Push)],
    FetchDepth = 0,
    EnableGitHubToken = true,
    ImportSecrets = [nameof(NuGetApiKey)])]
sealed class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly string Configuration = IsLocalBuild ? "Debug" : "Release";

    [Parameter("NuGet API key for publishing packages")]
    [Secret]
    readonly string? NuGetApiKey;

    [Parameter("NuGet source URL")]
    readonly string NuGetSource = "https://api.nuget.org/v3/index.json";

    [Solution]
    readonly Solution? Solution;

    AbsolutePath SourceDirectory => RootDirectory / "PipeException";
    AbsolutePath TestsDirectory => RootDirectory / "PipeException.Tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath TestResultsDirectory => ArtifactsDirectory / "test-results";
    AbsolutePath PackagesDirectory => ArtifactsDirectory / "packages";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .SetDeterministic(IsServerBuild)
                .SetContinuousIntegrationBuild(IsServerBuild));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultsDirectory / "*.trx")
        .Executes(() =>
        {
            TestResultsDirectory.CreateOrCleanDirectory();

            DotNetTest(s => s
                .SetProjectFile(TestsDirectory)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild()
                .SetResultsDirectory(TestResultsDirectory)
                .SetLoggers("trx;LogFileName=test-results.trx"));
        });

    Target Pack => _ => _
        .DependsOn(Test)
        .Produces(PackagesDirectory / "*.nupkg")
        .Produces(PackagesDirectory / "*.snupkg")
        .Executes(() =>
        {
            PackagesDirectory.CreateOrCleanDirectory();

            DotNetPack(s => s
                .SetProject(SourceDirectory)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild()
                .SetOutputDirectory(PackagesDirectory)
                .SetDeterministic(IsServerBuild)
                .SetContinuousIntegrationBuild(IsServerBuild));
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .Requires(() => NuGetApiKey)
        .Requires(() => Configuration == "Release")
        .Executes(() =>
        {
            PackagesDirectory.GlobFiles("*.nupkg")
                .ForEach(package =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(package)
                        .SetSource(NuGetSource)
                        .SetApiKey(NuGetApiKey)
                        .EnableSkipDuplicate());
                });
        });

    AbsolutePath ChangelogFile => RootDirectory / "CHANGELOG.md";

    Target Changelog => _ => _
        .Executes(() =>
        {
            ProcessTasks.StartProcess(
                    "git-cliff",
                    arguments: $"-o {ChangelogFile}",
                    workingDirectory: RootDirectory)
                .AssertZeroExitCode();

            Serilog.Log.Information("Changelog generated at {Path}", ChangelogFile);
        });
}
