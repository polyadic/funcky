#if !SET_CURRENT_STACK_TRACE_SUPPORTED
#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Funcky.Monads;

/// <summary>
/// Sets the stack trace of an exception on frameworks without <c>ExceptionDispatchInfo.SetCurrentStackTrace</c>
/// by writing the private <c>_stackTraceString</c> field through a compiled expression.
/// </summary>
/// <remarks>
/// The private members are an implementation detail of the runtime and expression compilation is not available
/// everywhere, so the setter is resolved defensively: when anything is missing or fails, setting the stack trace
/// becomes a no-op instead of failing every <c>Result.Error</c> with a <see cref="TypeInitializationException"/>.
/// </remarks>
internal static class ExceptionUtilities
{
    private static readonly Action<Exception, StackTrace> SetStackTraceAction = TryCompileSetStackTrace().GetOrElse(NoOperation);

    public static void SetStackTrace(this Exception target, StackTrace stackTrace) => SetStackTraceAction(target, stackTrace);

    // Adapted from: https://stackoverflow.com/a/63685720
    private static Option<Action<Exception, StackTrace>> TryCompileSetStackTrace()
    {
        try
        {
            return
                from traceFormatType in Option.FromNullable(typeof(StackTrace).GetNestedType("TraceFormat", BindingFlags.NonPublic))
                from toString in Option.FromNullable(typeof(StackTrace).GetMethod("ToString", BindingFlags.NonPublic | BindingFlags.Instance, null, [traceFormatType], null))
                from stackTraceStringField in Option.FromNullable(typeof(Exception).GetField("_stackTraceString", BindingFlags.NonPublic | BindingFlags.Instance))
                where stackTraceStringField.FieldType == typeof(string)
                select CompileSetStackTrace(traceFormatType, toString, stackTraceStringField);
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            return Option<Action<Exception, StackTrace>>.None;
        }
    }

    private static Action<Exception, StackTrace> CompileSetStackTrace(Type traceFormatType, MethodInfo toString, FieldInfo stackTraceStringField)
    {
        var target = Expression.Parameter(typeof(Exception));
        var stackTrace = Expression.Parameter(typeof(StackTrace));
        var normalTraceFormat = Enum.GetValues(traceFormatType).GetValue(0);
        var stackTraceString = Expression.Call(stackTrace, toString, Expression.Constant(normalTraceFormat, traceFormatType));
        var assign = Expression.Assign(Expression.Field(target, stackTraceStringField), stackTraceString);

        return Expression.Lambda<Action<Exception, StackTrace>>(assign, target, stackTrace).Compile();
    }
}
#endif
