using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(byte), nameof(byte.TryParse))]
[OrNoneFromTryPattern(typeof(sbyte), nameof(sbyte.TryParse))]
[OrNoneFromTryPattern(typeof(short), nameof(short.TryParse))]
[OrNoneFromTryPattern(typeof(ushort), nameof(ushort.TryParse))]
[OrNoneFromTryPattern(typeof(int), nameof(int.TryParse))]
[OrNoneFromTryPattern(typeof(uint), nameof(uint.TryParse))]
[OrNoneFromTryPattern(typeof(long), nameof(long.TryParse))]
[OrNoneFromTryPattern(typeof(ulong), nameof(ulong.TryParse))]
[OrNoneFromTryPattern(typeof(float), nameof(float.TryParse))]
[OrNoneFromTryPattern(typeof(double), nameof(double.TryParse))]
[OrNoneFromTryPattern(typeof(decimal), nameof(decimal.TryParse))]
public static partial class ParseExtensions;
