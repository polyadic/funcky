using System.ComponentModel;
using Funcky.Xunit.Serializers;
using Xunit.Sdk;

namespace Funcky.Xunit;

[AttributeUsage(AttributeTargets.Assembly)]
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class RegisterEitherSerializerAttribute : Attribute, IRegisterXunitSerializerAttribute
{
    Type IRegisterXunitSerializerAttribute.SerializerType => typeof(EitherSerializer);

    Type[] IRegisterXunitSerializerAttribute.SupportedTypesForSerialization => [typeof(IEither)];
}
