namespace Facility.CodeGen.AspNet
{
	/// <summary>
	/// Used to determine which version of the framework the code should be built for.
	/// </summary>
	public enum TargetFramework
	{
		/// <summary>
		/// Target ASP.NET Web API 2.
		/// </summary>
		WebApi,

		/// <summary>
		/// Build for ASP.NET Core 2.0.
		/// </summary>
		Core,
	}
}
