using ArgsReading;
using Facility.CodeGen.AspNet;
using Facility.CodeGen.Console;
using Facility.Definition.CodeGen;

namespace fsdgenaspnet
{
	public sealed class FsdGenAspNetApp : CodeGeneratorApp
	{
		public static int Main(string[] args) => new FsdGenAspNetApp().Run(args);

		protected override IReadOnlyList<string> Description => new[]
		{
			"Generates ASP.NET for a Facility Service Definition.",
		};

		protected override IReadOnlyList<string> ExtraUsage => new[]
		{
			"   --namespace <name>",
			"      The namespace used by the generated code.",
			"   --api-namespace <name>",
			"      The namespace used by the API class library.",
			"   --target (webapi|core)",
			"      The target ASP.NET framework (default webapi).",
		};

		protected override CodeGenerator CreateGenerator() => new AspNetGenerator();

		protected override FileGeneratorSettings CreateSettings(ArgsReader args) =>
			new AspNetGeneratorSettings
			{
				NamespaceName = args.ReadOption("namespace"),
				ApiNamespaceName = args.ReadOption("api-namespace"),
				TargetFramework = ReadTargetOption(args),
			};

		public static AspNetFramework ReadTargetOption(ArgsReader args)
		{
			var value = args.ReadOption("target");

			if (value is null)
				return AspNetFramework.WebApi;

			if (!Enum.TryParse(value, ignoreCase: true, result: out AspNetFramework result))
				throw new ArgsReaderException($"Invalid target '{value}'. (Should be 'webapi' or 'core'.)");

			return result;
		}
	}
}
