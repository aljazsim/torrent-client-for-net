using System;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient
{
    /// <summary>
    /// The torrent hashing event arguments.
    /// </summary>
    public sealed class TorrentHashingEventArgs : EventArgs
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentHashingEventArgs" /> class.
        /// </summary>
        /// <param name="torrentInfo">The torrent information.</param>
        public TorrentHashingEventArgs(TorrentInfo torrentInfo)
        {
            torrentInfo.CannotBeNull();

            this.TorrentInfo = torrentInfo;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="TorrentHashingEventArgs"/> class from being created.
        /// </summary>
        private TorrentHashingEventArgs()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the torrent information.
        /// </summary>
        /// <value>
        /// The torrent information.
        /// </value>
        public TorrentInfo TorrentInfo
        {
            get;
            private set;
        }

        #endregion Public Properties
    }
}
