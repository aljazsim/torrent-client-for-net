using System.Text;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol.Messages
{
    /// <summary>
    /// Represents a "Have" message.
    /// </summary>
    public class HaveMessage : PeerMessage
    {
        #region Public Fields

        /// <summary>
        /// The message unique identifier.
        /// </summary>
        public const byte MessageId = 4;

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// The message unique identifier length in bytes.
        /// </summary>
        private const int MessageIdLength = 1;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int MessageLength = 5;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int MessageLengthLength = 4;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int PayloadLength = 4;

        /// <summary>
        /// The piece index.
        /// </summary>
        private int pieceIndex;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HaveMessage"/> class.
        /// </summary>
        /// <param name="pieceIndex">Index of the piece.</param>
        public HaveMessage(int pieceIndex)
        {
            pieceIndex.MustBeGreaterThanOrEqualTo(0);

            this.pieceIndex = pieceIndex;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="HaveMessage"/> class from being created.
        /// </summary>
        private HaveMessage()
        {
        }

        #endregion Private Constructors

        #region Public Properties

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
            get
            {
                return this.pieceIndex;
            }
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
        public static bool TryDecode(byte[] buffer, ref int offsetFrom, int offsetTo, out HaveMessage message, out bool isIncomplete)
        {
            int messageLength;
            byte messageId;
            int payload;

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
                payload = Message.ReadInt(buffer, ref offsetFrom);

                if (messageLength == MessageLength &&
                    messageId == MessageId &&
                    payload >= 0)
                {
                    if (offsetFrom <= offsetTo)
                    {
                        message = new HaveMessage(payload);
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
            Message.Write(buffer, ref written, this.pieceIndex);

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
            HaveMessage msg = obj as HaveMessage;

            if (msg == null)
            {
                return false;
            }
            else
            {
                return this.pieceIndex == msg.pieceIndex;
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
            return this.pieceIndex.GetHashCode();
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
            sb.Append("HaveMessage: ");
            sb.Append($"Index = {this.pieceIndex}");

            return sb.ToString();
        }

        #endregion Public Methods
    }
}
