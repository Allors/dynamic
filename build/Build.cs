using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    const string ProjectName = "Allors.Dynamic";

    public static int Main() => Execute<Build>(x => x.Ci);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    [Parameter("Collect code coverage. Default is 'true'")] readonly bool Cover = true;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath TestsDirectory => ArtifactsDirectory / "tests";
    AbsolutePath CoverageFile => ArtifactsDirectory / "coverage" / "coverage";
    AbsolutePath NugetDirectory => ArtifactsDirectory / "nuget";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetClean(v => v.SetProject(SourceDirectory));
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
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });


    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution.GetProject(ProjectName + ".Tests"))
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .AddLoggers("trx;LogFileName=AllorsDynamicTests.trx")
                .EnableProcessLogOutput()
                .SetResultsDirectory(TestsDirectory)
                .When(Cover, _ => _
                    .EnableCollectCoverage()
                    .SetCoverletOutputFormat(CoverletOutputFormat.cobertura)
                    .SetCoverletOutput(CoverageFile)
                    .SetExcludeByFile("*.g.cs")
                    .When(IsServerBuild, _ => _
                        .EnableUseSourceLink()))
            );
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution.GetProject(ProjectName))
                .SetConfiguration(Configuration)
                .EnableIncludeSource()
                .EnableIncludeSymbols()
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetOutputDirectory(NugetDirectory));
        });

    Target CiNonWin => _ => _
        .DependsOn(Test);
    Target Ci => _ => _
        .DependsOn(Pack, Test);

}
