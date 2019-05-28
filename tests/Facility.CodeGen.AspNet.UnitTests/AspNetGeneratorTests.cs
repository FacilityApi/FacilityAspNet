using System.IO;
using System.Reflection;
using Facility.Definition;
using Facility.Definition.Fsd;
using NUnit.Framework;

namespace Facility.CodeGen.AspNet.UnitTests
{
	public sealed class AspNetGeneratorTests
	{
		[Test]
		public void GenerateConformanceApiSuccess()
		{
			ServiceInfo service;
			const string fileName = "Facility.CodeGen.AspNet.UnitTests.ConformanceApi.fsd";
			var parser = new FsdParser();
			var stream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream(fileName);
			Assert.IsNotNull(stream);
			using (var reader = new StreamReader(stream))
				service = parser.ParseDefinition(new ServiceDefinitionText(Path.GetFileName(fileName), reader.ReadToEnd()));

			var generator = new AspNetGenerator
			{
				GeneratorName = "AspNetGeneratorTests",
			};
			generator.GenerateOutput(service);
		}
	}
}
