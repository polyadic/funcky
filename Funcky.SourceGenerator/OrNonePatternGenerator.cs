using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Funcky.Money.SourceGenerator
{
    [Generator]
    public sealed class OrNonePatternGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
            => _ = TryMethods()
                .Aggregate(context, AddSource);

        private GeneratorExecutionContext AddSource(GeneratorExecutionContext context, TryPattern pattern)
        {
            context.AddSource(pattern.FileName, pattern.Source);

            return context;
        }

        private IEnumerable<TryPattern> TryMethods()
            => Enumerable.Empty<TryPattern>();
    }
}
