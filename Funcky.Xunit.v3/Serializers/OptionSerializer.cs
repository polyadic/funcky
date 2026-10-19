using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit.Sdk;
using static Funcky.Discard;

namespace Funcky.Xunit.Serializers;

internal sealed class OptionSerializer : IXunitSerializer
{
    private static readonly MethodInfo GenericDeserialize
        = typeof(OptionSerializer).GetMethod(nameof(Deserialize), BindingFlags.Static | BindingFlags.NonPublic)
          ?? throw new MissingMethodException("Generic deserialize method not found");

    private static readonly MethodInfo GenericIsSerializable
        = typeof(OptionSerializer).GetMethod(nameof(IsSerializable), BindingFlags.Static | BindingFlags.NonPublic)
          ?? throw new MissingMethodException("Generic IsSerializable method not found");

    private static readonly MethodInfo GenericSerialize
        = typeof(OptionSerializer).GetMethod(nameof(Serialize), BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new MissingMethodException("Generic serialize method not found");

    public object Deserialize(Type type, string serializedValue)
    {
        var itemType = GetItemTypeOrThrow(type);
        return GenericDeserialize.MakeGenericMethod(itemType).Invoke(null, [serializedValue])!;
    }

    public bool IsSerializable(Type type, object? value, [NotNullWhen(false)] out string? failureReason)
    {
        if (GetItemType(type) is not [var itemType])
        {
            failureReason = $"{type} is not an Option<T>";
            return false;
        }

        if ((bool)GenericIsSerializable.MakeGenericMethod(itemType).Invoke(null, [itemType, value])!)
        {
            failureReason = null;
            return true;
        }

        failureReason = value is null
            ? $"The value is not an {type}"
            : $"The item of the {type} is not serializable";
        return false;
    }

    public string Serialize(object value)
    {
        var itemType = GetItemTypeOrThrow(value.GetType());
        return (string)GenericSerialize.MakeGenericMethod(itemType).Invoke(null, [value])!;
    }

    private static Type GetItemTypeOrThrow(Type eitherType)
        => GetItemType(eitherType).GetOrElse(() => throw new InvalidOperationException($"{eitherType} is not an Option<T>"));

    private static Option<Type> GetItemType(Type optionType)
        => optionType.IsGenericType && optionType.GetGenericTypeDefinition() == typeof(Option<>)
            ? optionType.GenericTypeArguments.First()
            : Option<Type>.None;

    private static object Deserialize<TItem>(string serializedValue)
        where TItem : notnull
        => __ switch
        {
            _ when serializedValue == Tag.None => Option<TItem>.None,
            _ when serializedValue.StripPrefix(Tag.Some) is [var rest] => Option.Some(SerializationHelper.Instance.Deserialize<TItem>(rest)!),
            _ => throw new FormatException($"'{serializedValue}' is not a valid option value"),
        };

    private static bool IsSerializable<TItem>(Type itemType, object? value)
        where TItem : notnull
        => value is Option<TItem> option
           && option.Match(none: true, some: item => SerializationHelper.Instance.IsSerializable(item, itemType));

    private static string Serialize<TItem>(object obj)
        where TItem : notnull
    {
        var option = (Option<TItem>)obj;
        return option.Match(
            none: Tag.None,
            some: item => $"{Tag.Some}{SerializationHelper.Instance.Serialize(item)}");
    }

    private static class Tag
    {
        public const string None = "N";
        public const string Some = "S:";
    }
}
