using System;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.PeerWireProtocol
{
    /// <summary>
    /// The peer message received event arguments.
    /// </summary>
    public sealed class PeerMessgeReceivedEventArgs : EventArgs
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PeerMessgeReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PeerMessgeReceivedEventArgs(PeerMessage message)
        {
            message.CannotBeNull();

            this.Message = message;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public PeerMessage Message
        {
            get;
            private set;
        }

        #endregion Public Properties
    }
}
