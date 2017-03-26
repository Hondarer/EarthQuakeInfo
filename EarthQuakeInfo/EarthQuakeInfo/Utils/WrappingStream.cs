using System;
using System.IO;

namespace HondarerSoft.Utils
{
    // WrappingStream Implementation
    // https://code.logos.com/blog/2009/05/wrappingstream_implementation.html
    // 素の Stream は Dispose しても、内部バッファを解放せず、参照がある限りメモリを使用しつづけてしまう。
    // 本クラスにより内部バッファを解放する Stream を実現する。
    // http://pierre3.hatenablog.com/entry/2015/10/25/001207

    /// <summary>
    /// A <see cref="Stream"/> that wraps another stream. The major feature of <see cref="WrappingStream"/> is that it does not dispose the
    /// underlying stream when it is disposed; this is useful when using classes such as <see cref="BinaryReader"/> and
    /// <see cref="System.Security.Cryptography.CryptoStream"/> that take ownership of the stream passed to their constructors.
    /// </summary>
    public class WrappingStream : Stream
    {
        /// <summary>
        /// The wrapped stream.
        /// </summary>
        protected Stream streamBase = null;

        /// <summary>
        /// Gets the wrapped stream.
        /// </summary>
        /// <value>The wrapped stream.</value>
        protected Stream WrappedStream
        {
            get
            {
                return streamBase;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value><c>true</c> if the stream supports reading; otherwise, <c>false</c>.</value>
        public override bool CanRead
        {
            get
            {
                if (streamBase == null)
                {
                    return false;
                }

                return streamBase.CanRead;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value><c>true</c> if the stream supports seeking; otherwise, <c>false</c>.</value>
        public override bool CanSeek
        {
            get
            {
                if (streamBase == null)
                {
                    return false;
                }

                return streamBase.CanSeek;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value><c>true</c> if the stream supports writing; otherwise, <c>false</c>.</value>
        public override bool CanWrite
        {
            get
            {
                if (streamBase == null)
                {
                    return false;
                }

                return streamBase.CanWrite;
            }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get
            {
                ThrowIfDisposed();
                return streamBase.Length;
            }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        public override long Position
        {
            get
            {
                ThrowIfDisposed();
                return streamBase.Position;
            }
            set
            {
                ThrowIfDisposed();
                streamBase.Position = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappingStream"/> class.
        /// </summary>
        /// <param name="streamBase">The wrapped stream.</param>
        /// <exception cref="ArgumentNullException"><paramref name="streamBase"/> is null.</exception>
        public WrappingStream(Stream streamBase)
        {
            // check parameters
            if (streamBase == null)
            {
                throw new ArgumentNullException("streamBase");
            }

            this.streamBase = streamBase;
        }

        /// <summary>
        /// Begins an asynchronous read operation.
        /// </summary>
        /// <param name="buffer">The buffer to read the data into.</param>
        /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing data read from the stream.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <param name="callback">An optional asynchronous callback, to be called when the read is complete.</param>
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous read request from other requests.</param>
        /// <returns>An <see cref="IAsyncResult"/> that represents the asynchronous read, which could still be pending.</returns>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            ThrowIfDisposed();
            return streamBase.BeginRead(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Begins an asynchronous write operation.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">The byte offset in <paramref name="buffer"/> from which to begin writing.</param>
        /// <param name="count">The maximum number of bytes to write.</param>
        /// <param name="callback">An optional asynchronous callback, to be called when the write is complete.</param>
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous write request from other requests.</param>
        /// <returns>An <see cref="IAsyncResult"/> that represents the asynchronous write, which could still be pending.</returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            ThrowIfDisposed();
            return streamBase.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Waits for the pending asynchronous read to complete.
        /// </summary>
        /// <param name="asyncResult">The reference to the pending asynchronous request to finish.</param>
        /// <returns>
        /// The number of bytes read from the stream, between zero (0) and the number of bytes you requested.
        /// Streams return zero (0) only at the end of the stream, otherwise, they should block until at least one byte is available.
        /// </returns>
        public override int EndRead(IAsyncResult asyncResult)
        {
            ThrowIfDisposed();
            return streamBase.EndRead(asyncResult);
        }

        /// <summary>
        /// Ends an asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">A reference to the outstanding asynchronous I/O request.</param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            ThrowIfDisposed();
            streamBase.EndWrite(asyncResult);
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            ThrowIfDisposed();
            streamBase.Flush();
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position
        /// within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();
            return streamBase.Read(buffer, offset, count);
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
        /// </summary>
        public override int ReadByte()
        {
            ThrowIfDisposed();
            return streamBase.ReadByte();
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIfDisposed();
            return streamBase.Seek(offset, origin);
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            ThrowIfDisposed();
            streamBase.SetLength(value);
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position
        /// within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from <paramref name="buffer"/> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();
            streamBase.Write(buffer, offset, count);
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        public override void WriteByte(byte value)
        {
            ThrowIfDisposed();
            streamBase.WriteByte(value);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="WrappingStream"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            // doesn't close the base stream, but just prevents access to it through this WrappingStream
            if (disposing == true)
            {
                streamBase = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Throws an ObjectDisposedException if this object has been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (streamBase == null)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
