namespace Facility.CodeGen.AspNet
{
	/// <summary>
	/// Used to determine which version of ASP.NET the code should be built for.
	/// </summary>
	public enum AspNetFramework
	{
		/// <summary>
		/// Target ASP.NET Web API 2.
		/// </summary>
		WebApi,

		/// <summary>
		/// Target ASP.NET Core 2.0.
		/// </summary>
		Core,
	}
}
