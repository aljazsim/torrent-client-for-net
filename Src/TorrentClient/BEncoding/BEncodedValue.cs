using System.IO;
using DefensiveProgrammingFramework;
using TorrentClient.Exceptions;
using TorrentClient.Extensions;

namespace TorrentClient.BEncoding
{
    /// <summary>
    /// The Base class for all BEncoded values.
    /// </summary>
    public abstract class BEncodedValue
    {
        #region Public Methods

        /// <summary>
        /// Clones the specified value.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The cloned BEncoded value.</returns>
        public static T Clone<T>(T value) where T : BEncodedValue
        {
            value.CannotBeNull();

            return (T)BEncodedValue.Decode(value.Encode());
        }

        /// <summary>
        /// Interface for all BEncoded values
        /// </summary>
        /// <param name="data">The byte array containing the BEncoded data</param>
        /// <returns>The decoded BEncoded value.</returns>
        public static BEncodedValue Decode(byte[] data)
        {
            data.CannotBeNull();

            BEncodedValue value = null;

            using (MemoryStream ms = new MemoryStream(data))
            {
                using (RawReader stream = new RawReader(ms))
                {
                    value = Decode(stream);
                }
            }

            return value;
        }

        /// <summary>
        /// Decode BEncoded data in the given byte array
        /// </summary>
        /// <param name="buffer">The byte array containing the BEncoded data</param>
        /// <param name="offset">The offset at which the data starts at</param>
        /// <param name="length">The number of bytes to be decoded</param>
        /// <returns>BEncodedValue containing the data that was in the byte[]</returns>
        public static BEncodedValue Decode(byte[] buffer, int offset, int length)
        {
            buffer.CannotBeNull();
            offset.MustBeGreaterThanOrEqualTo(0);
            length.MustBeGreaterThanOrEqualTo(0);

            return Decode(buffer, offset, length, true);
        }

        /// <summary>
        /// Decodes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="strictDecoding">if set to <c>true</c> [strict decoding].</param>
        /// <returns>The decoded BEncoded value.</returns>
        public static BEncodedValue Decode(byte[] buffer, int offset, int length, bool strictDecoding)
        {
            buffer.CannotBeNull();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThanOrEqualTo(buffer.Length - length);
            length.MustBeGreaterThanOrEqualTo(0);

            BEncodedValue value = null;

            using (RawReader reader = new RawReader(new MemoryStream(buffer, offset, length), strictDecoding))
            {
                value = BEncodedValue.Decode(reader);
            }

            return value;
        }

        /// <summary>
        /// Decoded the stream into a BEncoded value.
        /// </summary>
        /// <param name="stream">The stream containing the BEncoded data</param>
        /// <returns>The BEncoded value.</returns>
        public static BEncodedValue Decode(Stream stream)
        {
            stream.CannotBeNull();

            return Decode(new RawReader(stream));
        }

        /// <summary>
        /// Decode BEncoded data in the given RawReader
        /// </summary>
        /// <param name="reader">The RawReader containing the BEncoded data</param>
        /// <returns>BEncodedValue containing the data that was in the stream</returns>
        public static BEncodedValue Decode(RawReader reader)
        {
            reader.CannotBeNull();

            BEncodedValue data;
            int peekByte = reader.PeekByte();

            if (peekByte == 'i')
            {
                // integer
                data = new BEncodedNumber();
                data.DecodeInternal(reader);

                return data;
            }
            else if (peekByte == 'd')
            {
                // dictionary
                data = new BEncodedDictionary();
                data.DecodeInternal(reader);

                return data;
            }
            else if (peekByte == 'l')
            {
                // list
                data = new BEncodedList();
                data.DecodeInternal(reader);

                return data;
            }
            else if (peekByte == '0' ||
                     peekByte == '1' ||
                     peekByte == '2' ||
                     peekByte == '3' ||
                     peekByte == '4' ||
                     peekByte == '5' ||
                     peekByte == '6' ||
                     peekByte == '7' ||
                     peekByte == '8' ||
                     peekByte == '9')
            {
                // string
                data = new BEncodedString();
                data.DecodeInternal(reader);

                return data;
            }
            else
            {
                throw new BEncodingException("Could not find what value to decode.");
            }
        }

