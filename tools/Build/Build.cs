using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Faithlife.Build;
using static Faithlife.Build.AppRunner;
using static Faithlife.Build.BuildUtility;
using static Faithlife.Build.DotNetRunner;

internal static class Build
{
	public static int Main(string[] args) => BuildRunner.Execute(args, build =>
	{
		var conformanceVersion = "2.0.2-alpha8";

		var codegen = "fsdgenaspnet";

		var dotNetTools = new DotNetTools(Path.Combine("tools", "bin")).AddSource(Path.Combine("tools", "bin"));

		var dotNetBuildSettings = new DotNetBuildSettings
		{
			NuGetApiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY"),
			DocsSettings = new DotNetDocsSettings
			{
				GitLogin = new GitLoginInfo("FacilityApiBot", Environment.GetEnvironmentVariable("BUILD_BOT_PASSWORD") ?? ""),
				GitAuthor = new GitAuthorInfo("FacilityApiBot", "facilityapi@gmail.com"),
				GitBranchName = Environment.GetEnvironmentVariable("APPVEYOR_REPO_BRANCH"),
				SourceCodeUrl = "https://github.com/FacilityApi/RepoName/tree/master/src",
				ProjectHasDocs = name => !name.StartsWith("fsdgen", StringComparison.Ordinal),
			},
			DotNetTools = dotNetTools,
			SourceLinkSettings = new SourceLinkSettings
			{
				ShouldTestPackage = name => !name.StartsWith("fsdgen", StringComparison.Ordinal),
			},
		};

		build.AddDotNetTargets(dotNetBuildSettings);

		build.Target("codegen")
			.DependsOn("build")
			.Describe("Generates code from the FSD")
			.Does(() => codeGen(verify: false));

		build.Target("verify-codegen")
			.DependsOn("build")
			.Describe("Ensures the generated code is up-to-date")
			.Does(() => codeGen(verify: true));

		build.Target("test")
			.DependsOn("verify-codegen");

		void codeGen(bool verify)
		{
			string verifyOption = verify ? "--verify" : null;

			var conformanceToolPath = dotNetTools.GetToolPath($"FacilityConformance/{conformanceVersion}");
			RunApp(conformanceToolPath, "fsd", "--output", "conformance/ConformanceApi.fsd", verifyOption);
			RunApp(conformanceToolPath, "json", "--output", "conformance/ConformanceTests.json", verifyOption);

			string configuration = dotNetBuildSettings.BuildOptions.ConfigurationOption.Value;
			string versionSuffix = $"cg{DateTime.UtcNow:yyyyMMddHHmmss}";
			RunDotNet("pack", Path.Combine("src", codegen, $"{codegen}.csproj"), "-c", configuration, "--no-build",
				"--output", Path.GetFullPath(Path.Combine("tools", "bin")), "--version-suffix", versionSuffix);

			string packagePath = FindFiles($"tools/bin/{codegen}.*-{versionSuffix}.nupkg").Single();
			string packageVersion = Regex.Match(packagePath, @"[/\\][^/\\]*\.([0-9]+\.[0-9]+\.[0-9]+(-.+)?)\.nupkg$").Groups[1].Value;
			string codegenToolPath = dotNetTools.GetToolPath($"{codegen}/{packageVersion}");

			RunApp(codegenToolPath, "conformance/ConformanceApi.fsd", "conformance/WebApiControllerServer/Controllers",
				"--namespace", "WebApiControllerServer.Controllers", "--newline", "lf", verifyOption);
			RunApp(codegenToolPath, "conformance/ConformanceApi.fsd", "conformance/CoreWebApiShimServer/Controllers",
				"--namespace", "CoreWebApiShimServer.Controllers", "--target", "core", "--newline", "lf", verifyOption);
			RunApp(codegenToolPath, "conformance/ConformanceApi.fsd", "conformance/CoreControllerServer/Controllers",
				"--namespace", "CoreControllerServer.Controllers", "--target", "core", "--newline", "lf", verifyOption);
		}
	});
}
