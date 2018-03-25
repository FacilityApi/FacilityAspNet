using System.Collections.Generic;
using Facility.Definition;
using Facility.Definition.CodeGen;
using Facility.Definition.Http;

namespace Facility.CodeGen.AspNet
{
	/// <summary>
	/// Generates an APS.NET controller.
	/// </summary>
	public sealed class AspNetGenerator : CodeGenerator
	{
		/// <summary>
		/// The name of the controller namespace (optional).
		/// </summary>
		public string NamespaceName { get; set; }

		/// <summary>
		/// The name of the API namespace (optional).
		/// </summary>
		public string ApiNamespaceName { get; set; }

		/// <summary>
		/// The target framework (optional, defaults to WebApi).
		/// </summary>
		public AspNetFramework Target { get; set; }

		/// <summary>
		/// Generates the ASP.NET controller.
		/// </summary>
		protected override CodeGenOutput GenerateOutputCore(ServiceInfo serviceInfo)
		{
			string serviceName = serviceInfo.Name;
			string apiNamespaceName = ApiNamespaceName ?? CSharpUtility.GetNamespaceName(serviceInfo);
			string namespaceName = NamespaceName ?? $"{apiNamespaceName}.Controllers";
			string controllerName = $"{CodeGenUtility.Capitalize(serviceName)}Controller";
			var httpServiceInfo = new HttpServiceInfo(serviceInfo);

			return new CodeGenOutput(CreateNamedText($"{controllerName}{CSharpUtility.FileExtension}", code =>
			{
				CSharpUtility.WriteFileHeader(code, GeneratorName);

				List<string> usings = new List<string>
				{
					"System",
					"System.Net.Http",
					"System.Threading",
					"System.Threading.Tasks",
					"Facility.Core",
					Target == AspNetFramework.WebApi ? "System.Web.Http" : "Microsoft.AspNetCore.Mvc",
					apiNamespaceName
				};
				CSharpUtility.WriteUsings(code, usings, namespaceName);

				code.WriteLine("#pragma warning disable 1591 // missing XML comment");
				code.WriteLine();

				code.WriteLine($"namespace {namespaceName}");
				using (code.Block())
				{
					CSharpUtility.WriteCodeGenAttribute(code, GeneratorName);
					code.WriteLine($"public partial class {controllerName}");
					using (code.Block())
					{
						foreach (var httpMethodInfo in httpServiceInfo.Methods)
						{
							var methodInfo = httpMethodInfo.ServiceMethod;
							string methodName = CodeGenUtility.Capitalize(methodInfo.Name);

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

		private static string GetHttpMethodAttribute(HttpMethodInfo httpMethodInfo)
		{
			return "Http" + CodeGenUtility.Capitalize(httpMethodInfo.Method.ToLowerInvariant());
		}
	}
}