        /// <summary>
        /// Decodes the specified data.
        /// </summary>
        /// <typeparam name="T">The BEncoded value type.</typeparam>
        /// <param name="data">The data.</param>
        /// <returns>The BEncoded value.</returns>
        public static T Decode<T>(byte[] data) where T : BEncodedValue
        {
            data.CannotBeNull();

            return (T)BEncodedValue.Decode(data);
        }

        /// <summary>
        /// Decodes the specified buffer.
        /// </summary>
        /// <typeparam name="T">The BEncoded value type.</typeparam>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>The BEncoded value.</returns>
        public static T Decode<T>(byte[] buffer, int offset, int length) where T : BEncodedValue
        {
            buffer.CannotBeNull();
            offset.MustBeGreaterThanOrEqualTo(0);
            length.MustBeGreaterThanOrEqualTo(0);

            return BEncodedValue.Decode<T>(buffer, offset, length, true);
        }

        /// <summary>
        /// Decodes the specified buffer.
        /// </summary>
        /// <typeparam name="T">The BEncoded value type.</typeparam>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="strictDecoding">if set to <c>true</c> [strict decoding].</param>
        /// <returns>
        /// The BEncoded value.
        /// </returns>
        public static T Decode<T>(byte[] buffer, int offset, int length, bool strictDecoding) where T : BEncodedValue
        {
            buffer.CannotBeNull();
            offset.MustBeGreaterThanOrEqualTo(0);
            length.MustBeGreaterThanOrEqualTo(0);

            return (T)BEncodedValue.Decode(buffer, offset, length, strictDecoding);
        }

        /// <summary>
        /// Decodes the specified stream.
        /// </summary>
        /// <typeparam name="T">The BEncoded value type.</typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns>
        /// The BEncoded value.
        /// </returns>
        public static T Decode<T>(Stream stream) where T : BEncodedValue
        {
            stream.CannotBeNull();

            return (T)BEncodedValue.Decode(stream);
        }

        /// <summary>
        /// Decodes the specified reader.
        /// </summary>
        /// <typeparam name="T">The BEncoded value type.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <returns>
        /// The BEncoded value.
        /// </returns>
        public static T Decode<T>(RawReader reader) where T : BEncodedValue
        {
            reader.CannotBeNull();

            return (T)BEncodedValue.Decode(reader);
        }

        /// <summary>
        /// Encodes the BEncodedValue into a byte array.
        /// </summary>
        /// <returns>Byte array containing the BEncoded Data.</returns>
        public byte[] Encode()
        {
            byte[] buffer = new byte[this.LengthInBytes()];

            if (this.Encode(buffer, 0) != buffer.Length)
            {
                throw new BEncodingException("Error encoding the data");
            }

            return buffer;
        }

        /// <summary>
        /// Encodes the BEncodedValue into the supplied buffer
        /// </summary>
        /// <param name="buffer">The buffer to encode the information to</param>
        /// <param name="offset">The offset in the buffer to start writing the data</param>
        /// <returns>The number of bytes encoded.</returns>
        public abstract int Encode(byte[] buffer, int offset);

        /// <summary>
        /// Returns the size of the byte[] needed to encode this BEncodedValue
        /// </summary>
        /// <returns>The length in bytes.</returns>
        public abstract int LengthInBytes();

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Decodes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="strictDecoding">if set to <c>true</c> use strict decoding.</param>
        /// <returns>The b-encoded value.</returns>
        internal static BEncodedValue Decode(byte[] buffer, bool strictDecoding)
        {
            buffer.CannotBeNull();

            return Decode(buffer, 0, buffer.Length, strictDecoding);
        }

        /// <summary>
        /// Decodes the stream into respected value.
        /// </summary>
        /// <param name="reader">The reader.</param>
        internal abstract void DecodeInternal(RawReader reader);

        #endregion Internal Methods
    }
}
