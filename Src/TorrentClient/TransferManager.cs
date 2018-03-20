using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol;
using TorrentClient.TrackerProtocol;
using TorrentClient.TrackerProtocol.Http;
using TorrentClient.TrackerProtocol.Udp;
using TorrentClient.TrackerProtocol.Udp.Messages.Messages;

namespace TorrentClient
{
    /// <summary>
    /// The torrent transfer manager.
    /// </summary>
    public sealed class TransferManager : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// The downloaded bytes count.
        /// </summary>
        private long downloaded = 0;

        /// <summary>
        /// The peers.
        /// </summary>
        private Dictionary<IPEndPoint, Peer> peers = new Dictionary<IPEndPoint, Peer>();

        /// <summary>
        /// The persistence manager.
        /// </summary>
        private PersistenceManager persistenceManager;

        /// <summary>
        /// The piece manager.
        /// </summary>
        private PieceManager pieceManager;

        /// <summary>
        /// The throttling manager.
        /// </summary>
        private ThrottlingManager throttlingManager;

        /// <summary>
        /// The trackers.
        /// </summary>
        private IDictionary<Uri, Tracker> trackers = new Dictionary<Uri, Tracker>();

        /// <summary>
        /// The uploaded bytes count.
        /// </summary>
        private long uploaded = 0;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferManager" /> class.
        /// </summary>
        /// <param name="listeningPort">The port.</param>
        /// <param name="torrentInfo">The torrent information.</param>
        /// <param name="throttlingManager">The throttling manager.</param>
        /// <param name="persistenceManager">The persistence manager.</param>
        public TransferManager(int listeningPort, TorrentInfo torrentInfo, ThrottlingManager throttlingManager, PersistenceManager persistenceManager)
        {
            listeningPort.MustBeGreaterThanOrEqualTo(IPEndPoint.MinPort);
            listeningPort.MustBeLessThanOrEqualTo(IPEndPoint.MaxPort);
            torrentInfo.CannotBeNull();
            throttlingManager.CannotBeNull();
            persistenceManager.CannotBeNull();

            Tracker tracker = null;

            this.PeerId = "-AB1100-" + "0123456789ABCDEF".Random(24);

            Debug.WriteLine($"creating torrent manager for torrent {torrentInfo.InfoHash}");
            Debug.WriteLine($"local peer id {this.PeerId}");

            this.TorrentInfo = torrentInfo;

            this.throttlingManager = throttlingManager;

            this.persistenceManager = persistenceManager;

            // initialize trackers
            foreach (var trackerUri in torrentInfo.AnnounceList)
            {
                if (trackerUri.Scheme == "http" ||
                    trackerUri.Scheme == "https")
                {
                    tracker = new HttpTracker(trackerUri, this.PeerId, torrentInfo.InfoHash, listeningPort);
                }
                else if (trackerUri.Scheme == "udp")
                {
                    tracker = new UdpTracker(trackerUri, this.PeerId, torrentInfo.InfoHash, listeningPort);
                }

                if (tracker != null)
                {
                    tracker.TrackingEvent = TrackingEvent.Started;
                    tracker.Announcing += this.Tracker_Announcing;
                    tracker.Announced += this.Tracker_Announced;
                    tracker.TrackingFailed += this.Tracker_TrackingFailed;
                    tracker.BytesLeftToDownload = this.TorrentInfo.Length - this.Downloaded;
                    tracker.WantedPeerCount = 30; // we can handle 30 peers at a time

                    this.trackers.Add(trackerUri, tracker);
                }
                else
                {
                    // unsupported tracker protocol
                    Debug.WriteLine($"unsupported tracker protocol {trackerUri.Scheme}");
                }
            }
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="TransferManager"/> class from being created.
        /// </summary>
        private TransferManager()
        {
        }

        #endregion Private Constructors

        #region Public Events

        /// <summary>
        /// Occurs when torrent transfer has begun hashing.
        /// </summary>
        public event EventHandler<EventArgs> TorrentHashing;

        /// <summary>
        /// Occurs when torrent is leeching.
        /// </summary>
        public event EventHandler<EventArgs> TorrentLeeching;

        /// <summary>
        /// Occurs when torrent transfer has begun seeding.
        /// </summary>
        public event EventHandler<EventArgs> TorrentSeeding;

        /// <summary>
        /// Occurs when torrent has started.
        /// </summary>
        public event EventHandler<EventArgs> TorrentStarted;

        /// <summary>
        /// Occurs when torrent has stopped.
        /// </summary>
        public event EventHandler<EventArgs> TorrentStopped;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets the completed percentage.
        /// </summary>
        /// <value>
        /// The completed percentage.
        /// </value>
        public decimal CompletedPercentage
        {
            get
            {
                return this.pieceManager.CompletedPercentage;
            }
        }

        /// <summary>
        /// Gets the downloaded bytes count.
        /// </summary>
        /// <value>
        /// The downloaded bytes count.
        /// </value>
        public long Downloaded
        {
            get
            {
                lock (((IDictionary)this.peers).SyncRoot)
                {
                    return this.downloaded + this.peers.Values.Sum(x => x.Downloaded);
                }
            }
        }

        /// <summary>
        /// Gets the download speed in bytes per second.
        /// </summary>
        /// <value>
        /// The download speed in bytes per second.
        /// </value>
        public decimal DownloadSpeed
        {
            get
            {
                lock (((IDictionary)this.peers).SyncRoot)
                {
                    return this.peers.Values.Sum(x => x.DownloadSpeed);
                }
            }
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
        /// Gets the leeching peer count.
        /// </summary>
        /// <value>
        /// The leeching peer count.
        /// </value>
        public int LeechingPeerCount
        {
            get
            {
                this.CheckIfObjectIsDisposed();

                lock (((IDictionary)this.peers).SyncRoot)
                {
                    return this.peers.Values.Count(x => x.LeechingState == LeechingState.Interested);
                }
            }
        }

        /// <summary>
        /// Gets the peer count.
        /// </summary>
        /// <value>
        /// The peer count.
        /// </value>
        public int PeerCount
        {
            get
            {
                this.CheckIfObjectIsDisposed();

                lock (((IDictionary)this.peers).SyncRoot)
                {
                    return this.peers.Count;
                }
            }
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
        /// Gets the seeding peer count.
        /// </summary>
        /// <value>
        /// The seeding peer count.
        /// </value>
        public int SeedingPeerCount
        {
            get
            {
                this.CheckIfObjectIsDisposed();

                lock (((IDictionary)this.peers).SyncRoot)
                {
                    return this.peers.Values.Count(x => x.SeedingState == SeedingState.Unchoked);
                }
            }
        }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public DateTime StartTime
        {
            get;
            private set;
        }

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

        /// <summary>
        /// Gets the uploaded bytes count.
        /// </summary>
        /// <value>
        /// The uploaded bytes count.
        /// </value>
        public long Uploaded
        {
            get
            {
                lock (((IDictionary)this.peers).SyncRoot)
                {
                    return this.uploaded + this.peers.Values.Sum(x => x.Uploaded);
                }
            }
        }

        /// <summary>
        /// Gets the upload speed in bytes per second.
        /// </summary>
        /// <value>
        /// The upload speed in bytes per second.
        /// </value>
        public decimal UploadSpeed
        {
            get
            {
                lock (((IDictionary)this.peers).SyncRoot)
                {
                    return this.peers.Values.Sum(x => x.UploadSpeed);
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds the leecher.
        /// </summary>
        /// <param name="tcp">The TCP.</param>
        /// <param name="peerId">The peer identifier.</param>
        public void AddLeecher(TcpClient tcp, string peerId)
        {
            tcp.CannotBeNull();
            peerId.CannotBeNull();

            Peer peer;
            int maxLeechers = 10;

            lock (((IDictionary)this.peers).SyncRoot)
            {
                if (!this.peers.ContainsKey(tcp.Client.RemoteEndPoint as IPEndPoint))
                {
                    if (this.LeechingPeerCount < maxLeechers)
                    {
                        Debug.WriteLine($"adding leeching peer {tcp.Client.RemoteEndPoint} to torrent {this.TorrentInfo.InfoHash}");

                        // setup tcp client
                        tcp.ReceiveBufferSize = (int)Math.Max(this.TorrentInfo.BlockLength, this.TorrentInfo.PieceHashes.Count()) + 100;
                        tcp.SendBufferSize = tcp.ReceiveBufferSize;
                        tcp.Client.ReceiveTimeout = (int)TimeSpan.FromSeconds(60).TotalMilliseconds;
                        tcp.Client.SendTimeout = tcp.Client.ReceiveTimeout;

                        // add new peer
                        peer = new Peer(new PeerCommunicator(this.throttlingManager, tcp), this.pieceManager, this.PeerId, peerId);
                        peer.CommunicationErrorOccurred += this.Peer_CommunicationErrorOccurred;

                        this.peers.Add(tcp.Client.RemoteEndPoint as IPEndPoint, peer);
                    }
                    else
                    {
                        tcp.Close();
                    }
                }
                else
                {
                    tcp.Close();
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.IsDisposed = true;

                Debug.WriteLine($"disposing torrent manager for torrent {this.TorrentInfo.InfoHash}");

                this.Stop();

                lock (((IDictionary)this.trackers).SyncRoot)
                {
                    if (this.trackers != null)
                    {
                        this.trackers.Clear();
                        this.trackers = null;
                    }
                }

                lock (((IDictionary)this.peers).SyncRoot)
                {
                    if (this.peers != null)
                    {
                        this.peers.Clear();
                        this.peers = null;
                    }
                }

                if (this.pieceManager != null)
                {
                    this.pieceManager.Dispose();
                    this.pieceManager = null;
                }
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.CheckIfObjectIsDisposed();

            this.StartTime = DateTime.UtcNow;

            Debug.WriteLine($"starting torrent manager for torrent {this.TorrentInfo.InfoHash}");

            this.OnTorrentHashing(this, EventArgs.Empty);

            // initialize piece manager
            this.pieceManager = new PieceManager(this.TorrentInfo.InfoHash, this.TorrentInfo.Length, this.TorrentInfo.PieceHashes, this.TorrentInfo.PieceLength, this.TorrentInfo.BlockLength, this.persistenceManager.Verify());
            this.pieceManager.PieceCompleted += this.PieceManager_PieceCompleted;
            this.pieceManager.PieceRequested += this.PieceManager_PieceRequested;

            // start tracking
            lock (((IDictionary)this.trackers).SyncRoot)
            {
                foreach (var tracker in this.trackers.Values)
                {
                    tracker.StartTracking();
                }
            }

            this.OnTorrentStarted(this, EventArgs.Empty);

            if (this.pieceManager.IsComplete)
            {
                this.OnTorrentSeeding(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.CheckIfObjectIsDisposed();

            Debug.WriteLine($"stopping torrent manager for torrent {this.TorrentInfo.InfoHash}");

            // stop peers
            lock (((IDictionary)this.peers).SyncRoot)
            {
                foreach (var peer in this.peers.Values)
                {
                    peer.Dispose();

                    this.downloaded += peer.Downloaded;
                    this.uploaded += peer.Uploaded;
                }

                this.peers.Clear();
            }

            // stop tracking
            lock (((IDictionary)this.trackers).SyncRoot)
            {
                foreach (var tracker in this.trackers.Values)
                {
                    tracker.StopTracking();
                }
            }

            this.OnTorrentStopped(this, EventArgs.Empty);

            this.pieceManager.Dispose();
            this.pieceManager = null;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Adds the peer.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        private void AddSeeder(IPEndPoint endpoint)
        {
            endpoint.CannotBeNull();

            TcpClient tcp;

            lock (((IDictionary)this.peers).SyncRoot)
            {
                if (!this.peers.ContainsKey(endpoint))
                {
                    // set up tcp client
                    tcp = new TcpClient();
                    tcp.ReceiveBufferSize = (int)Math.Max(this.TorrentInfo.BlockLength, this.TorrentInfo.PieceHashes.Count()) + 100;
                    tcp.SendBufferSize = tcp.ReceiveBufferSize;
                    tcp.Client.ReceiveTimeout = (int)TimeSpan.FromSeconds(60).TotalMilliseconds;
                    tcp.Client.SendTimeout = tcp.Client.ReceiveTimeout;
                    tcp.BeginConnect(endpoint.Address, endpoint.Port, this.PeerConnected, new AsyncConnectData(endpoint, tcp));
                }
            }
        }

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
        /// Called when torrent is hashing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTorrentHashing(object sender, EventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.TorrentHashing != null)
            {
                this.TorrentHashing(sender, e);
            }
        }

        /// <summary>
        /// Called when torrent is leeching.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTorrentLeeching(object sender, EventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.TorrentLeeching != null)
            {
                this.TorrentLeeching(sender, e);
            }
        }

        /// <summary>
        /// Called when torrent transfer has begun seeding.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTorrentSeeding(object sender, EventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.TorrentSeeding != null)
            {
                this.TorrentSeeding(sender, e);
            }
        }

        /// <summary>
        /// Called when torrent has started.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTorrentStarted(object sender, EventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.TorrentStarted != null)
            {
                this.TorrentStarted(sender, e);
            }
        }

        /// <summary>
        /// Called when torrent has stopped.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTorrentStopped(object sender, EventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.TorrentStopped != null)
            {
                this.TorrentStopped(sender, e);
            }
        }

        /// <summary>
        /// Handles the CommunicationErrorOccurred event of the Peer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Peer_CommunicationErrorOccurred(object sender, PeerCommunicationErrorEventArgs e)
        {
            Peer peer;

            peer = sender as Peer;

            if (e.IsFatal)
            {
                Debug.WriteLine($"fatal communication error occurred for peer {peer.Endpoint} on torrent {this.TorrentInfo.InfoHash}: {e.ErrorMessage}");

                lock (((IDictionary)this.peers).SyncRoot)
                {
                    // update transfer parameters
                    this.downloaded += peer.Downloaded;
                    this.uploaded += peer.Uploaded;

                    // something is wrong with the peer -> remove it from the list and close the connection
                    if (this.peers.ContainsKey(peer.Endpoint))
                    {
                        this.peers.Remove(peer.Endpoint);
                    }

                    // dispose of the peer
                    peer.Dispose();
                }
            }
            else
            {
                Debug.WriteLine($"communication error occurred for peer {peer.Endpoint} on torrent {this.TorrentInfo.InfoHash}: {e.ErrorMessage}");
            }
        }

        /// <summary>
        /// Peers the connected.
        /// </summary>
        /// <param name="ar">The async result.</param>
        private void PeerConnected(IAsyncResult ar)
        {
            AsyncConnectData data;
            TcpClient tcp;
            Peer peer;
            IPEndPoint endpoint;

            data = ar.AsyncState as AsyncConnectData;
            endpoint = data.Endpoint;

            try
            {
                tcp = data.Tcp;
                tcp.EndConnect(ar);

                lock (((IDictionary)this.peers).SyncRoot)
                {
                    if (this.peers.ContainsKey(endpoint))
                    {
                        // peer is already present
                        tcp.Close();
                        tcp = null;
                    }
                    else
                    {
                        Debug.WriteLine($"adding seeding peer {endpoint} to torrent {this.TorrentInfo.InfoHash}");

                        // add new peer
                        peer = new Peer(new PeerCommunicator(this.throttlingManager, tcp), this.pieceManager, this.PeerId);
                        peer.CommunicationErrorOccurred += this.Peer_CommunicationErrorOccurred;

                        this.peers.Add(endpoint, peer);
                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine($"could not connect to peer {endpoint}: {ex.Message}");
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"connection to peer {endpoint} was closed: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the PieceCompleted event of the PieceManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PieceCompletedEventArgs"/> instance containing the event data.</param>
        private void PieceManager_PieceCompleted(object sender, PieceCompletedEventArgs e)
        {
            Debug.WriteLine($"piece {e.PieceIndex} completed for torrent {this.TorrentInfo.InfoHash}");

            // persist piece
            this.persistenceManager.Put(this.TorrentInfo.Files, this.TorrentInfo.PieceLength, e.PieceIndex, e.PieceData);

            if (this.pieceManager.CompletedPercentage == 1)
            {
                this.OnTorrentSeeding(this, EventArgs.Empty);
            }
            else
            {
                this.OnTorrentLeeching(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the PieceRequested event of the PieceManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PieceRequestedEventArgs"/> instance containing the event data.</param>
        private void PieceManager_PieceRequested(object sender, PieceRequestedEventArgs e)
        {
            Debug.WriteLine($"piece {e.PieceIndex} requested for torrent {this.TorrentInfo.InfoHash}");

            // get piece data
            e.PieceData = this.persistenceManager.Get(e.PieceIndex);
        }

        /// <summary>
        /// Handles the Announced event of the Tracker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="AnnouncedEventArgs"/> instance containing the event data.</param>
        private void Tracker_Announced(object sender, AnnouncedEventArgs e)
        {
            lock (((IDictionary)this.peers).SyncRoot)
            {
                foreach (var endpoint in e.Peers)
                {
                    try
                    {
                        this.AddSeeder(endpoint);
                    }
                    catch (SocketException ex)
                    {
                        Debug.WriteLine($"could not connect to peer {endpoint}: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Announcing event of the Tracker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Tracker_Announcing(object sender, EventArgs e)
        {
            Tracker tracker;

            tracker = sender as Tracker;
            tracker.BytesDownloaded = this.Downloaded;
            tracker.BytesLeftToDownload = this.TorrentInfo.Length - this.Downloaded;
            tracker.BytesUploaded = this.Uploaded;
            tracker.TrackingEvent = this.CompletedPercentage == 1 ? TrackingEvent.Completed : TrackingEvent.Started;
        }

        /// <summary>
        /// Handles the TrackingFailed event of the Tracker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TrackingFailedEventArgs"/> instance containing the event data.</param>
        private void Tracker_TrackingFailed(object sender, TrackingFailedEventArgs e)
        {
            Debug.WriteLine($"tracking failed for tracker {e.TrackerUri} for torrent {this.TorrentInfo.InfoHash}: \"{e.FailureReason}\"");

            sender.As<Tracker>().Dispose();
        }

        #endregion Private Methods
    }
}
