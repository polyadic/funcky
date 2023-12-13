#if !SET_CURRENT_STACK_TRACE_SUPPORTED
#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Funcky.Monads;

internal static class ExceptionUtilities
{
    private static readonly Func<Exception, StackTrace, Exception> SetStackTraceAction = CompileSetStackTrace();

    public static void SetStackTrace(this Exception target, StackTrace stackTrace) => SetStackTraceAction(target, stackTrace);

    // Adapted from: https://stackoverflow.com/a/63685720
    private static Func<Exception, StackTrace, Exception> CompileSetStackTrace()
    {
        var traceFormatType = typeof(StackTrace).GetNestedType("TraceFormat", BindingFlags.NonPublic)!;
        var toString = typeof(StackTrace).GetMethod("ToString", BindingFlags.NonPublic | BindingFlags.Instance, null, [traceFormatType], null)!;
        var stackTraceStringField = typeof(Exception).GetField("_stackTraceString", BindingFlags.NonPublic | BindingFlags.Instance)!;

        var target = Expression.Parameter(typeof(Exception));
        var stackTrace = Expression.Parameter(typeof(StackTrace));
        var normalTraceFormat = Enum.GetValues(traceFormatType).GetValue(0);
        var stackTraceString = Expression.Call(stackTrace, toString, Expression.Constant(normalTraceFormat, traceFormatType));
        var assign = Expression.Assign(Expression.Field(target, stackTraceStringField), stackTraceString);

        return Expression.Lambda<Func<Exception, StackTrace, Exception>>(Expression.Block(assign, target), target, stackTrace).Compile();
    }
}
#endif
