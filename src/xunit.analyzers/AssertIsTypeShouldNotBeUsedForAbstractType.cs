﻿using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Xunit.Analyzers
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class AssertIsTypeShouldNotBeUsedForAbstractType : AssertUsageAnalyzerBase
	{
		private const string AbstractClass = "abstract class";
		private const string Interface = "interface";

		private static HashSet<string> IsTypeMethods { get; } = new HashSet<string>(new[] { "IsType", "IsNotType" });

		public AssertIsTypeShouldNotBeUsedForAbstractType()
			: base(Descriptors.X2018_AssertIsTypeShouldNotBeUsedForAbstractType, IsTypeMethods)
		{ }

		protected override void Analyze(OperationAnalysisContext context, IInvocationOperation invocationOperation, InvocationExpressionSyntax invocation, IMethodSymbol method)
		{
			if (!(invocation.GetSimpleName() is GenericNameSyntax genericName))
				return;

			var typeSyntax = genericName.TypeArgumentList.Arguments[0];
			var typeInfo = context.GetSemanticModel().GetTypeInfo(typeSyntax);
			var typeKind = GetAbstractTypeKind(typeInfo.Type);
			if (typeKind == null)
				return;

			var typeName = SymbolDisplay.ToDisplayString(typeInfo.Type);

			context.ReportDiagnostic(
				Diagnostic.Create(
					Descriptors.X2018_AssertIsTypeShouldNotBeUsedForAbstractType,
					invocation.GetLocation(),
					typeKind,
					typeName));
		}

		private static string GetAbstractTypeKind(ITypeSymbol typeSymbol)
		{
			switch (typeSymbol.TypeKind)
			{
				case TypeKind.Class:
					if (typeSymbol.IsAbstract)
						return AbstractClass;
					break;

				case TypeKind.Interface:
					return Interface;
			}

			return null;
		}
	}
}
