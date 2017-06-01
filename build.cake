#tool "nuget:?package=GitVersion.CommandLine&prerelease"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var solutionFile = "./ExpressionCache.sln";

string semVersion = null;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
	DeleteDirectoryIfExists("./artifacts");
	DeleteFiles("./src/*/bin/*/*.nupkg");
});

Task("Version")
    .Does(() =>
{
    if (AppVeyor.IsRunningOnAppVeyor)
    {
		GitVersion(new GitVersionSettings 
		{
			OutputType = GitVersionOutput.BuildServer,
			UpdateAssemblyInfo = true
		});
	}

    var result = GitVersion(new GitVersionSettings 
	{
		OutputType = GitVersionOutput.Json
    });

	semVersion = result.NuGetVersionV2;
});

Task("NuGet-Restore")
    .Does(() =>
{
	MSBuild(solutionFile, new MSBuildSettings()
		.SetConfiguration(configuration)
		.WithProperty("Version", semVersion)
		.WithTarget("restore")
	);
});

Task("Build")
    .IsDependentOn("NuGet-Restore")
    .Does(() =>
{
	DotNetCoreBuild(solutionFile, new DotNetCoreBuildSettings
    {
        Configuration = configuration
	});
});

Task("Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
	foreach (var file in GetFiles("./test/*/*.csproj"))
	{
		DotNetCoreTest(file.FullPath, new DotNetCoreTestSettings
		{
			Configuration = configuration,
			NoBuild = true
		});
	}
});

Task("Package")
	.IsDependentOn("Clean")
	.IsDependentOn("Version")
	.IsDependentOn("Unit-Tests")
    .Does(() =>
{
	foreach (var file in GetFiles("./src/*/*.csproj"))
	{
		MSBuild(file, new MSBuildSettings()
			.SetConfiguration(configuration)
			.WithProperty("Version", semVersion)
			.WithTarget("pack")
		);
	}

	CreateDirectoryIfNotExists("./artifacts");
	MoveFiles("./src/*/bin/*/*.nupkg", "./artifacts");
});

Task("Upload-Artifacts")
    .IsDependentOn("Package")
    .Does(() =>
{
	foreach (var file in GetFiles("./artifacts/*"))
	{
		if (AppVeyor.IsRunningOnAppVeyor)
		{
			AppVeyor.UploadArtifact(file);
		}
	}
});

//////////////////////////////////////////////////////////////////////
// HELPERS
//////////////////////////////////////////////////////////////////////

void CreateDirectoryIfNotExists(string path)
{
	var directory = Directory(path);
	if (!DirectoryExists(directory))
	{
		CreateDirectory(directory);
	}
}

void DeleteDirectoryIfExists(string path)
{
	var directory = Directory(path);
	if (DirectoryExists(directory))
	{
		DeleteDirectory(directory, true);
	}
}

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Upload-Artifacts");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);