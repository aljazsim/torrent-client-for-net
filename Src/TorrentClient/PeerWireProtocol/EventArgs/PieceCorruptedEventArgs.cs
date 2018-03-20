using System;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol
{
    /// <summary>
    /// The piece corrupted event arguments.
    /// </summary>
    public sealed class PieceCorruptedEventArgs : EventArgs
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PieceCorruptedEventArgs" /> class.
        /// </summary>
        /// <param name="pieceIndex">Index of the piece.</param>
        public PieceCorruptedEventArgs(int pieceIndex)
        {
            pieceIndex.MustBeGreaterThanOrEqualTo(0);

            this.PieceIndex = pieceIndex;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="PieceCorruptedEventArgs"/> class from being created.
        /// </summary>
        private PieceCorruptedEventArgs()
        {
        }

        #endregion Private Constructors

        #region Public Properties

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
    }
}
