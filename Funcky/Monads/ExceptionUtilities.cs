using System;
using System.Diagnostics;
using System.Reflection;

namespace Funcky.Monads
{
    internal static class ExceptionUtilities
    {
        private static readonly FieldInfo STACK_TRACE_STRING_FI = typeof(Exception).GetField("_stackTraceString", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly Type TRACE_FORMAT_TI = Type.GetType("System.Diagnostics.StackTrace").GetNestedType("TraceFormat", BindingFlags.NonPublic);
        private static readonly MethodInfo TRACE_TO_STRING_MI = typeof(StackTrace).GetMethod("ToString", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { TRACE_FORMAT_TI }, null);

        public static Exception SetStackTrace(this Exception target, StackTrace stack)
        {
            var getStackTraceString = TRACE_TO_STRING_MI.Invoke(stack, new object[] { Enum.GetValues(TRACE_FORMAT_TI).GetValue(0) });
            STACK_TRACE_STRING_FI.SetValue(target, getStackTraceString);
            return target;
        }
    }
}
