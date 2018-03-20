using DefensiveProgrammingFramework;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;
using TorrentClient.TrackerProtocol.Udp.Messages.Messages;

namespace TorrentClient.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The tracker message base.
    /// </summary>
    public abstract class TrackerMessage : Message
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackerMessage"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="transactionId">The transaction unique identifier.</param>
        public TrackerMessage(TrackingAction action, int transactionId)
        {
            transactionId.MustBeGreaterThanOrEqualTo(0);

            this.Action = action;
            this.TransactionId = transactionId;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public TrackingAction Action
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the transaction unique identifier.
        /// </summary>
        /// <value>
        /// The transaction unique identifier.
        /// </value>
        public int TransactionId
        {
            get;
            protected set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Tries to decode the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="message">The decoded message.</param>
        /// <returns>
        /// True if message was successfully decoded; false otherwise.
        /// </returns>
        public static bool TryDecode(byte[] buffer, int offset, MessageType messageType, out TrackerMessage message)
        {
            int action;

            message = null;

            if (buffer.IsNotNullOrEmpty())
            {
                action = messageType == MessageType.Request ? Message.ReadInt(buffer, ref offset) : Message.ReadInt(buffer, ref offset);
                offset = 0;

                if (action == (int)TrackingAction.Connect)
                {
                    if (messageType == MessageType.Request)
                    {
                        ConnectMessage message2;
                        ConnectMessage.TryDecode(buffer, offset, out message2);

                        message = message2;
                    }
                    else
                    {
                        ConnectResponseMessage message2;
                        ConnectResponseMessage.TryDecode(buffer, offset, out message2);

                        message = message2;
                    }
                }
                else if (action == (int)TrackingAction.Announce)
                {
                    if (messageType == MessageType.Request)
                    {
                        AnnounceMessage message2;
                        AnnounceMessage.TryDecode(buffer, offset, out message2);

                        message = message2;
                    }
                    else
                    {
                        AnnounceResponseMessage message2;
                        AnnounceResponseMessage.TryDecode(buffer, offset, out message2);

                        message = message2;
                    }
                }
                else if (action == (int)TrackingAction.Scrape)
                {
                    if (messageType == MessageType.Request)
                    {
                        ScrapeMessage message2;
                        ScrapeMessage.TryDecode(buffer, offset, out message2);

                        message = message2;
                    }
                    else
                    {
                        ScrapeResponseMessage message2;
                        ScrapeResponseMessage.TryDecode(buffer, offset, out message2);

                        message = message2;
                    }
                }
                else if (action == (int)TrackingAction.Error)
                {
                    ErrorMessage message2;
                    ErrorMessage.TryDecode(buffer, offset, out message2);

                    message = message2;
                }
                else
                {
                    // could not decode UDP message
                }
            }

            return message != null;
        }

        #endregion Public Methods
    }
}
