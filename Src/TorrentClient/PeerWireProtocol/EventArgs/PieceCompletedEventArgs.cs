using System;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol
{
    /// <summary>
    /// The piece completed event arguments.
    /// </summary>
    public sealed class PieceCompletedEventArgs : EventArgs
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PieceCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="pieceIndex">Index of the piece.</param>
        /// <param name="pieceData">The piece data.</param>
        public PieceCompletedEventArgs(int pieceIndex, byte[] pieceData)
        {
            pieceIndex.MustBeGreaterThanOrEqualTo(0);
            pieceData.CannotBeNullOrEmpty();

            this.PieceIndex = pieceIndex;
            this.PieceData = pieceData;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="PieceCompletedEventArgs"/> class from being created.
        /// </summary>
        private PieceCompletedEventArgs()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the piece data.
        /// </summary>
        /// <value>
        /// The piece data.
        /// </value>
        public byte[] PieceData
        {
            get;
            private set;
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
    }
}
