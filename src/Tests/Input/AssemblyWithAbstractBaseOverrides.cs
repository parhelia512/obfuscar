using System;
using System.IO;

namespace TestClasses
{
    // Internal class overriding abstract members from external base class (System.IO.Stream)
    // Reproduces issue #549: Obfuscar renames overridden methods in .NET Core but not in .NET Framework
    internal class InternalStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => 0;
        public override long Position { get; set; }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count) => 0;

        public override long Seek(long offset, SeekOrigin origin) => 0;

        public override void SetLength(long value) { }

        public override void Write(byte[] buffer, int offset, int count) { }
    }

    public static class StreamFactory
    {
        public static Stream CreateStream() => new InternalStream();
    }
}
