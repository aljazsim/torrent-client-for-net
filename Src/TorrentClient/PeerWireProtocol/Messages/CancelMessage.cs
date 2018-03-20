using System.Text;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol.Messages
{
    /// <summary>
    /// The cancel message.
    /// </summary>
    public class CancelMessage : PeerMessage
    {
        #region Public Fields

        /// <summary>
        /// The message unique identifier.
        /// </summary>
        public const byte MessageId = 8;

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// The message unique identifier length in bytes.
        /// </summary>
        private const int MessageIdLength = 1;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int MessageLength = 13;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int MessageLengthLength = 4;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int PayloadLength = 12;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelMessage" /> class.
        /// </summary>
        /// <param name="pieceIndex">Index of the piece.</param>
        /// <param name="blockOffset">The block offset.</param>
        /// <param name="blockLength">Length of the block.</param>
        public CancelMessage(int pieceIndex, int blockOffset, int blockLength)
        {
            pieceIndex.MustBeGreaterThanOrEqualTo(0);
            blockOffset.MustBeGreaterThanOrEqualTo(0);
            blockLength.MustBeGreaterThanOrEqualTo(0);

            this.PieceIndex = pieceIndex;
            this.BlockOffset = blockOffset;
            this.BlockLength = blockLength;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="CancelMessage"/> class from being created.
        /// </summary>
        private CancelMessage()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the length of the block.
        /// </summary>
        /// <value>
        /// The length of the block.
        /// </value>
        public int BlockLength
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the block offset.
        /// </summary>
        /// <value>
        /// The block offset.
        /// </value>
        public int BlockOffset
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the length in bytes.
        /// </summary>
        /// <value>
        /// The length in bytes.
        /// </value>
        public override int Length
        {
            get
            {
                return MessageLengthLength + MessageIdLength + PayloadLength;
            }
        }

        /// <summary>
        /// Gets the index of the piece.
        /// </summary>
        /// <value>
        /// The index of the piece.
        /// </value>
        public int PieceIndex
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Decodes the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offsetFrom">The offset.</param>
        /// <param name="offsetTo">The offset to.</param>
        /// <param name="message">The message.</param>
        /// <param name="isIncomplete">if set to <c>true</c> the message is incomplete.</param>
        /// <returns>
        /// True if decoding was successful; false otherwise.
        /// </returns>
        public static bool TryDecode(byte[] buffer, ref int offsetFrom, int offsetTo, out CancelMessage message, out bool isIncomplete)
        {
            int messageLength;
            byte messageId;
            int pieceIndex;
            int blockOffset;
            int blockLength;

            message = null;
            isIncomplete = false;

            if (buffer != null &&
                buffer.Length >= offsetFrom + MessageLengthLength + MessageIdLength + PayloadLength &&
                offsetFrom >= 0 &&
                offsetTo >= offsetFrom &&
                offsetTo <= buffer.Length)
            {
                messageLength = Message.ReadInt(buffer, ref offsetFrom);
                messageId = Message.ReadByte(buffer, ref offsetFrom);
                pieceIndex = Message.ReadInt(buffer, ref offsetFrom);
                blockOffset = Message.ReadInt(buffer, ref offsetFrom);
                blockLength = Message.ReadInt(buffer, ref offsetFrom);

                if (messageLength == MessageLength &&
                    messageId == MessageId &&
                    pieceIndex >= 0 &&
                    blockOffset >= 0 &&
                    blockLength >= 0)
                {
                    if (offsetFrom <= offsetTo)
                    {
                        message = new CancelMessage(pieceIndex, blockOffset, blockLength);
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
        /// <returns>
        /// The encoded peer message.
        /// </returns>
        public override int Encode(byte[] buffer, int offset)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThan(buffer.Length);

            int written = offset;

            Message.Write(buffer, ref written, MessageLength);
            Message.Write(buffer, ref written, MessageId);
            Message.Write(buffer, ref written, this.PieceIndex);
            Message.Write(buffer, ref written, this.BlockOffset);
            Message.Write(buffer, ref written, this.BlockLength);

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
            CancelMessage msg = obj as CancelMessage;

            return msg == null ? false : this.PieceIndex == msg.PieceIndex &&
                                         this.BlockOffset == msg.BlockOffset &&
                                         this.BlockLength == msg.BlockLength;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.PieceIndex.GetHashCode() ^
                   this.BlockLength.GetHashCode() ^
                   this.BlockOffset.GetHashCode();
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

            sb = new System.Text.StringBuilder();
            sb.Append("CancelMessage: ");
            sb.Append($"PieceIndex = {this.PieceIndex}, ");
            sb.Append($"BlockOffset = {this.BlockOffset}, ");
            sb.Append($"BlockLength = {this.BlockLength}");

            return sb.ToString();
        }

        #endregion Public Methods
    }
}
