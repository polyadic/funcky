using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Funcky.Analyzers.Test;

public static partial class CSharpCodeRefactoringVerifier<TCodeRefactoring>
    where TCodeRefactoring : CodeRefactoringProvider, new()
{
    public class Test : CSharpCodeRefactoringTest<TCodeRefactoring, DefaultVerifier>
    {
        public Test()
        {
            SolutionTransforms.Add((solution, projectId) =>
            {
                var project = solution.GetProject(projectId);
                if (project is not null)
                {
                    var compilationOptions = project.CompilationOptions;
                    if (compilationOptions is not null)
                    {
                        compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
                        solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);
                    }
                }

                return solution;
            });
        }
    }
}
