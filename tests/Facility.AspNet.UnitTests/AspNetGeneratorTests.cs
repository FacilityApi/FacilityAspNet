using System.IO;
using System.Reflection;
using Facility.Definition;
using Facility.Definition.Fsd;
using NUnit.Framework;

namespace Facility.AspNet.UnitTests
{
	public sealed class AspNetGeneratorTests
	{
		[Test]
		public void GenerateExampleApiSuccess()
		{
			ServiceInfo service;
			const string fileName = "Facility.AspNet.UnitTests.ExampleApi.fsd";
			var parser = new FsdParser();
			var stream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream(fileName);
			Assert.IsNotNull(stream);
			using (var reader = new StreamReader(stream))
				service = parser.ParseDefinition(new NamedText(Path.GetFileName(fileName), reader.ReadToEnd()));

			var generator = new AspNetGenerator
			{
				GeneratorName = "AspNetGeneratorTests",
			};
			generator.GenerateOutput(service);
		}
	}
}
