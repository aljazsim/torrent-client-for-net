using System;
using System.Globalization;
using DefensiveProgrammingFramework;
using TorrentClient.Exceptions;
using TorrentClient.Extensions;

namespace TorrentClient.BEncoding
{
    /// <summary>
    /// The BEncoded number.
    /// </summary>
    public class BEncodedNumber : BEncodedValue, IComparable<BEncodedNumber>
    {
        #region Private Fields

        /// <summary>
        /// The number.
        /// </summary>
        private long number;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodedNumber"/> class.
        /// </summary>
        public BEncodedNumber()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodedNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public BEncodedNumber(long value)
        {
            this.number = value;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public long Number
        {
            get
            {
                return this.number;
            }

            set
            {
                this.number = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Converts the specified value to BEncoded number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The BEncoded number.</returns>
        public static BEncodedNumber ToBEncodedNumber(long value)
        {
            return new BEncodedNumber(value);
        }

        /// <summary>
        /// Converts the specified value to BEncoded number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The BEncoded number.</returns>
        public static BEncodedNumber ToBEncodedNumber(int value)
        {
            return new BEncodedNumber(value);
        }

        /// <summary>
        /// Compares the automatic.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(object other)
        {
            if (other is BEncodedNumber ||
                other is long ||
                other is int)
            {
                return this.CompareTo((BEncodedNumber)other);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(BEncodedNumber other)
        {
            other.CannotBeNull();

            return this.number.CompareTo(other.number);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(long other)
        {
            return this.number.CompareTo(other);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(int other)
        {
            return this.number.CompareTo(other);
        }

        /// <summary>
        /// Encodes this number to the supplied byte[] starting at the supplied offset
        /// </summary>
        /// <param name="buffer">The buffer to write the data to</param>
        /// <param name="offset">The offset to start writing the data at</param>
        /// <returns>The number of bytes encoded.</returns>
        public override int Encode(byte[] buffer, int offset)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);

            long number = this.number;
            int written = offset;
            long reversed;

            buffer[written++] = (byte)'i';

            if (number < 0)
            {
                buffer[written++] = (byte)'-';
                number = -number;
            }

            // Reverse the number '12345' to get '54321'
            reversed = 0;

            for (long i = number; i != 0; i /= 10)
            {
                reversed = (reversed * 10) + (i % 10);
            }

            // Write each digit of the reversed number to the array. We write '1'
            // first, then '2', etc
            for (long i = reversed; i != 0; i /= 10)
            {
                buffer[written++] = (byte)((i % 10) + '0');
            }

            if (number == 0)
            {
                buffer[written++] = (byte)'0';
            }

            // If the original number ends in one or more zeros, they are lost
            // when we reverse the number. We add them back in here.
            for (long i = number; i % 10 == 0 && number != 0; i /= 10)
            {
                buffer[written++] = (byte)'0';
            }

            buffer[written++] = (byte)'e';

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
            BEncodedNumber obj2 = obj as BEncodedNumber;

            if (obj2 == null)
            {
                return false;
            }
            else
            {
                return this.number == obj2.number;
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
            return this.number.GetHashCode();
        }

        /// <summary>
        /// Returns the length of the encoded string in bytes
        /// </summary>
        /// <returns>The length in bytes.</returns>
        public override int LengthInBytes()
        {
            long number = this.number;
            int count = 2; // account for the 'i' and 'e'

            if (number == 0)
            {
                return count + 1;
            }

            if (number < 0)
            {
                number = -number;
                count++;
            }

            for (long i = number; i != 0; i /= 10)
            {
                count++;
            }

            return count;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.number.ToString(CultureInfo.InvariantCulture);
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Decodes a BEncoded number from the supplied RawReader
        /// </summary>
        /// <param name="reader">RawReader containing a BEncoded Number</param>
        internal override void DecodeInternal(RawReader reader)
        {
            reader.CannotBeNull();

            int sign = 1;
            int letter;

            if (reader.ReadByte() != 'i')
            {
                throw new BEncodingException("Invalid data found. Aborting.");
            }

            if (reader.PeekByte() == '-')
            {
                sign = -1;
                reader.ReadByte();
            }

            while ((letter = reader.PeekByte()) != -1 &&
                    letter != 'e')
            {
                if (letter < '0' ||
                    letter > '9')
                {
                    throw new BEncodingException("Invalid number found.");
                }

                this.number = (this.number * 10) + (letter - '0');

                reader.ReadByte();
            }

            if (reader.ReadByte() != 'e')
            {
                throw new BEncodingException("Invalid data found. Aborting.");
            }

            this.number *= sign;
        }

        #endregion Internal Methods
    }
}
