using System;
using System.Collections.Generic;
using ArgsReading;
using Facility.CodeGen.AspNet;
using Facility.CodeGen.Console;
using Facility.Definition.CodeGen;

namespace fsdgenaspnet
{
	public sealed class FsdGenAspNetApp : CodeGeneratorApp
	{
		public static int Main(string[] args)
		{
			return new FsdGenAspNetApp().Run(args);
		}

		protected override IReadOnlyList<string> Description => new[]
		{
			"Generates ASP.NET for a Facility Service Definition.",
		};

		protected override IReadOnlyList<string> ExtraUsage => new[]
		{
			"   --namespace <name>",
			"      The namespace used by the generated code.",
			"   --apinamespace <name>",
			"      The namespace used by the API class library.",
			"   --target (webapi|core)",
			"      The target framework to write code against. (default `webapi`)",
		};

		protected override CodeGenerator CreateGenerator(ArgsReader args)
		{
			return new AspNetGenerator
			{
				NamespaceName = args.ReadOption("namespace"),
				ApiNamespaceName = args.ReadOption("apinamespace"),
				Target = ReadTargetOption(args),
			};
		}

		public static TargetFramework ReadTargetOption(ArgsReader args)
		{
			string value = args.ReadOption("target");

			if (value == null)
				return TargetFramework.WebApi;

			TargetFramework result;
			if (!Enum.TryParse(value, ignoreCase: true, result: out result))
				throw new ArgsReaderException($"Invalid target '{value}'. (Should be 'webapi' or 'core'.)");

			return result;
		}
	}
}
