using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Xunit.Analyzers;
using Verify = CSharpVerifier<Xunit.Suppressors.ConsiderCallingConfigureAwaitSuppressor>;

public sealed class NullPropSetInInitializeAsyncSuppressorTests
{
	[Theory]
	[InlineData("Fact")]
	[InlineData("FactAttribute")]
	[InlineData("Theory")]
	[InlineData("TheoryAttribute")]
	public async Task StandardTestMethod_Suppresses(string attribute)
	{
		var valueProp = "Hello World";
		var code = string.Format(/* lang=c#-test */ """
			using System.Threading.Tasks;
			using Xunit;

			public class TestClass : Xunit.IAsyncLifetime
			{{
			    private SomeClass _someClass;

			    public Task DisposeAsync()
			    {{
			        return Task.CompletedTask;
			    }}

			    public Task InitializeAsync()
			    {{
			        _someClass = new() {{ SomeProperty = [{1}] }};
			        return Task.CompletedTask;
			    }}

			    [{0}] public void TestMethod() {{ }}
			}}

			class SomeClass
			{{
			    public string SomeProperty {{ get; set; }} = default!;
			}}
			""", attribute, valueProp);
		var expected = DiagnosticResult.CompilerWarning("CS8618").WithLocation(0).WithIsSuppressed(true);

		await Verify.VerifySuppressor(code, CodeAnalysisNetAnalyzers.CS8618(), expected);
	}
}
