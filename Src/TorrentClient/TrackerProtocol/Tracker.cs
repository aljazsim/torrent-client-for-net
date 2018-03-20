using System;
using System.Diagnostics;
using System.Net;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;
using TorrentClient.TrackerProtocol.Udp.Messages.Messages;

namespace TorrentClient.TrackerProtocol
{
    /// <summary>
    /// The tracker base.
    /// </summary>
    public abstract class Tracker : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// The tracking timer.
        /// </summary>
        private System.Timers.Timer timer;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Tracker" /> class.
        /// </summary>
        /// <param name="trackerUri">The tracker URI.</param>
        /// <param name="peerId">The peer identifier.</param>
        /// <param name="torrentInfoHash">The torrent information hash.</param>
        /// <param name="listeningPort">The listening port.</param>
        public Tracker(Uri trackerUri, string peerId, string torrentInfoHash, int listeningPort)
        {
            trackerUri.CannotBeNull();
            torrentInfoHash.CannotBeNullOrEmpty();
            torrentInfoHash.Length.MustBeEqualTo(40);
            peerId.CannotBeNullOrEmpty();
            peerId.Length.MustBeGreaterThanOrEqualTo(20);
            listeningPort.MustBeGreaterThanOrEqualTo(IPEndPoint.MinPort);
            listeningPort.MustBeGreaterThanOrEqualTo(IPEndPoint.MinPort);

            this.UpdateInterval = TimeSpan.FromMinutes(10);
            this.TrackerUri = trackerUri;
            this.TorrentInfoHash = torrentInfoHash;
            this.ListeningPort = listeningPort;
            this.PeerId = peerId;
        }

        #endregion Public Constructors

        #region Public Events

        /// <summary>
        /// Occurs when announce has completed.
        /// </summary>
        public event EventHandler<AnnouncedEventArgs> Announced;

        /// <summary>
        /// Occurs before announce has started.
        /// </summary>
        public event EventHandler<EventArgs> Announcing;

        /// <summary>
        /// Occurs when tracking has failed.
        /// </summary>
        public event EventHandler<TrackingFailedEventArgs> TrackingFailed;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets or sets the bytes downloaded.
        /// </summary>
        /// <value>
        /// The bytes downloaded.
        /// </value>
        public long BytesDownloaded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bytes left to download.
        /// </summary>
        /// <value>
        /// The bytes left to download.
        /// </value>
        public long BytesLeftToDownload
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bytes uploaded.
        /// </summary>
        /// <value>
        /// The bytes uploaded.
        /// </value>
        public long BytesUploaded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the object is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the object is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the listening port.
        /// </summary>
        /// <value>
        /// The listening port.
        /// </value>
        public int ListeningPort
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the peer identifier.
        /// </summary>
        /// <value>
        /// The peer identifier.
        /// </value>
        public string PeerId
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
        /// Gets the tracker URI.
        /// </summary>
        /// <value>
        /// The tracker URI.
        /// </value>
        public Uri TrackerUri
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the tracking event.
        /// </summary>
        /// <value>
        /// The tracking event.
        /// </value>
        public TrackingEvent TrackingEvent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the update interval.
        /// </summary>
        /// <value>
        /// The update interval.
        /// </value>
        public TimeSpan UpdateInterval
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the wanted peer count.
        /// </summary>
        /// <value>
        /// The wanted peer count.
        /// </value>
        public int WantedPeerCount
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.IsDisposed = true;

                Debug.WriteLine("disposing tracker");

                this.StopTracking();
            }
        }

        /// <summary>
        /// Starts the tracking.
        /// </summary>
        public void StartTracking()
        {
            this.CheckIfObjectIsDisposed();

            Debug.WriteLine($"starting tracking {this.TrackerUri} for torrent { this.TorrentInfoHash}");

            this.OnStart();

            this.timer = new System.Timers.Timer();
            this.timer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
            this.timer.Elapsed += this.Timer_Elapsed;
            this.timer.Enabled = true;
            this.timer.Start();
        }

        /// <summary>
        /// Stops the tracking.
        /// </summary>
        public void StopTracking()
        {
            this.CheckIfObjectIsDisposed();

            Debug.WriteLine($"stopping tracking {this.TrackerUri} for torrent {this.TorrentInfoHash}");

            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer.Enabled = false;
                this.timer.Dispose();
                this.timer = null;
            }

            this.OnStop();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Called when announcing.
        /// </summary>
        protected abstract void OnAnnounce();

        /// <summary>
        /// Called when announce has completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        protected void OnAnnounced(object sender, AnnouncedEventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.Announced != null)
            {
                this.Announced(sender, e);
            }
        }

        /// <summary>
        /// Called when announce.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        protected void OnAnnouncing(object sender, EventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.Announcing != null)
            {
                this.Announcing(sender, e);
            }
        }

        /// <summary>
        /// Called when starting tracking.
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// Called when stopping tracking.
        /// </summary>
        protected abstract void OnStop();

        #endregion Protected Methods

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
        /// Called when tracking has failed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTrackingFailed(object sender, TrackingFailedEventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.TrackingFailed != null)
            {
                this.TrackingFailed(sender, e);
            }
        }

        /// <summary>
        /// Handles the Elapsed event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;

            this.OnAnnounce();

            this.timer.Interval = this.UpdateInterval.TotalMilliseconds;
        }

        #endregion Private Methods
    }
}
