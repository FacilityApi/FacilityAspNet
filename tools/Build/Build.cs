return BuildRunner.Execute(args, build =>
{
	var codegen = "fsdgenaspnet";

	var dotNetBuildSettings = new DotNetBuildSettings
	{
		NuGetApiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY"),
		PackageSettings = new DotNetPackageSettings
		{
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
		var verifyOption = verify ? "--verify" : null;

		RunDotNet("FacilityConformance", "fsd", "--output", "conformance/ConformanceApi.fsd", verifyOption);
		RunDotNet("FacilityConformance", "json", "--output", "conformance/ConformanceTests.json", verifyOption);

		RunCodeGen("conformance/ConformanceApi.fsd", "conformance/WebApiControllerServer/Controllers",
			"--namespace", "WebApiControllerServer.Controllers");
		RunCodeGen("conformance/ConformanceApi.fsd", "conformance/CoreControllerServer/Controllers",
			"--namespace", "CoreControllerServer.Controllers", "--target", "core");

		void RunCodeGen(params string?[] args) =>
			RunDotNet(new[] { "run", "--no-build", "--project", $"src/{codegen}", "-c", configuration, "--", "--newline", "lf", verifyOption }.Concat(args));
	}
});
