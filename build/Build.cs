using System;
using Nuke.Common;
using Nuke.Common.BuildServers;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.MSBuild;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

internal class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    private readonly string Configuration = IsLocalBuild ? "Debug" : "Release";

    [Solution("Shibari.sln")] private readonly Solution Solution;

    private Target Clean => _ => _
        .Executes(() => { });

    private Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            MSBuild(s => s
                .SetTargetPath(Solution)
                .SetTargets("Restore"));
        });

    private Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            MSBuild(s => s
                .SetTargetPath(Solution)
                .SetTargets("Rebuild")
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(AppVeyor.Instance?.BuildVersion)
                .SetFileVersion(AppVeyor.Instance?.BuildVersion)
                .SetInformationalVersion(AppVeyor.Instance?.BuildVersion)
                .SetMaxCpuCount(Environment.ProcessorCount)
                .SetNodeReuse(IsLocalBuild));
        });

    public static int Main()
    {
        return Execute<Build>(x => x.Compile);
    }
}