using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DefensiveProgrammingFramework;
using TorrentClient.Exceptions;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient
{
    /// <summary>
    /// The torrent client.
    /// </summary>
    public sealed class TorrentClient : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// The downloaded bytes count.
        /// </summary>
        private long downloaded = 0;

        /// <summary>
        /// The TCP server.
        /// </summary>
        private TcpListener server;

        /// <summary>
        /// The throttling manager.
        /// </summary>
        private ThrottlingManager throttlingManager = new ThrottlingManager();

        /// <summary>
        /// The torrent info hash / torrent transfer manager dictionary.
        /// </summary>
        private Dictionary<string, TransferManager> transfers = new Dictionary<string, TransferManager>();

        /// <summary>
        /// The uploaded bytes count.
        /// </summary>
        private long uploaded = 0;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentClient" /> class.
        /// </summary>
        /// <param name="listeningPort">The listening port.</param>
        /// <param name="baseDirectory">The base directory, where all torrents are persisted.</param>
        public TorrentClient(int listeningPort, string baseDirectory)
        {
            baseDirectory.CannotBeNullOrEmpty();
            baseDirectory.MustBeValidDirectoryPath();
            listeningPort.MustBeGreaterThanOrEqualTo(IPEndPoint.MinPort);
            listeningPort.MustBeLessThanOrEqualTo(IPEndPoint.MaxPort);

            Debug.WriteLine("creating torrent client");
            Debug.WriteLine($"listening port: {listeningPort}");
            Debug.WriteLine($"base directory: {Path.GetFullPath(baseDirectory)}");

            this.BaseDirectory = baseDirectory;
            this.ListeningPort = listeningPort;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="TorrentClient"/> class from being created.
        /// </summary>
        private TorrentClient()
        {
        }

        #endregion Private Constructors

        #region Public Events

        /// <summary>
        /// Occurs when torrent is hashing.
        /// </summary>
        public event EventHandler<TorrentHashingEventArgs> TorrentHashing;

        /// <summary>
        /// Occurs when torrent leeching.
        /// </summary>
        public event EventHandler<TorrentLeechingEventArgs> TorrentLeeching;

        /// <summary>
        /// Occurs when torrent seeding.
        /// </summary>
        public event EventHandler<TorrentSeedingEventArgs> TorrentSeeding;

        /// <summary>
        /// Occurs when torrent started.
        /// </summary>
        public event EventHandler<TorrentStartedEventArgs> TorrentStarted;

        /// <summary>
        /// Occurs when torrent stopped.
        /// </summary>
        public event EventHandler<TorrentStoppedEventArgs> TorrentStopped;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets the base directory.
        /// </summary>
        /// <value>
        /// The base directory.
        /// </value>
        public string BaseDirectory
        {
            get;
            private set;
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
                lock (((IDictionary)this.transfers).SyncRoot)
                {
                    return this.downloaded + this.transfers.Values.Sum(x => x.Downloaded);
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
                lock (((IDictionary)this.transfers).SyncRoot)
                {
                    return this.transfers.Values.Sum(x => x.DownloadSpeed);
                }
            }
        }

        /// <summary>
        /// Gets or sets the download speed limit in bytes per second.
        /// </summary>
        /// <value>
        /// The download speed limit in bytes per second.
        /// </value>
        public long DownloadSpeedLimit
        {
            get
            {
                this.CheckIfObjectIsDisposed();

                return this.throttlingManager.ReadSpeedLimit;
            }

            set
            {
                this.CheckIfObjectIsDisposed();

                Debug.WriteLine($"setting download speed limit to {value}B/s");

                this.throttlingManager.ReadSpeedLimit = value;
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
        /// Gets a value indicating whether the torrent client is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the torrent client is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
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
        /// Gets the uploaded bytes count.
        /// </summary>
        /// <value>
        /// The uploaded bytes count.
        /// </value>
        public long Uploaded
        {
            get
            {
                lock (((IDictionary)this.transfers).SyncRoot)
                {
                    return this.uploaded + this.transfers.Values.Sum(x => x.Uploaded);
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
                lock (((IDictionary)this.transfers).SyncRoot)
                {
                    return this.transfers.Values.Sum(x => x.UploadSpeed);
                }
            }
        }

        /// <summary>
        /// Gets or sets the upload speed limit in bytes per second.
        /// </summary>
        /// <value>
        /// The upload speed limit in bytes per second.
        /// </value>
        public long UploadSpeedLimit
        {
            get
            {
                this.CheckIfObjectIsDisposed();

                return this.throttlingManager.WriteSpeedLimit;
            }

            set
            {
                this.CheckIfObjectIsDisposed();

                Debug.WriteLine($"setting upload speed limit to {value}B/s");

                this.throttlingManager.WriteSpeedLimit = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.IsDisposed = true;

                Debug.WriteLine("disposing torrent client");

                if (this.IsRunning)
                {
                    this.Stop();
                }
            }
        }

        /// <summary>
        /// Gets the progress information.
        /// </summary>
        /// <param name="torrentInfoHash">The torrent information hash.</param>
        /// <returns>
        /// The torrent progress information.
        /// </returns>
        public TorrentProgressInfo GetProgressInfo(string torrentInfoHash)
        {
            torrentInfoHash.CannotBeNull();

            TransferManager transferManager;

            if (this.transfers.TryGetValue(torrentInfoHash, out transferManager))
            {
                return new TorrentProgressInfo(
                    torrentInfoHash,
                    DateTime.UtcNow - transferManager.StartTime,
                    transferManager.CompletedPercentage,
                    transferManager.Downloaded,
                    transferManager.DownloadSpeed,
                    transferManager.Uploaded,
                    transferManager.UploadSpeed,
                    transferManager.LeechingPeerCount,
                    transferManager.SeedingPeerCount);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the progress information.
        /// </summary>
        /// <returns>
        /// The torrent progress information.
        /// </returns>
        public IEnumerable<TorrentProgressInfo> GetProgressInfo()
        {
            List<TorrentProgressInfo> info = new List<TorrentProgressInfo>();

            this.CheckIfObjectIsDisposed();

            lock (((IDictionary)this.transfers).SyncRoot)
            {
                foreach (var transferManager in this.transfers.Values)
                {
                    info.Add(new TorrentProgressInfo(
                        transferManager.TorrentInfo.InfoHash,
                        DateTime.UtcNow - transferManager.StartTime,
                        transferManager.CompletedPercentage,
                        transferManager.Downloaded,
                        transferManager.DownloadSpeed,
                        transferManager.Uploaded,
                        transferManager.UploadSpeed,
                        transferManager.LeechingPeerCount,
                        transferManager.SeedingPeerCount));
                }
            }

            return info;
        }

        /// <summary>
        /// Starts the specified listening port.
        /// </summary>
        public void Start()
        {
            this.CheckIfObjectIsDisposed();

            Thread thread;

            Debug.WriteLine("starting torrent client");

            // start listening
            this.server = new TcpListener(IPAddress.Any, (int)this.ListeningPort);
            this.server.Start();

            this.IsRunning = true;

            thread = new Thread(this.Listen);
            thread.IsBackground = true;
            thread.Name = "port listener";
            thread.Start();
        }

        /// <summary>
        /// Starts the specified torrent.
        /// </summary>
        /// <param name="torrentInfo">The torrent information.</param>
        public void Start(TorrentInfo torrentInfo)
        {
            torrentInfo.CannotBeNull();

            TransferManager transfer;

            Debug.WriteLine($"starting torrent {torrentInfo.InfoHash}");

            if (this.IsRunning)
            {
                lock (((IDictionary)this.transfers).SyncRoot)
                {
                    if (!this.transfers.ContainsKey(torrentInfo.InfoHash))
                    {
                        transfer = new TransferManager(this.ListeningPort, torrentInfo, this.throttlingManager, new PersistenceManager(this.BaseDirectory, torrentInfo.Length, torrentInfo.PieceLength, torrentInfo.PieceHashes, torrentInfo.Files));
                        transfer.TorrentHashing += this.Transfer_TorrentHashing;
                        transfer.TorrentLeeching += this.Transfer_TorrentLeeching;
                        transfer.TorrentSeeding += this.Transfer_TorrentSeeding;
                        transfer.TorrentStarted += this.Transfer_TorrentStarted;
                        transfer.TorrentStopped += this.Transfer_TorrentStopped;
                        transfer.Start();

                        this.transfers.Add(torrentInfo.InfoHash, transfer);
                    }
                    else
                    {
                        throw new TorrentClientException($"Torrent {torrentInfo.InfoHash} is already active.");
                    }
                }
            }
            else
            {
                throw new TorrentClientException("Torrent client is not running.");
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.CheckIfObjectIsDisposed();

            Debug.WriteLine("stopping torrent client");

            if (!this.IsRunning)
            {
                // stop tracking
                lock (((IDictionary)this.transfers).SyncRoot)
                {
                    foreach (var transfer in this.transfers.Values)
                    {
                        this.downloaded += transfer.Downloaded;
                        this.uploaded += transfer.Uploaded;

                        transfer.Stop();
                        transfer.Dispose();
                    }

                    this.transfers.Clear();
                }

                // stop server
                this.server.Stop();
                this.server = null;

                this.IsRunning = false;
            }
            else
            {
                throw new TorrentClientException("Torrent client is not running.");
            }
        }

        /// <summary>
        /// Stops the specified torrent.
        /// </summary>
        /// <param name="torrentInfoHash">The torrent information hash.</param>
        public void Stop(string torrentInfoHash)
        {
            torrentInfoHash.CannotBeNullOrEmpty();

            TransferManager transfer;

            Debug.WriteLine($"stopping torrent {torrentInfoHash}");

            lock (((IDictionary)this.transfers).SyncRoot)
            {
                if (this.transfers.TryGetValue(torrentInfoHash, out transfer))
                {
                    this.transfers.Remove(torrentInfoHash);

                    transfer.Stop();

                    transfer.Dispose();
                    transfer = null;
                }
                else
                {
                    throw new TorrentClientException($"Torrent {torrentInfoHash} is not active.");
                }
            }
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
        /// Listens to incoming socket messages.
        /// </summary>
        private void Listen()
        {
            int timeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
            int bufferSize = 1024 * 1024;
            int bytesRead;
            int offset = 0;
            byte[] buffer = new byte[bufferSize];
            TcpClient tcp;
            TransferManager transfer;

            while (!this.IsDisposed &&
                   this.IsRunning)
            {
                tcp = this.server.AcceptTcpClient();
                tcp.Client.SendTimeout = timeout;
                tcp.SendBufferSize = bufferSize;
                tcp.Client.ReceiveTimeout = timeout;
                tcp.ReceiveBufferSize = bufferSize;

                offset = 0;

                try
                {
                    bytesRead = tcp.GetStream().Read(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        foreach (var message in PeerMessage.Decode(buffer, ref offset, bytesRead))
                        {
                            if (message is HandshakeMessage)
                            {
                                lock (((IDictionary)this.transfers).SyncRoot)
                                {
                                    if (this.transfers.TryGetValue(message.As<HandshakeMessage>().InfoHash, out transfer))
                                    {
                                        transfer.AddLeecher(tcp, message.As<HandshakeMessage>().PeerId);
                                    }
                                    else
                                    {
                                        Debug.WriteLine($"invalid torrent info hash: {message.As<HandshakeMessage>().InfoHash} received");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    // something is wrong with remote peer -> ignore it
                    Debug.WriteLine($"could not read stream from {tcp.Client.RemoteEndPoint}: {ex.Message}");

                    // close the connection
                    tcp.Close();
                    tcp = null;
                }
            }
        }

        /// <summary>
        /// Called when torrent is hashing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTorrentHashing(object sender, TorrentHashingEventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.TorrentHashing != null)
            {
                this.TorrentHashing(sender, e);
            }
        }

        /// <summary>
        /// Called when torrent leeching.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTorrentLeeching(object sender, TorrentLeechingEventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.TorrentLeeching != null)
            {
                this.TorrentLeeching(sender, e);
            }
        }

        /// <summary>
        /// Called when torrent seeding.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTorrentSeeding(object sender, TorrentSeedingEventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.TorrentSeeding != null)
            {
                this.TorrentSeeding(sender, e);
            }
        }

        /// <summary>
        /// Called when torrent started.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTorrentStarted(object sender, TorrentStartedEventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.TorrentStarted != null)
            {
                this.TorrentStarted(sender, e);
            }
        }

        /// <summary>
        /// Called when torrent stopped.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTorrentStopped(object sender, TorrentStoppedEventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.TorrentStopped != null)
            {
                this.TorrentStopped(sender, e);
            }
        }

        /// <summary>
        /// Handles the TorrentHashing event of the Transfer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Transfer_TorrentHashing(object sender, EventArgs e)
        {
            Debug.WriteLine($"torrent {sender.As<TransferManager>().TorrentInfo.InfoHash} hashing");

            this.OnTorrentHashing(this, new TorrentHashingEventArgs(sender.As<TransferManager>().TorrentInfo));
        }

        /// <summary>
        /// Handles the TorrentLeeching event of the Transfer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Transfer_TorrentLeeching(object sender, EventArgs e)
        {
            Debug.WriteLine($"torrent {sender.As<TransferManager>().TorrentInfo.InfoHash} leeching");

            this.OnTorrentLeeching(this, new TorrentLeechingEventArgs(sender.As<TransferManager>().TorrentInfo));
        }

        /// <summary>
        /// Handles the TorrentSeeding event of the Transfer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Transfer_TorrentSeeding(object sender, EventArgs e)
        {
            Debug.WriteLine($"torrent {sender.As<TransferManager>().TorrentInfo.InfoHash} seeding");

            this.OnTorrentSeeding(this, new TorrentSeedingEventArgs(sender.As<TransferManager>().TorrentInfo));
        }

        /// <summary>
        /// Handles the TorrentStarted event of the Transfer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Transfer_TorrentStarted(object sender, EventArgs e)
        {
            Debug.WriteLine($"torrent {sender.As<TransferManager>().TorrentInfo.InfoHash} started");

            this.OnTorrentStarted(this, new TorrentStartedEventArgs(sender.As<TransferManager>().TorrentInfo));
        }

        /// <summary>
        /// Handles the TorrentStopped event of the Transfer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Transfer_TorrentStopped(object sender, EventArgs e)
        {
            Debug.WriteLine($"torrent {sender.As<TransferManager>().TorrentInfo.InfoHash} stopped");

            this.OnTorrentStopped(this, new TorrentStoppedEventArgs(sender.As<TransferManager>().TorrentInfo));
        }

        #endregion Private Methods
    }
}
