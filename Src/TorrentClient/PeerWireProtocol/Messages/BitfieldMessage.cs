using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol.Messages
{
    /// <summary>
    /// The bit field message.
    /// </summary>
    public class BitFieldMessage : PeerMessage
    {
        #region Public Fields

        /// <summary>
        /// The message unique identifier.
        /// </summary>
        public const byte MessageId = 5;

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// The message unique identifier length in bytes.
        /// </summary>
        private const int MessageIdLength = 1;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int MessageLengthLength = 4;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private readonly int messageLength;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private readonly int payloadLength;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitFieldMessage" /> class.
        /// </summary>
        /// <param name="bitField">The bit field.</param>
        public BitFieldMessage(bool[] bitField)
        {
            bitField.CannotBeNull();

            this.payloadLength = (int)Math.Ceiling((decimal)bitField.Length / (decimal)8);
            this.messageLength = MessageIdLength + this.payloadLength;

            this.BitField = bitField;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitFieldMessage" /> class.
        /// </summary>
        /// <param name="pieceCount">The piece count.</param>
        /// <param name="missingPieces">The missing pieces.</param>
        public BitFieldMessage(long pieceCount, IEnumerable<int> missingPieces)
        {
            pieceCount.MustBeGreaterThan(0);
            missingPieces.CannotBeNull();

            this.BitField = new bool[pieceCount];

            for (int i = 0; i < pieceCount; i++)
            {
                this.BitField[i] = true;
            }

            foreach (var missingPiece in missingPieces)
            {
                if (missingPiece < pieceCount)
                {
                    this.BitField[missingPiece] = false;
                }
            }

            this.payloadLength = (int)Math.Ceiling((decimal)this.BitField.Length / (decimal)8);
            this.messageLength = MessageIdLength + this.payloadLength;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the bit field.
        /// </summary>
        /// <value>
        /// The bit field.
        /// </value>
        public bool[] BitField
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the length information bytes.
        /// </summary>
        /// <value>
        /// The length information bytes.
        /// </value>
        public override int Length
        {
            get
            {
                return MessageLengthLength + MessageIdLength + this.payloadLength;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Decodes the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offsetFrom">The offset from.</param>
        /// <param name="offsetTo">The offset to.</param>
        /// <param name="message">The message.</param>
        /// <param name="isIncomplete">if set to <c>true</c> the message is incomplete.</param>
        /// <returns>
        /// True if decoding was successful; false otherwise.
        /// </returns>
        public static bool TryDecode(byte[] buffer, ref int offsetFrom, int offsetTo, out BitFieldMessage message, out bool isIncomplete)
        {
            int messageLength;
            byte messageId;
            byte[] payload;

            message = null;
            isIncomplete = false;

            if (buffer != null &&
                buffer.Length > offsetFrom + MessageLengthLength + MessageIdLength &&
                offsetFrom >= 0 &&
                offsetFrom < buffer.Length &&
                offsetTo >= offsetFrom &&
                offsetTo <= buffer.Length)
            {
                messageLength = Message.ReadInt(buffer, ref offsetFrom);
                messageId = Message.ReadByte(buffer, ref offsetFrom);

                if (messageLength > 0 &&
                    messageId == MessageId)
                {
                    if (offsetFrom + messageLength - MessageIdLength <= offsetTo)
                    {
                        payload = Message.ReadBytes(buffer, ref offsetFrom, messageLength - MessageIdLength);

                        if (payload.IsNotNullOrEmpty() &&
                            payload.Length == messageLength - MessageIdLength)
                        {
                            message = new BitFieldMessage(new BitArray(payload).Cast<bool>().ToArray());
                        }
                    }
                    else
                    {
                        isIncomplete = true;
                    }
                }
            }

            return message != null;
        }

        /// <summary>
        /// Encodes the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The number of encoded bits.</returns>
        public override int Encode(byte[] buffer, int offset)
        {
            byte[] byteField = new byte[this.payloadLength];
            int written = offset;

            new BitArray(this.BitField).CopyTo(byteField, 0);

            Message.Write(buffer, ref written, this.messageLength);
            Message.Write(buffer, ref written, MessageId);
            Message.Write(buffer, ref written, byteField);

            return this.CheckWritten(written - offset);
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
            BitFieldMessage bf = obj as BitFieldMessage;

            if (bf == null)
            {
                return false;
            }
            else
            {
                return this.BitField.Equals(bf.BitField);
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
            return this.BitField.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb;

            sb = new StringBuilder();

            for (int i = 0; i < this.BitField.Length; i++)
            {
                sb.Append(this.BitField[i] ? "1" : "0");
            }

            return "BitfieldMessage: Bitfield = " + sb.ToString();
        }

        #endregion Public Methods
    }
}
