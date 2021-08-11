using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzer
{
    public sealed class SyntaxMatcher
    {
        private const string NamespaceSeparator = ".";
        private readonly SyntaxNodeAnalysisContext _analysisContext;

        public SyntaxMatcher(SyntaxNodeAnalysisContext analysisContext)
            => _analysisContext = analysisContext;

        private InvocationExpressionSyntax InvocationExpr => (InvocationExpressionSyntax)_analysisContext.Node;

        private SemanticModel SemanticModel => _analysisContext.SemanticModel;

        public bool MatchStaticCall(string fullTypeName, string methodName)
            => _analysisContext.SemanticModel.GetOperation(InvocationExpr) is IInvocationOperation { TargetMethod: { } method }
                && method.Name == methodName
                && FullTypeName(method) == fullTypeName;

        public bool MatchArgument<TArgument>(int argumentPosition, TArgument argumentValue)
            => GetArgument(argumentPosition) is { } argument
                && _analysisContext.SemanticModel.GetConstantValue(argument.Expression, _analysisContext.CancellationToken) is { HasValue: true, Value: var constantValue }
                && constantValue is TArgument value
                && value.Equals(argumentValue);

        public string GetArgumentAsString(int argumentPosition)
            => GetArgument(argumentPosition) is { } argument
                ? argument.ToString()
                : throw new NullReferenceException($"GetArgument({argumentPosition}) returned null.");

        public string GetArgumentType(int argumentPosition)
            => GetArgument(argumentPosition) is { } argument
                ? SemanticModel.GetTypeInfo(argument.Expression).Type.ToDisplayString()
                : throw new NullReferenceException($"GetArgument({argumentPosition}) returned null.");

        private ArgumentSyntax? GetArgument(int argumentPosition)
            => InvocationExpr.ArgumentList is ArgumentListSyntax argumentList
                && argumentList.Arguments.Count > argumentPosition
                    ? argumentList.Arguments[argumentPosition]
                    : null;

        private static string FullTypeName(IMethodSymbol method)
            => string.Join(NamespaceSeparator, NamespaceParts(method.ContainingNamespace).Append(method.ContainingType.Name));

        private static IEnumerable<string> NamespaceParts(INamespaceSymbol namespaceSymbol)
            => namespaceSymbol.ContainingNamespace.IsGlobalNamespace
                ? Enumerable.Repeat(namespaceSymbol.Name, 1)
                : NamespaceParts(namespaceSymbol.ContainingNamespace).Append(namespaceSymbol.Name);
    }
}
