using System.Threading.Tasks;
using Xunit;
using Verify = CSharpVerifier<Xunit.Analyzers.TestMethodShouldNotBeSkipped>;

public class TestMethodShouldNotBeSkippedTests
{
	[Theory]
	[InlineData("Fact")]
	[InlineData("Theory")]
	public async Task NotSkippedTest_DoesNotTrigger(string attribute)
	{
		var source = string.Format(/* lang=c#-test */ """
			public class TestClass {{
			    [Xunit.{0}]
			    public void TestMethod() {{ }}
			}}
			""", attribute);

		await Verify.VerifyAnalyzer(source);
	}

	[Theory]
	[InlineData("Fact")]
	[InlineData("Theory")]
	public async Task SkippedTest_Triggers(string attribute)
	{
		var source = string.Format(/* lang=c#-test */ """
			class TestClass {{
			    [Xunit.{0}([|Skip="Lazy"|])]
			    public void TestMethod() {{ }}
			}}
			""", attribute);

		await Verify.VerifyAnalyzer(source);
	}
}
