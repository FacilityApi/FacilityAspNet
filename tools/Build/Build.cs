using System;
using System.Linq;
using Faithlife.Build;
using static Faithlife.Build.BuildUtility;
using static Faithlife.Build.DotNetRunner;

return BuildRunner.Execute(args, build =>
{
	var codegen = "fsdgenaspnet";

	var gitLogin = new GitLoginInfo("FacilityApiBot", Environment.GetEnvironmentVariable("BUILD_BOT_PASSWORD") ?? "");

	var dotNetBuildSettings = new DotNetBuildSettings
	{
		NuGetApiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY"),
		DocsSettings = new DotNetDocsSettings
		{
			GitLogin = gitLogin,
			GitAuthor = new GitAuthorInfo("FacilityApiBot", "facilityapi@gmail.com"),
			SourceCodeUrl = "https://github.com/FacilityApi/FacilityAspNet/tree/master/src",
			ProjectHasDocs = name => !name.StartsWith("fsdgen", StringComparison.Ordinal),
		},
		PackageSettings = new DotNetPackageSettings
		{
			GitLogin = gitLogin,
			PushTagOnPublish = x => $"nuget.{x.Version}",
		},
	};

	build.AddDotNetTargets(dotNetBuildSettings);

	build.Target("codegen")
		.DependsOn("build")
		.Describe("Generates code from the FSD")
		.Does(() => CodeGen(verify: false));

	build.Target("verify-codegen")
		.DependsOn("build")
		.Describe("Ensures the generated code is up-to-date")
		.Does(() => CodeGen(verify: true));

	build.Target("test")
		.DependsOn("verify-codegen");

	void CodeGen(bool verify)
	{
		var configuration = dotNetBuildSettings.GetConfiguration();
		var toolPath = FindFiles($"src/{codegen}/bin/{configuration}/net5.0/{codegen}.dll").FirstOrDefault() ?? throw new BuildException($"Missing {codegen}.dll.");

		var verifyOption = verify ? "--verify" : null;

		RunDotNet("tool", "run", "FacilityConformance", "fsd", "--output", "conformance/ConformanceApi.fsd", verifyOption);
		RunDotNet("tool", "run", "FacilityConformance", "json", "--output", "conformance/ConformanceTests.json", verifyOption);

		RunDotNet(toolPath, "conformance/ConformanceApi.fsd", "conformance/WebApiControllerServer/Controllers",
			"--namespace", "WebApiControllerServer.Controllers", "--newline", "lf", verifyOption);
		RunDotNet(toolPath, "conformance/ConformanceApi.fsd", "conformance/CoreWebApiShimServer/Controllers",
			"--namespace", "CoreWebApiShimServer.Controllers", "--target", "core", "--newline", "lf", verifyOption);
		RunDotNet(toolPath, "conformance/ConformanceApi.fsd", "conformance/CoreControllerServer/Controllers",
			"--namespace", "CoreControllerServer.Controllers", "--target", "core", "--newline", "lf", verifyOption);
	}
});
