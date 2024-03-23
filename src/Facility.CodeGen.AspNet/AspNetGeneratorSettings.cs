using Facility.Definition.CodeGen;

namespace Facility.CodeGen.AspNet;

/// <summary>
/// Settings for generating an ASP.NET controller.
/// </summary>
public sealed class AspNetGeneratorSettings : FileGeneratorSettings
{
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
}
