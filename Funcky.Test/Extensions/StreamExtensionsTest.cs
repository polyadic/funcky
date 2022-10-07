using FsCheck;
using FsCheck.Xunit;

namespace Funcky.Test.Extensions;

public sealed class StreamExtensionsTest
{
    [Fact]
    public void ReadingPastTheEndOfAMemoryStreamReturnsNone()
    {
        using var stream = new MemoryStream();

        FunctionalAssert.None(stream.ReadByteOrNone());
    }

    [Property]
    public Property ReadingAByteWithReadByteOrNoneWorksCorrectly(byte testByte)
    {
        using var stream = new MemoryStream();

        stream.WriteByte(testByte);
        stream.Position = 0;

        return (stream.ReadByteOrNone() == testByte).ToProperty();
    }

    [Fact]
    public void GetLengthOrNoneReturnsNoneIfTheLengthIsNotSupportedAndTheLengthOtherwise()
    {
        using var stream = new MemoryStream();
        FunctionalAssert.Some(0, stream.GetLengthOrNone());

        using var nostream = new NoStream();
        FunctionalAssert.None(nostream.GetLengthOrNone());
    }

    [Fact]
    public void GetPositionOrNoneReturnsNoneIfThePositionIsNotSupportedAndThePositionOtherwise()
    {
        using var stream = new MemoryStream();
        FunctionalAssert.Some(0, stream.GetPositionOrNone());

        using var nostream = new NoStream();
        FunctionalAssert.None(nostream.GetPositionOrNone());
    }

    [Fact]
    public void GetReadTimeoutOrNoneReturnsNoneIfTheReadTimeoutIsNotSupportedAndTheReadTimeoutOtherwise()
    {
        using var memoryStream = new MemoryStream();
        FunctionalAssert.None(memoryStream.GetReadTimeoutOrNone());

        using var nostream = new NoStream();
        FunctionalAssert.Some(300, nostream.GetReadTimeoutOrNone());
    }

    [Fact]
    public void GetWriteTimeoutOrNoneReturnsNoneIfTheWriteTimeoutIsNotSupportedAndTheWriteTimeoutOtherwise()
    {
        using var memoryStream = new MemoryStream();
        FunctionalAssert.None(memoryStream.GetReadTimeoutOrNone());

        using var nostream = new NoStream();
        FunctionalAssert.Some(600, nostream.GetWriteTimeoutOrNone());
    }
}
