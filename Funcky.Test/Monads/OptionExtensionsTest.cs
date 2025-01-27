using FsCheck;
using FsCheck.Fluent;
using Funcky.FsCheck;

namespace Funcky.Test.Monads;

public class OptionExtensionsTest
{
    public enum Reason
    {
        UserNotFound,
        NoFirstName,
    }

    [Fact]
    public void WithToEitherYouCanConvertAnOptionToAnEitherWithAnIndividualLeftIfOptionIsNone()
    {
        var noUser = GetFirstNameFromUser(GetNoUser);
        var emptyUser = GetFirstNameFromUser(GetEmptyUser);
        var fullUser = GetFirstNameFromUser(GetCompleteUser);

        noUser.Switch(left: reason => Assert.Equal(Reason.UserNotFound, reason), right: _ => Assert.Fail("failed"));
        emptyUser.Switch(left: reason => Assert.Equal(Reason.NoFirstName, reason), right: _ => Assert.Fail("failed"));
        fullUser.Switch(left: _ => Assert.Fail("failed"), right: firstName => Assert.Equal("Name", firstName));
    }

    [FunckyProperty]
    public Property AnOptionWithAValueTypeCanBeConvertedToANullable(Option<int> input)
    {
        var nullable = input.ToNullable();

        return input.Match(
            none: () => nullable is null,
            some: value => nullable == value)
            .ToProperty();
    }

    [FunckyProperty]
    public Property AnOptionWithAReferenceTypeCanBeConvertedToNullableReferences(Option<string> input)
    {
        var nullable = input.ToNullable();

        return input.Match(
                none: () => nullable is null,
                some: value => nullable == value)
            .ToProperty();
    }

    private static Either<Reason, string> GetFirstNameFromUser(Func<Option<User>> getUser)
        => getUser()
            .ToEither(Reason.UserNotFound)
            .SelectMany(GetFirstName);

    private static Either<Reason, string> GetFirstName(User user)
        => user.FirstName
            .ToEither(() => Reason.NoFirstName);

    private static Option<User> GetNoUser() => Option<User>.None;

    private static Option<User> GetEmptyUser() => new User(Option<string>.None);

    private static Option<User> GetCompleteUser() => new User("Name");

    private record User(Option<string> FirstName);
}
