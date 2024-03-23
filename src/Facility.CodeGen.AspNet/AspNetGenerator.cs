using Facility.Definition;
using Facility.Definition.CodeGen;
using Facility.Definition.Http;

namespace Facility.CodeGen.AspNet;

/// <summary>
/// Generates an ASP.NET controller.
/// </summary>
public sealed class AspNetGenerator : CodeGenerator
{
	/// <summary>
	/// Generates an ASP.NET controller.
	/// </summary>
	/// <param name="settings">The settings.</param>
	/// <returns>The number of updated files.</returns>
	public static int GenerateAspNet(AspNetGeneratorSettings settings) =>
		FileGenerator.GenerateFiles(new AspNetGenerator { GeneratorName = nameof(AspNetGenerator) }, settings);

	/// <summary>
	/// The name of the controller namespace (optional).
	/// </summary>
	public string? NamespaceName { get; set; }

	/// <summary>
	/// The name of the API namespace (optional).
	/// </summary>
	public string? ApiNamespaceName { get; set; }

	/// <summary>
	/// The target framework (optional, defaults to WebApi).
	/// </summary>
	public AspNetFramework TargetFramework { get; set; }

	/// <summary>
	/// Generates the ASP.NET controller.
	/// </summary>
	public override CodeGenOutput GenerateOutput(ServiceInfo service)
	{
		var serviceName = service.Name;
		var apiNamespaceName = ApiNamespaceName ?? CSharpUtility.GetNamespaceName(service);
		var namespaceName = NamespaceName ?? $"{apiNamespaceName}.Controllers";
		var controllerName = $"{CodeGenUtility.Capitalize(serviceName)}Controller";
		var httpServiceInfo = HttpServiceInfo.Create(service);

		return new CodeGenOutput(CreateFile($"{controllerName}{CSharpUtility.FileExtension}", code =>
		{
			CSharpUtility.WriteFileHeader(code, GeneratorName ?? "");

			var usings = new List<string>
			{
				"System",
				"System.Net.Http",
				"System.Threading",
				"System.Threading.Tasks",
				"Facility.Core",
				TargetFramework == AspNetFramework.WebApi ? "System.Web.Http" : "Microsoft.AspNetCore.Mvc",
				apiNamespaceName,
			};
			CSharpUtility.WriteUsings(code, usings, namespaceName);

			code.WriteLine("#pragma warning disable 1591 // missing XML comment");
			if (TargetFramework != AspNetFramework.WebApi)
				code.WriteLine("#pragma warning disable ASP0018 // Unused route parameter");
			code.WriteLine();

			code.WriteLine($"namespace {namespaceName}");
			using (code.Block())
			{
				CSharpUtility.WriteCodeGenAttribute(code, GeneratorName ?? "");
				code.WriteLine($"public partial class {controllerName}");
				using (code.Block())
				{
					foreach (var httpMethodInfo in httpServiceInfo.Methods)
					{
						var methodInfo = httpMethodInfo.ServiceMethod;
						var methodName = CodeGenUtility.Capitalize(methodInfo.Name);

						code.WriteLineSkipOnce();
						CSharpUtility.WriteObsoleteAttribute(code, methodInfo);
						code.WriteLine($"[{GetHttpMethodAttribute(httpMethodInfo)}, Route(\"{httpMethodInfo.Path.Substring(1)}\")]");
						code.WriteLine($"public Task<HttpResponseMessage> {methodName}(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))");
						using (code.Block())
							code.WriteLine($"return GetServiceHttpHandler().TryHandle{methodName}Async(httpRequest, cancellationToken);");
					}
				}
			}
		}));
	}

	/// <summary>
	/// Applies generator-specific settings.
	/// </summary>
	public override void ApplySettings(FileGeneratorSettings settings)
	{
		var aspNetSettings = (AspNetGeneratorSettings) settings;
		NamespaceName = aspNetSettings.NamespaceName;
		ApiNamespaceName = aspNetSettings.ApiNamespaceName;
		TargetFramework = aspNetSettings.TargetFramework;
	}

	private static string GetHttpMethodAttribute(HttpMethodInfo httpMethodInfo)
	{
		return "Http" + CodeGenUtility.Capitalize(httpMethodInfo.Method.ToLowerInvariant());
	}
}
