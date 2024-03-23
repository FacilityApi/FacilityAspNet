using Facility.Definition;
using Facility.Definition.CodeGen;

namespace Facility.CodeGen.AspNet;

internal static class CSharpUtility
{
	public static void WriteFileHeader(CodeWriter code, string generatorName) =>
		code.WriteLine("// " + CodeGenUtility.GetCodeGenComment(generatorName));

	public static void WriteCodeGenAttribute(CodeWriter code, string generatorName) =>
		code.WriteLine($"[System.CodeDom.Compiler.GeneratedCode(\"{generatorName}\", \"\")]");

	public static void WriteObsoleteAttribute(CodeWriter code, ServiceElementWithAttributesInfo element)
	{
		if (element.IsObsolete)
			code.WriteLine("[Obsolete]");
	}

	public static void WriteUsings(CodeWriter code, IEnumerable<string> namespaceNames, string namespaceName)
	{
		var sortedNamespaceNames = namespaceNames.Distinct().Where(x => namespaceName != x && !namespaceName.StartsWith(x + ".", StringComparison.Ordinal)).ToList();
		sortedNamespaceNames.Sort(CompareUsings);
		if (sortedNamespaceNames.Count != 0)
		{
			foreach (var namepaceName in sortedNamespaceNames)
				code.WriteLine("using " + namepaceName + ";");
			code.WriteLine();
		}
	}

	public const string FileExtension = ".g.cs";

	public static string GetNamespaceName(ServiceInfo serviceInfo) =>
		serviceInfo.TryGetAttribute("csharp")?.TryGetParameterValue("namespace") ?? CodeGenUtility.Capitalize(serviceInfo.Name);

	private static int CompareUsings(string left, string right)
	{
		var leftGroup = GetUsingGroup(left);
		var rightGroup = GetUsingGroup(right);
		var result = leftGroup.CompareTo(rightGroup);
		return result != 0 ? result : string.CompareOrdinal(left, right);
	}

	private static int GetUsingGroup(string namespaceName) =>
		namespaceName == "System" || namespaceName.StartsWith("System.", StringComparison.Ordinal) ? 1 : 2;
}
