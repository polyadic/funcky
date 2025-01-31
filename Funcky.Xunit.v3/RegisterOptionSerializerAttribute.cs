using System.ComponentModel;
using Funcky.Xunit.Serializers;
using Xunit.Sdk;

namespace Funcky.Xunit;

[AttributeUsage(AttributeTargets.Assembly)]
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class RegisterOptionSerializerAttribute : Attribute, IRegisterXunitSerializerAttribute
{
    Type IRegisterXunitSerializerAttribute.SerializerType => typeof(OptionSerializer);

    Type[] IRegisterXunitSerializerAttribute.SupportedTypesForSerialization => [typeof(IOption)];
}
