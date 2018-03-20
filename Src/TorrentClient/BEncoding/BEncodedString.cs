using System;
using System.Globalization;
using System.Text;
using DefensiveProgrammingFramework;
using TorrentClient.Exceptions;
using TorrentClient.Extensions;

namespace TorrentClient.BEncoding
{
    /// <summary>
    /// The BEncoded string.
    /// </summary>
    public class BEncodedString : BEncodedValue, IComparable<BEncodedString>
    {
        #region Private Fields

        /// <summary>
        /// The text bytes.
        /// </summary>
        private byte[] textBytes;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodedString"/> class.
        /// </summary>
        public BEncodedString()
            : this(Array.Empty<byte>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodedString"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public BEncodedString(char[] value)
            : this(Encoding.ASCII.GetBytes(value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodedString"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public BEncodedString(string value)
            : this(Encoding.ASCII.GetBytes(value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodedString"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public BEncodedString(byte[] value)
        {
            value.CannotBeNull();

            this.textBytes = value;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the hexadecimal value of the text.
        /// </summary>
        /// <value>
        /// The hexadecimal value of the text.
        /// </value>
        public string Hex
        {
            get
            {
                return BitConverter.ToString(this.TextBytes);
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get
            {
                return Encoding.ASCII.GetString(this.textBytes);
            }

            set
            {
                this.textBytes = Encoding.ASCII.GetBytes(value);
            }
        }

        /// <summary>
        /// Gets the text bytes.
        /// </summary>
        /// <value>
        /// The text bytes.
        /// </value>
        public byte[] TextBytes
        {
            get
            {
                return this.textBytes;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Checks to see if the contents of two byte arrays are equal
        /// </summary>
        /// <param name="array1">The first array</param>
        /// <param name="array2">The second array</param>
        /// <returns>True if the arrays are equal, false if they aren't.</returns>
        public static bool ByteMatch(byte[] array1, byte[] array2)
        {
            array1.CannotBeNull();
            array2.CannotBeNull();

            if (array1.Length != array2.Length)
            {
                return false;
            }
            else
            {
                return ByteMatch(array1, 0, array2, 0, array1.Length);
            }
        }

        /// <summary>
        /// Checks to see if the contents of two byte arrays are equal
        /// </summary>
        /// <param name="array1">The first array</param>
        /// <param name="offset1">The starting index for the first array</param>
        /// <param name="array2">The second array</param>
        /// <param name="offset2">The starting index for the second array</param>
        /// <param name="count">The number of bytes to check</param>
        /// <returns>
        /// True if arrays match; false otherwise.
        /// </returns>
        public static bool ByteMatch(byte[] array1, int offset1, byte[] array2, int offset2, int count)
        {
            array1.CannotBeNull();
            array2.CannotBeNull();

            // If either of the arrays is too small, they're not equal
            if ((array1.Length - offset1) < count ||
                (array2.Length - offset2) < count)
            {
                return false;
            }

            // Check if any elements are unequal
            for (int i = 0; i < count; i++)
            {
                if (array1[offset1 + i] != array2[offset2 + i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Converts the specified value to BEncoded string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The BEncoded string.</returns>
        public static BEncodedString ToBEncodedString(string value)
        {
            value.CannotBeNullOrEmpty();

            return new BEncodedString(value);
        }

        /// <summary>
        /// Converts the specified value to BEncoded string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The BEncoded string.</returns>
        public static BEncodedString ToBEncodedString(char[] value)
        {
            value.CannotBeNullOrEmpty();

            return new BEncodedString(value);
        }

        /// <summary>
        /// Converts the specified value to BEncoded string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The BEncoded string.</returns>
        public static BEncodedString ToBEncodedString(byte[] value)
        {
            value.CannotBeNullOrEmpty();

            return new BEncodedString(value);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(object other)
        {
            return this.CompareTo(other as BEncodedString);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(BEncodedString other)
        {
            int difference;
            int length;

            if (other == null)
            {
                return 1;
            }
            else
            {
                difference = 0;
                length = this.textBytes.Length > other.textBytes.Length ? other.textBytes.Length : this.textBytes.Length;

                for (int i = 0; i < length; i++)
                {
                    if ((difference = this.textBytes[i].CompareTo(other.textBytes[i])) != 0)
                    {
                        return difference;
                    }
                }

                if (this.textBytes.Length == other.textBytes.Length)
                {
                    return 0;
                }
                else
                {
                    return this.textBytes.Length > other.textBytes.Length ? 1 : -1;
                }
            }
        }

        /// <summary>
        /// Encodes the BEncodedString to a byte[] using the supplied Encoding
        /// </summary>
        /// <param name="buffer">The buffer to encode the string to</param>
        /// <param name="offset">The offset at which to save the data to</param>
        /// <returns>
        /// The number of bytes encoded
        /// </returns>
        public override int Encode(byte[] buffer, int offset)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);

            int written;

            written = offset;
            written += this.WriteAscii(buffer, written, this.textBytes.Length.ToString(CultureInfo.InvariantCulture));
            written += this.WriteAscii(buffer, written, ":");
            written += this.Write(buffer, written, this.textBytes);

            return written - offset;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            BEncodedString other;

            if (obj == null)
            {
                return false;
            }
            else
            {
                if (obj is string)
                {
                    other = new BEncodedString((string)obj);
                }
                else if (obj is BEncodedString)
                {
                    other = (BEncodedString)obj;
                }
                else
                {
                    return false;
                }

                return ByteMatch(this.textBytes, other.textBytes);
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 0;

            for (int i = 0; i < this.textBytes.Length; i++)
            {
                hash += this.textBytes[i];
            }

            return hash;
        }

        /// <summary>
        /// Returns the size of the byte[] needed to encode this BEncodedValue
        /// </summary>
        /// <returns>The length in bytes.</returns>
        public override int LengthInBytes()
        {
            // The length is equal to the length-prefix + ':' + length of data
            int prefix = 1; // Account for ':'

            // Count the number of characters needed for the length prefix
            for (int i = this.textBytes.Length; i != 0; i = i / 10)
            {
                prefix += 1;
            }

            if (this.textBytes.Length == 0)
            {
                prefix++;
            }

            return prefix + this.textBytes.Length;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Encoding.ASCII.GetString(this.textBytes);
        }

        /// <summary>
        /// Writes the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="destinationOffset">The destination offset.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourceOffset">The source offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>The number of written bytes.</returns>.
        public int Write(byte[] destination, int destinationOffset, byte[] source, int sourceOffset, int count)
        {
            destination.CannotBeNullOrEmpty();
            destinationOffset.MustBeGreaterThanOrEqualTo(0);
            source.CannotBeNull();
            sourceOffset.MustBeGreaterThanOrEqualTo(0);
            count.MustBeGreaterThanOrEqualTo(0);

            Buffer.BlockCopy(source, sourceOffset, destination, destinationOffset, count);

            return count;
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>The number of written bytes.</returns>.
        public int Write(byte[] buffer, int offset, byte[] value)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            value.CannotBeNull();

            return this.Write(buffer, offset, value, 0, value.Length);
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>The number of written bytes.</returns>.
        public int Write(byte[] buffer, int offset, byte value)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);

            buffer[offset] = value;

            return 1;
        }

        /// <summary>
        /// Writes the ASCII.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="text">The text.</param>
        /// <returns>The number of characters written.</returns>
        public int WriteAscii(byte[] buffer, int offset, string text)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            text.CannotBeNullOrEmpty();

            for (int i = 0; i < text.Length; i++)
            {
                this.Write(buffer, offset + i, (byte)text[i]);
            }

            return text.Length;
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Decodes a BEncodedString from the supplied StreamReader
        /// </summary>
        /// <param name="reader">The StreamReader containing the BEncodedString</param>
        internal override void DecodeInternal(RawReader reader)
        {
            reader.CannotBeNull();

            int letterCount;
            string length = string.Empty;

            // read in how many characters the string is long
            while (reader.PeekByte() != -1 &&
                   reader.PeekByte() != ':')
            {
                length += (char)reader.ReadByte();
            }

            if (reader.ReadByte() != ':')
            {
                throw new BEncodingException("Invalid data found. Aborting");
            }

            if (!int.TryParse(length, out letterCount))
            {
                throw new BEncodingException("Invalid BEncodedString. Could not read the string length.");
            }

            this.textBytes = new byte[letterCount];

            if (reader.Read(this.textBytes, 0, letterCount) != letterCount)
            {
                throw new BEncodingException("Invalid BEncodedString. The string does not match the specified length.");
            }
        }

        #endregion Internal Methods
    }
}
