using System;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;
using TorrentClient.TrackerProtocol.Udp.Messages.Messages;

namespace TorrentClient.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The connect message.
    /// </summary>
    public class ConnectMessage : TrackerMessage
    {
        #region Private Fields

        /// <summary>
        /// The action length in bytes.
        /// </summary>
        private const int ActionLength = 4;

        /// <summary>
        /// The default connection identifier
        /// </summary>
        private const long ConnectionId = 0x41727101980;

        /// <summary>
        /// The connection identifier length in bytes.
        /// </summary>
        private const int ConnectionIdLength = 8;

        /// <summary>
        /// The transaction identifier length in bytes.
        /// </summary>
        private const int TransactionIdLength = 4;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectMessage" /> class.
        /// </summary>
        /// <param name="transactionId">The transaction identifier.</param>
        public ConnectMessage(int transactionId)
            : base(TrackingAction.Connect, transactionId)
        {
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="ConnectMessage"/> class from being created.
        /// </summary>
        private ConnectMessage()
            : this(DateTime.UtcNow.GetHashCode())
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
                return ConnectionIdLength + ActionLength + TransactionIdLength;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Decodes the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// True if decoding was successful; false otherwise.
        /// </returns>
        public static bool TryDecode(byte[] buffer, int offset, out ConnectMessage message)
        {
            long connectionId;
            int action;
            int transactionId;

            message = null;

            if (buffer != null &&
                buffer.Length >= offset + ConnectionIdLength + ActionLength + TransactionIdLength &&
                offset >= 0)
            {
                connectionId = Message.ReadLong(buffer, ref offset);
                action = Message.ReadInt(buffer, ref offset);
                transactionId = Message.ReadInt(buffer, ref offset);

                if (connectionId == ConnectMessage.ConnectionId &&
                    action == (int)TrackingAction.Connect &&
                    transactionId >= 0)
                {
                    message = new ConnectMessage(transactionId);
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

            Message.Write(buffer, ref written, ConnectMessage.ConnectionId);
            Message.Write(buffer, ref written, (int)this.Action);
            Message.Write(buffer, ref written, this.TransactionId);

            return this.CheckWritten(written - offset);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "UdpTrackerConnectMessage";
        }

        #endregion Public Methods
    }
}
