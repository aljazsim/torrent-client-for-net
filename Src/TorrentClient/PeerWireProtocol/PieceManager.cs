using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol
{
    /// <summary>
    /// The piece manager.
    /// </summary>
    public sealed class PieceManager : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// The checkout timeout.
        /// </summary>
        private readonly TimeSpan checkoutTimeout = TimeSpan.FromSeconds(120);

        /// <summary>
        /// The timer timeout.
        /// </summary>
        private readonly TimeSpan timerTimeout = TimeSpan.FromSeconds(10);

        /// <summary>
        /// The check out piece index / check out time dictionary.
        /// </summary>
        private Dictionary<int, DateTime> checkouts;

        /// <summary>
        /// The thread locker.
        /// </summary>
        private object locker = new object();

        /// <summary>
        /// The piece hashes.
        /// </summary>
        private IEnumerable<string> pieceHashes;

        /// <summary>
        /// The pieces count.
        /// </summary>
        private int piecesCount = 0;

        /// <summary>
        /// The present pieces count.
        /// </summary>
        private int presentPiecesCount = 0;

        /// <summary>
        /// The checkout timer.
        /// </summary>
        private System.Timers.Timer timer;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PieceManager" /> class.
        /// </summary>
        /// <param name="torrentInfoHash">The torrent information hash.</param>
        /// <param name="torrentLength">Length of the torrent.</param>
        /// <param name="pieceHashes">The piece hashes.</param>
        /// <param name="pieceLength">Length of the piece.</param>
        /// <param name="blockLength">Length of the block.</param>
        /// <param name="bitField">The bit field.</param>
        public PieceManager(string torrentInfoHash, long torrentLength, IEnumerable<string> pieceHashes, long pieceLength, int blockLength, PieceStatus[] bitField)
        {
            torrentInfoHash.CannotBeNullOrEmpty();
            pieceHashes.CannotBeNullOrEmpty();
            pieceLength.MustBeGreaterThan(0);
            ((long)blockLength).MustBeLessThanOrEqualTo(pieceLength);
            (pieceLength % blockLength).MustBeEqualTo(0);
            bitField.CannotBeNull();
            bitField.Length.MustBeEqualTo(pieceHashes.Count());

            this.PieceLength = pieceLength;
            this.BlockLength = blockLength;
            this.BlockCount = (int)(pieceLength / blockLength);

            this.TorrentInfoHash = torrentInfoHash;
            this.TorrentLength = torrentLength;

            this.pieceHashes = pieceHashes;

            this.BitField = bitField;

            for (int i = 0; i < this.BitField.Length; i++)
            {
                if (this.BitField[i] != PieceStatus.Ignore)
                {
                    this.piecesCount++;
                }

                if (bitField[i] == PieceStatus.Present)
                {
                    this.presentPiecesCount++;
                }
            }

            this.checkouts = new Dictionary<int, DateTime>();

            // setup checkout timer
            this.timer = new System.Timers.Timer();
            this.timer.Interval = this.timerTimeout.TotalMilliseconds;
            this.timer.Elapsed += this.Timer_Elapsed;
            this.timer.Enabled = true;
            this.timer.Start();
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="PieceManager"/> class from being created.
        /// </summary>
        private PieceManager()
        {
        }

        #endregion Private Constructors

        #region Public Events

        /// <summary>
        /// Occurs when piece has completed.
        /// </summary>
        public event EventHandler<PieceCompletedEventArgs> PieceCompleted;

        /// <summary>
        /// Occurs when block is requested.
        /// </summary>
        public event EventHandler<PieceRequestedEventArgs> PieceRequested;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets the bit field.
        /// </summary>
        /// <value>
        /// The bit field.
        /// </value>
        public PieceStatus[] BitField
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the block count.
        /// </summary>
        /// <value>
        /// The block count.
        /// </value>
        public int BlockCount
        {
            get;
            private set;
        }

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
        /// Gets the completed percentage.
        /// </summary>
        /// <value>
        /// The completed percentage.
        /// </value>
        public decimal CompletedPercentage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether all pieces are complete.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all pieces are complete; otherwise, <c>false</c>.
        /// </value>
        public bool IsComplete
        {
            get
            {
                this.CheckIfObjectIsDisposed();

                return this.presentPiecesCount == this.piecesCount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the object is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if object is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether it is end game (only 5% pieces is missing).
        /// </summary>
        /// <value>
        ///   <c>true</c> if it is end game; otherwise, <c>false</c>.
        /// </value>
        public bool IsEndGame
        {
            get
            {
                this.CheckIfObjectIsDisposed();

                return this.CompletedPercentage >= 0.95m;
            }
        }

        /// <summary>
        /// Gets the piece count.
        /// </summary>
        /// <value>
        /// The piece count.
        /// </value>
        public int PieceCount
        {
            get
            {
                this.CheckIfObjectIsDisposed();

                return this.BitField.Length;
            }
        }

        /// <summary>
        /// Gets the length of the piece.
        /// </summary>
        /// <value>
        /// The length of the piece.
        /// </value>
        public long PieceLength
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the torrent information hash.
        /// </summary>
        /// <value>
        /// The torrent information hash.
        /// </value>
        public string TorrentInfoHash
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the length of the torrent.
        /// </summary>
        /// <value>
        /// The length of the torrent.
        /// </value>
        public long TorrentLength
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Checks out the piece.
        /// </summary>
        /// <param name="pieceIndex">Index of the piece.</param>
        /// <param name="pieceData">The piece data.</param>
        /// <param name="bitField">The bit field.</param>
        /// <returns>
        /// The piece.
        /// </returns>
        public Piece CheckOut(int pieceIndex, byte[] pieceData = null, bool[] bitField = null)
        {
            pieceIndex.MustBeGreaterThanOrEqualTo(0);
            pieceIndex.MustBeLessThanOrEqualTo(this.PieceCount);
            pieceData.IsNotNull().Then(() => pieceData.LongLength.MustBeEqualTo(this.GetPieceLength(pieceIndex)));
            bitField.IsNotNull().Then(() => bitField.Length.MustBeEqualTo(this.GetBlockCount(pieceIndex)));

            this.CheckIfObjectIsDisposed();

            Piece piece = null;
            string hash;
            long pieceLength = this.PieceLength;
            int blockCount = this.BlockCount;

            if (pieceData != null)
            {
                Array.Clear(pieceData, 0, pieceData.Length);
            }

            if (bitField != null)
            {
                Array.Clear(bitField, 0, bitField.Length);
            }

            lock (this.locker)
            {
                // only missing pieces can be checked out
                if (this.BitField[pieceIndex] == PieceStatus.Missing ||
                    (this.BitField[pieceIndex] == PieceStatus.CheckedOut &&
                     this.IsEndGame))
                {
                    hash = this.pieceHashes.ElementAt(pieceIndex);
                    pieceLength = this.GetPieceLength(pieceIndex);
                    blockCount = this.GetBlockCount(pieceIndex);

                    piece = new Piece(pieceIndex, hash, pieceLength, this.BlockLength, blockCount, pieceData, bitField);
                    piece.Completed += this.Piece_Completed;
                    piece.Corrupted += this.Piece_Corrupted;

                    this.BitField[pieceIndex] = PieceStatus.CheckedOut;

                    if (!this.checkouts.ContainsKey(pieceIndex))
                    {
                        this.checkouts.Add(pieceIndex, DateTime.UtcNow);
                    }
                }
            }

            return piece;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.CheckIfObjectIsDisposed();

            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer.Enabled = false;
                this.timer.Dispose();
                this.timer = null;
            }
        }

        /// <summary>
        /// Gets the block count.
        /// </summary>
        /// <param name="pieceIndex">Index of the piece.</param>
        /// <returns>The block count.</returns>
        public int GetBlockCount(int pieceIndex)
        {
            pieceIndex.MustBeGreaterThanOrEqualTo(0);
            pieceIndex.MustBeLessThan(this.PieceCount);

            this.CheckIfObjectIsDisposed();

            long pieceLength = this.GetPieceLength(pieceIndex);
            long blockLength = this.BlockLength;
            long remainder = pieceLength % blockLength;
            long blockCount;

            blockCount = (pieceLength - remainder) / blockLength;
            blockCount += remainder > 0 ? 1 : 0;

            return (int)blockCount;
        }

        /// <summary>
        /// Gets the length of the block in bytes.
        /// </summary>
        /// <param name="pieceIndex">Index of the piece.</param>
        /// <param name="blockIndex">The block offset.</param>
        /// <returns>
        /// The length of the block in bytes.
        /// </returns>
        public int GetBlockLength(int pieceIndex, int blockIndex)
        {
            pieceIndex.MustBeGreaterThanOrEqualTo(0);
            pieceIndex.MustBeLessThan(this.PieceCount);
            blockIndex.MustBeGreaterThanOrEqualTo(0);
            blockIndex.MustBeLessThan(this.BlockCount);

            long pieceLength;
            long blockCount;

            this.CheckIfObjectIsDisposed();

            blockCount = this.GetBlockCount(pieceIndex);

            if (blockIndex == blockCount - 1)
            {
                pieceLength = this.GetPieceLength(pieceIndex);

                if (pieceLength % this.BlockLength != 0)
                {
                    // last block can be shorter
                    return (int)(pieceLength % this.BlockLength);
                }
            }

            return this.BlockLength;
        }

        /// <summary>
        /// Gets the piece.
        /// </summary>
        /// <param name="pieceIndex">Index of the piece.</param>
        /// <returns>The piece.</returns>
        public Piece GetPiece(int pieceIndex)
        {
            pieceIndex.MustBeGreaterThanOrEqualTo(0);
            pieceIndex.MustBeLessThan(this.PieceCount);

            this.CheckIfObjectIsDisposed();

            PieceRequestedEventArgs e = new PieceRequestedEventArgs(pieceIndex);

            this.OnPieceRequested(this, e);

            if (e.PieceData != null)
            {
                return new Piece(pieceIndex, this.pieceHashes.ElementAt(pieceIndex), this.PieceLength, this.BlockLength, this.BlockCount);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the length of the piece in bytes.
        /// </summary>
        /// <param name="pieceIndex">Index of the piece.</param>
        /// <returns>
        /// The length of the piece in bytes.
        /// </returns>
        public long GetPieceLength(int pieceIndex)
        {
            pieceIndex.MustBeGreaterThanOrEqualTo(0);
            pieceIndex.MustBeLessThan(this.PieceCount);

            this.CheckIfObjectIsDisposed();

            if (pieceIndex == this.PieceCount - 1)
            {
                if (this.TorrentLength % this.PieceLength != 0)
                {
                    // last piece can be shorter
                    return this.TorrentLength % this.PieceLength;
                }
            }

            return this.PieceLength;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Checks if object is disposed.
        /// </summary>
        private void CheckIfObjectIsDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        /// <summary>
        /// Called when piece has completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPieceCompleted(object sender, PieceCompletedEventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.PieceCompleted != null)
            {
                this.PieceCompleted(sender, e);
            }
        }

        /// <summary>
        /// Called when piece is requested.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPieceRequested(object sender, PieceRequestedEventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.PieceRequested != null)
            {
                this.PieceRequested(sender, e);
            }
        }

        /// <summary>
        /// Handles the Completed event of the Piece control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PieceCompletedEventArgs"/> instance containing the event data.</param>
        private void Piece_Completed(object sender, PieceCompletedEventArgs e)
        {
            lock (this.locker)
            {
                // only pieces not yet downloaded can be checked in
                if (this.BitField[e.PieceIndex] == PieceStatus.Missing ||
                    this.BitField[e.PieceIndex] == PieceStatus.CheckedOut)
                {
                    this.BitField[e.PieceIndex] = PieceStatus.Present;

                    this.presentPiecesCount++;
                    this.CompletedPercentage = (decimal)this.presentPiecesCount / (decimal)this.piecesCount;

                    if (this.checkouts.ContainsKey(e.PieceIndex))
                    {
                        this.checkouts.Remove(e.PieceIndex);
                    }

                    this.OnPieceCompleted(this, new PieceCompletedEventArgs(e.PieceIndex, e.PieceData));
                }
            }
        }

        /// <summary>
        /// Handles the Corrupted event of the Piece control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Piece_Corrupted(object sender, EventArgs e)
        {
            // Ignore
        }

        /// <summary>
        /// Handles the Elapsed event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime checkoutTime;
            int pieceIndex;
            HashSet<int> checkoutsToRemove = new HashSet<int>();

            Thread.CurrentThread.Name = "piece manager checker";

            this.timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;

            lock (this.locker)
            {
                foreach (var checkOut in this.checkouts)
                {
                    pieceIndex = checkOut.Key;
                    checkoutTime = checkOut.Value;

                    if (DateTime.UtcNow - checkoutTime > this.checkoutTimeout)
                    {
                        checkoutsToRemove.Add(checkOut.Key);
                    }
                }

                foreach (var checkoutToRemove in checkoutsToRemove)
                {
                    this.checkouts.Remove(checkoutToRemove);

                    // checkout timeout -> mark piece as missing, giving other peers a chance to download it
                    this.BitField[checkoutToRemove] = PieceStatus.Missing;
                }
            }

            this.timer.Interval = this.timerTimeout.TotalMilliseconds;
        }

        #endregion Private Methods
    }
}
