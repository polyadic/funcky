namespace Funcky.Test.Extensions;

public sealed class NoStream : Stream
{
    public override bool CanRead
        => false;

    public override bool CanSeek
        => false;

    public override bool CanWrite
        => false;

    public override long Length
        => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override int ReadTimeout { get; set; } = 300;

    public override int WriteTimeout { get; set; } = 600;

    public override void Flush() => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin)
        => throw new NotSupportedException();

    public override void SetLength(long value)
        => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();
}
