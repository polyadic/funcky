using System.ComponentModel;
using Funcky.Xunit.Serializers;
using Xunit.Sdk;

namespace Funcky.Xunit;

[AttributeUsage(AttributeTargets.Assembly)]
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class RegisterUnitSerializerAttribute : Attribute, IRegisterXunitSerializerAttribute
{
    Type IRegisterXunitSerializerAttribute.SerializerType => typeof(UnitSerializer);

    Type[] IRegisterXunitSerializerAttribute.SupportedTypesForSerialization => [typeof(Unit)];
}
