using System.Collections.Generic;
using System.Linq;

namespace Funcky.SourceGenerator.VariadicFunctions
{
    internal sealed class NoOperationTemplate : IVariadicFunctionTemplate
    {
        public string FormatTypeParameterName(int index, int arity) => $"T{index + 1}";

        public string FormatParameterName(int index, int arity) => $"p{index + 1}";

        public string FormatLeadingTrivia(IEnumerable<(string TypeParameterName, string ParameterName)> parameters)
            => "[System.Diagnostics.Contracts.Pure]\n";

        public string FormatModifiers() => "public static";

        public string FormatMethodName() => "NoOperation";

        public string FormatReturnType(IEnumerable<(string TypeParameterName, string ParameterName)> parameters) => "void";

        public IEnumerable<string> FormatParameters(IEnumerable<(string TypeParameterName, string ParameterName)> parameters)
            => parameters.Select(p => $"{p.TypeParameterName} {p.ParameterName}");

        public string FormatBody(IEnumerable<(string TypeParameterName, string ParameterName)> parameters) => "{ }";
    }
}
