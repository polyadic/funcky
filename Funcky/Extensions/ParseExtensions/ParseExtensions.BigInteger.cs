using System.Numerics;
using Funcky.Internal;

namespace Funcky.Extensions;

[OrNoneFromTryPattern(typeof(BigInteger), nameof(BigInteger.TryParse))]
public static partial class ParseExtensions;
