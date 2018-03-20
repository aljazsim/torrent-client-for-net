using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol.Messages
{
    /// <summary>
    /// The un-choke message.
    /// </summary>
    public class UnchokeMessage : PeerMessage
    {
        #region Public Fields

        /// <summary>
        /// The message unique identifier.
        /// </summary>
        public const byte MessageId = 1;

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// The message unique identifier length in bytes.
        /// </summary>
        private const int MessageIdLength = 1;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int MessageLength = 1;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int MessageLengthLength = 4;

        /// <summary>
        /// The message length in bytes.
        /// </summary>
        private const int PayloadLength = 0;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnchokeMessage"/> class.
        /// </summary>
        public UnchokeMessage()
        {
        }

        #endregion Public Constructors

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
        public static bool TryDecode(byte[] buffer, ref int offsetFrom, int offsetTo, out UnchokeMessage message, out bool isIncomplete)
        {
            int messageLength;
            byte messageId;

            message = null;
            isIncomplete = false;

            if (buffer != null &&
                buffer.Length >= offsetFrom + MessageLengthLength + MessageIdLength + PayloadLength &&
                offsetFrom >= 0)
            {
                messageLength = Message.ReadInt(buffer, ref offsetFrom);
                messageId = Message.ReadByte(buffer, ref offsetFrom);

                if (messageLength == MessageLength &&
                    messageId == MessageId)
                {
                    if (offsetFrom <= offsetTo)
                    {
                        message = new UnchokeMessage();
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
            return obj is UnchokeMessage;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "UnChokeMessage";
        }

        #endregion Public Methods
    }
}
