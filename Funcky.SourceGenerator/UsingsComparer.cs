using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.SourceGenerator;

internal class UsingsComparer : IEqualityComparer<UsingDirectiveSyntax>
{
    public bool Equals(UsingDirectiveSyntax left, UsingDirectiveSyntax right)
    => left.Name.ToFullString() == right.Name.ToFullString();

    public int GetHashCode(UsingDirectiveSyntax usingDirectiveSyntax)
    => usingDirectiveSyntax.Name.ToFullString().GetHashCode();
}
