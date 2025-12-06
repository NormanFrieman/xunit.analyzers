using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit.Analyzers;

namespace Xunit.Suppressors;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NullPropSetInInitializeAsyncSuppressor : XunitDiagnosticSuppressor
{
	public NullPropSetInInitializeAsyncSuppressor() :
		base(Descriptors.CS8618_Suppression)
	{ }

	protected override bool ShouldSuppress(
		Diagnostic diagnostic,
		SuppressionAnalysisContext context,
		XunitContext xunitContext)
	{
		return true;
	}
}
