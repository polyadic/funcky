using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit.Sdk;
using static Funcky.Discard;

namespace Funcky.Xunit.Serializers;

internal sealed class EitherSerializer : IXunitSerializer
{
    private static readonly MethodInfo GenericDeserialize
        = typeof(EitherSerializer).GetMethod(nameof(Deserialize), BindingFlags.Static | BindingFlags.NonPublic)
          ?? throw new MissingMethodException("Generic deserialize method not found");

    private static readonly MethodInfo GenericIsSerializable
        = typeof(EitherSerializer).GetMethod(nameof(IsSerializable), BindingFlags.Static | BindingFlags.NonPublic)
          ?? throw new MissingMethodException("Generic IsSerializable method not found");

    private static readonly MethodInfo GenericSerialize
        = typeof(EitherSerializer).GetMethod(nameof(Serialize), BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new MissingMethodException("Generic serialize method not found");

    public object Deserialize(Type type, string serializedValue)
    {
        var (leftType, rightType) = GetLeftAndRightTypeOrThrow(type);
        return GenericDeserialize.MakeGenericMethod(leftType, rightType).Invoke(null, [serializedValue])!;
    }

    public bool IsSerializable(Type type, object? value, [NotNullWhen(false)] out string? failureReason)
    {
        failureReason = string.Empty;
        return GetLeftAndRightType(type) is [var (leftType, rightType)]
            && (bool)GenericIsSerializable.MakeGenericMethod(leftType, rightType).Invoke(null, [leftType, rightType, value])!;
    }

    public string Serialize(object value)
    {
        var (leftType, rightType) = GetLeftAndRightTypeOrThrow(value.GetType());
        return (string)GenericSerialize.MakeGenericMethod(leftType, rightType).Invoke(null, [value])!;
    }

    private static (Type Left, Type Right) GetLeftAndRightTypeOrThrow(Type eitherType)
        => GetLeftAndRightType(eitherType).GetOrElse(() => throw new InvalidOperationException($"{eitherType} is not an Either<L, R>"));

    private static Option<(Type Left, Type Right)> GetLeftAndRightType(Type eitherType)
        => eitherType.IsGenericType && eitherType.GetGenericTypeDefinition() == typeof(Either<,>)
            ? (eitherType.GenericTypeArguments[0], eitherType.GenericTypeArguments[1])
            : Option<(Type, Type)>.None;

    private static object Deserialize<TLeft, TRight>(string serializedValue)
        where TLeft : notnull
        where TRight : notnull
        => __ switch
        {
            _ when serializedValue.StripPrefix(Tag.Left) is [var rest]
                => Either<TLeft, TRight>.Left(SerializationHelper.Instance.Deserialize<TLeft>(rest)!),
            _ when serializedValue.StripPrefix(Tag.Right) is [var rest]
                => Either<TLeft, TRight>.Right(SerializationHelper.Instance.Deserialize<TRight>(rest)!),
            _ => throw new FormatException($"'{serializedValue}' is not a valid either value"),
        };

    private static bool IsSerializable<TLeft, TRight>(Type leftType, Type rightType, object? value)
        where TLeft : notnull
        where TRight : notnull
    {
        var either = (Either<TLeft, TRight>)(value ?? throw new InvalidOperationException("Either cannot be null"));
        return either.Match(
            left: left => SerializationHelper.Instance.IsSerializable(left, leftType),
            right: right => SerializationHelper.Instance.IsSerializable(right, rightType));
    }

    private static string Serialize<TLeft, TRight>(object obj)
        where TLeft : notnull
        where TRight : notnull
    {
        var either = (Either<TLeft, TRight>)obj;
        return either.Match(
            left: left => $"{Tag.Left}{SerializationHelper.Instance.Serialize(left)}",
            right: right => $"{Tag.Right}{SerializationHelper.Instance.Serialize(right)}");
    }

    private static class Tag
    {
        public const string Left = "L:";
        public const string Right = "R:";
    }
}
