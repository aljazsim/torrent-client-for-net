using System;
using System.Net;
using System.Text;
using DefensiveProgrammingFramework;
using TorrentClient.Exceptions;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol.Messages
{
    /// <summary>
    /// The message base class.
    /// </summary>
    public abstract class Message
    {
        #region Public Fields

        /// <summary>
        /// The byte length in bytes.
        /// </summary>
        public const int ByteLength = 1;

        /// <summary>
        /// The integer length in bytes.
        /// </summary>
        public const int IntLength = 4;

        /// <summary>
        /// The long length in bytes.
        /// </summary>
        public const int LongLength = 8;

        /// <summary>
        /// The short length in bytes.
        /// </summary>
        public const int ShortLength = 2;

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// Gets the length in bytes.
        /// </summary>
        /// <value>
        /// The length in bytes.
        /// </value>
        public abstract int Length
        {
            get;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Copies the bytes from the specified source array to the specified destination array.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="sourceOffset">The source offset.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="destinationOffset">The destination offset.</param>
        /// <param name="count">The count.</param>
        public static void Copy(byte[] source, int sourceOffset, byte[] destination, ref int destinationOffset, int count)
        {
            destination.CannotBeNullOrEmpty();
            destinationOffset.MustBeGreaterThanOrEqualTo(0);
            destinationOffset.MustBeLessThan(destination.Length);
            source.CannotBeNullOrEmpty();
            sourceOffset.MustBeGreaterThanOrEqualTo(0);
            sourceOffset.MustBeLessThan(source.Length);
            count.MustBeLessThanOrEqualTo(destination.Length - destinationOffset);
            count.MustBeLessThanOrEqualTo(source.Length - sourceOffset);

            Buffer.BlockCopy(source, sourceOffset, destination, destinationOffset, count);

            destinationOffset += count;
        }

        /// <summary>
        /// Converts peer id to binary array.
        /// </summary>
        /// <param name="peerId">The peer identifier.</param>
        /// <returns>The peer id binary array.</returns>
        public static byte[] FromPeerId(string peerId)
        {
            peerId.MustBe(x => x.Contains("-", StringComparison.InvariantCulture));

            int delimiterIndex;

            delimiterIndex = peerId.LastIndexOf('-');

            peerId = Encoding.ASCII.GetBytes(peerId.Substring(0, delimiterIndex + 1)).ToHexaDecimalString() + // client id
                     peerId.Substring(delimiterIndex + 1); // random number

            peerId.Length.MustBeEqualTo(40);

            return peerId.ToByteArray();
        }

        /// <summary>
        /// Reads the byte form the buffer at the specified offset by advancing the offset afterwards.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The read byte.</returns>
        public static byte ReadByte(byte[] buffer, ref int offset)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThanOrEqualTo(buffer.Length - ByteLength);

            byte value;

            value = Buffer.GetByte(buffer, offset);

            offset++;

            return value;
        }

        /// <summary>
        /// Reads the byte form the buffer starting at the specified offset for the specified count by advancing the offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>
        /// The array of read bytes
        /// </returns>
        public static byte[] ReadBytes(byte[] buffer, ref int offset, int count)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThanOrEqualTo(buffer.Length - count);
            count.MustBeGreaterThanOrEqualTo(0);
            count.MustBeLessThanOrEqualTo(buffer.Length - offset);

            byte[] result = new byte[count];

            Buffer.BlockCopy(buffer, offset, result, 0, count);

            offset += count;

            return result;
        }

        /// <summary>
        /// Reads the endpoint from the buffer starting at the specified offset by advancing the offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The endpoint.</returns>
        public static IPEndPoint ReadEndpoint(byte[] buffer, ref int offset)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThanOrEqualTo(buffer.Length - IntLength - ShortLength);

            byte[] ipaddress = new byte[4];
            int port;

            ipaddress = ReadBytes(buffer, ref offset, 4);
            port = (ushort)ReadShort(buffer, ref offset);

            return new IPEndPoint(new IPAddress(ipaddress), port);
        }

        /// <summary>
        /// Reads the integer form the buffer starting at the specified offset by advancing the offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The integer.</returns>
        public static int ReadInt(byte[] buffer, ref int offset)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThanOrEqualTo(buffer.Length - IntLength);

            int value;

            value = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, offset));

            offset += IntLength;

            return value;
        }

        /// <summary>
        /// Reads the long number form the buffer starting at the specified offset by advancing the offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The long number.</returns>
        public static long ReadLong(byte[] buffer, ref int offset)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThanOrEqualTo(buffer.Length - LongLength);

            long result;

            result = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buffer, offset));

            offset += LongLength;

            return result;
        }

        /// <summary>
        /// Reads the short number form the buffer starting at the specified offset by advancing the offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The short number.</returns>
        public static short ReadShort(byte[] buffer, ref int offset)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThan(buffer.Length);

            short value;

            value = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, offset));

            offset += ShortLength;

            return value;
        }

        /// <summary>
        /// Reads the string form the buffer starting at the specified offset for the specified count by advancing the offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>
        /// The string.
        /// </returns>
        public static string ReadString(byte[] buffer, ref int offset, int count)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThan(buffer.Length);
            count.MustBeGreaterThanOrEqualTo(0);
            count.MustBeLessThanOrEqualTo(buffer.Length - offset);

            string value;

            value = Encoding.ASCII.GetString(buffer, offset, count);

            offset += count;

            return value;
        }

        /// <summary>
        /// Reads the peer identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The peer identifier.
        /// </returns>
        public static string ToPeerId(byte[] value)
        {
            value.CannotBeNullOrEmpty();
            value.Length.MustBeEqualTo(20);

            int delimiterIndex = -1;
            int offset = 0;
            string peerId = null;

            for (int i = 0; i < value.Length; i++)
            {
                if ((char)value[i] == '-')
                {
                    delimiterIndex = i;
                }
            }

            if (delimiterIndex > 0)
            {
                peerId = ReadString(value, ref offset, delimiterIndex + 1) + // client id
                         ReadBytes(value, ref offset, value.Length - delimiterIndex - 1).ToHexaDecimalString(); // random number
            }
            else
            {
                peerId = ReadBytes(value, ref offset, value.Length).ToHexaDecimalString(); // could not interpret peer id -> read as binary string
            }

            return peerId;
        }

        /// <summary>
        /// Writes the specified byte value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        public static void Write(byte[] buffer, ref int offset, byte value)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThan(buffer.Length);

            Buffer.SetByte(buffer, offset, value);

            offset += ByteLength;
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        public static void Write(byte[] buffer, ref int offset, ushort value)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThanOrEqualTo(buffer.Length - ShortLength);

            Write(buffer, ref offset, (short)value);
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        public static void Write(byte[] buffer, ref int offset, short value)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThanOrEqualTo(buffer.Length - ShortLength);

            Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value)), 0, buffer, ref offset, ShortLength);
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        public static void Write(byte[] buffer, ref int offset, IPAddress value)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThan(buffer.Length);
            value.CannotBeNull();

            Write(buffer, ref offset, value.GetAddressBytes());
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        public static void Write(byte[] buffer, ref int offset, IPEndPoint value)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThan(buffer.Length);
            value.CannotBeNull();

            Write(buffer, ref offset, value.Address);
            Write(buffer, ref offset, (ushort)value.Port);
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        public static void Write(byte[] buffer, ref int offset, int value)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThanOrEqualTo(buffer.Length - IntLength);

            Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value)), 0, buffer, ref offset, IntLength);
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        public static void Write(byte[] buffer, ref int offset, uint value)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThanOrEqualTo(buffer.Length - IntLength);

            Write(buffer, ref offset, (int)value);
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        public static void Write(byte[] buffer, ref int offset, long value)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThanOrEqualTo(buffer.Length - LongLength);

            Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value)), 0, buffer, ref offset, LongLength);
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        public static void Write(byte[] buffer, ref int offset, ulong value)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThanOrEqualTo(buffer.Length - LongLength);

            Write(buffer, ref offset, (long)value);
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        public static void Write(byte[] buffer, ref int offset, byte[] value)
        {
            value.CannotBeNull();
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThanOrEqualTo(buffer.Length - value.Length);

            Copy(value, 0, buffer, ref offset, value.Length);
        }

        /// <summary>
        /// Writes the value to the buffer at the specified offset.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        public static void Write(byte[] buffer, ref int offset, string value)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThan(buffer.Length);

            byte[] data;

            data = Encoding.ASCII.GetBytes(value);

            Copy(data, 0, buffer, ref offset, data.Length);
        }

        /// <summary>
        /// Encodes the message.
        /// </summary>
        /// <returns>The byte array.</returns>
        public byte[] Encode()
        {
            byte[] buffer = new byte[this.Length];

            this.Encode(buffer, 0);

            return buffer;
        }

        /// <summary>
        /// Encodes the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The number of encoded bytes.</returns>
        public abstract int Encode(byte[] buffer, int offset);

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Checks the written.
        /// </summary>
        /// <param name="written">The written.</param>
        /// <returns>The written byte count.</returns>
        protected int CheckWritten(int written)
        {
            if (written != this.Length)
            {
                throw new MessageException("Message encoded incorrectly. Incorrect number of bytes written");
            }
            else
            {
                return written;
            }
        }

        #endregion Protected Methods
    }
}
