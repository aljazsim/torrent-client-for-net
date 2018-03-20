using System;
using System.Collections.Generic;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol;

namespace TorrentClient
{
    /// <summary>
    /// The torrent progress info.
    /// </summary>
    public class TorrentProgressInfo
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentProgressInfo" /> class.
        /// </summary>
        /// <param name="torrentInfoHash">The torrent information hash.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="completedPercentage">The completed percentage.</param>
        /// <param name="downloaded">The downloaded.</param>
        /// <param name="downloadSpeed">The download speed.</param>
        /// <param name="uploaded">The uploaded.</param>
        /// <param name="uploadSpeed">The upload speed.</param>
        /// <param name="leecherCount">The leecher count.</param>
        /// <param name="seederCount">The seeder count.</param>
        public TorrentProgressInfo(string torrentInfoHash, TimeSpan duration, decimal completedPercentage, long downloaded, decimal downloadSpeed, long uploaded, decimal uploadSpeed, int leecherCount, int seederCount)
        {
            torrentInfoHash.CannotBeNullOrEmpty();
            duration.MustBeGreaterThanOrEqualTo(TimeSpan.Zero);
            completedPercentage.MustBeGreaterThanOrEqualTo(0);
            downloaded.MustBeGreaterThanOrEqualTo(0);
            downloadSpeed.MustBeGreaterThanOrEqualTo(0);
            uploaded.MustBeGreaterThanOrEqualTo(0);
            uploadSpeed.MustBeGreaterThanOrEqualTo(0);
            leecherCount.MustBeGreaterThanOrEqualTo(0);
            seederCount.MustBeGreaterThanOrEqualTo(0);

            this.TorrentInfoHash = torrentInfoHash;
            this.Duration = duration;
            this.CompletedPercentage = completedPercentage;
            this.Downloaded = downloaded;
            this.DownloadSpeed = downloadSpeed;
            this.Uploaded = uploaded;
            this.UploadSpeed = uploadSpeed;
            this.LeecherCount = leecherCount;
            this.SeederCount = seederCount;

            this.Trackers = new List<TorrentTrackerInfo>();
            this.Peers = new List<TorrentPeerInfo>();
            this.Pieces = new List<PieceStatus>();
            this.Files = new List<TorrentFileInfo>();
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="TorrentProgressInfo"/> class from being created.
        /// </summary>
        private TorrentProgressInfo()
        {
        }

        #endregion Private Constructors

        #region Public Properties

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
        /// Gets the downloaded byte count.
        /// </summary>
        /// <value>
        /// The downloaded byte count.
        /// </value>
        public long Downloaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the download speed in bytes per second.
        /// </summary>
        /// <value>
        /// The download speed in bytes per second.
        /// </value>
        public decimal DownloadSpeed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public TimeSpan Duration
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        public IEnumerable<TorrentFileInfo> Files
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the leecher count.
        /// </summary>
        /// <value>
        /// The leecher count.
        /// </value>
        public int LeecherCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the peers.
        /// </summary>
        /// <value>
        /// The peers.
        /// </value>
        public IEnumerable<TorrentPeerInfo> Peers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the pieces.
        /// </summary>
        /// <value>
        /// The pieces.
        /// </value>
        public List<PieceStatus> Pieces
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the seeder count.
        /// </summary>
        /// <value>
        /// The seeder count.
        /// </value>
        public int SeederCount
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
        /// Gets the trackers.
        /// </summary>
        /// <value>
        /// The trackers.
        /// </value>
        public IEnumerable<TorrentTrackerInfo> Trackers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the uploaded byte count.
        /// </summary>
        /// <value>
        /// The uploaded byte count.
        /// </value>
        public long Uploaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the upload speed in bytes per second.
        /// </summary>
        /// <value>
        /// The upload speed in bytes per second.
        /// </value>
        public decimal UploadSpeed
        {
            get;
            private set;
        }

        #endregion Public Properties
    }
}
