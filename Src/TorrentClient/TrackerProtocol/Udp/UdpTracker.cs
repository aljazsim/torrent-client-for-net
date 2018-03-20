using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;
using TorrentClient.TrackerProtocol.Udp.Messages;

namespace TorrentClient.TrackerProtocol.Udp
{
    /// <summary>
    /// The UDP tracker.
    /// </summary>
    public sealed class UdpTracker : Tracker
    {
        #region Private Fields

        /// <summary>
        /// The transaction identifier.
        /// </summary>
        private readonly int transactionId;

        /// <summary>
        /// The connection identifier.
        /// </summary>
        private long connectionId;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpTracker" /> class.
        /// </summary>
        /// <param name="trackerUri">The tracker URI.</param>
        /// <param name="peerId">The peer identifier.</param>
        /// <param name="torrentInfoHash">The torrent information hash.</param>
        /// <param name="listeningPort">The listening port.</param>
        public UdpTracker(Uri trackerUri, string peerId, string torrentInfoHash, int listeningPort)
            : base(trackerUri, peerId, torrentInfoHash, listeningPort)
        {
            this.transactionId = new Random(DateTime.UtcNow.Millisecond).Next(0, int.MaxValue);
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <summary>
        /// Called when announce is requested.
        /// </summary>
        protected override void OnAnnounce()
        {
            TrackerMessage message;

            this.OnAnnouncing(this, EventArgs.Empty);

            message = new AnnounceMessage(this.connectionId, this.transactionId, this.TorrentInfoHash, this.PeerId, this.BytesDownloaded, this.BytesLeftToDownload, this.BytesUploaded, this.TrackingEvent, 0, this.WantedPeerCount, new IPEndPoint(IPAddress.Loopback, this.ListeningPort));

            Debug.WriteLine($"{this.TrackerUri} -> {message}");

            message = this.ExecuteUdpRequest(this.TrackerUri, message);

            if (message is AnnounceResponseMessage)
            {
                Debug.WriteLine($"{this.TrackerUri} <- {message}");

                this.OnAnnounced(this, new AnnouncedEventArgs(message.As<AnnounceResponseMessage>().Interval, message.As<AnnounceResponseMessage>().LeecherCount, message.As<AnnounceResponseMessage>().SeederCount, message.As<AnnounceResponseMessage>().Peers));
            }
        }

        /// <summary>
        /// Called when starting tracking.
        /// </summary>
        protected override void OnStart()
        {
            TrackerMessage message;

            message = new ConnectMessage(this.transactionId);
            message = this.ExecuteUdpRequest(this.TrackerUri, message);

            if (message is ConnectResponseMessage)
            {
                if (message.TransactionId == this.transactionId)
                {
                    this.connectionId = message.As<ConnectResponseMessage>().ConnectionId;
                }
                else
                {
                    // connect failed -> drop tracker
                    Debug.WriteLine($"connecting to tracker {this.TrackerUri} failed");
                }
            }
        }

        /// <summary>
        /// Called when stopping tracking.
        /// </summary>
        protected override void OnStop()
        {
            this.TrackingEvent = Messages.Messages.TrackingEvent.Stopped;

            this.OnAnnounce();
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Executes the UDP request.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The tracker response message.
        /// </returns>
        private TrackerMessage ExecuteUdpRequest(Uri uri, TrackerMessage message)
        {
            uri.CannotBeNull();
            message.CannotBeNull();

            byte[] data = null;
            IAsyncResult asyncResult;
            IPEndPoint any = new IPEndPoint(IPAddress.Any, this.ListeningPort);

            try
            {
                using (UdpClient udp = new UdpClient())
                {
                    udp.Client.SendTimeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
                    udp.Client.ReceiveTimeout = (int)TimeSpan.FromSeconds(15).TotalMilliseconds;

                    udp.Send(message.Encode(), message.Length, this.TrackerUri.Host, this.TrackerUri.Port);

                    asyncResult = udp.BeginReceive(null, null);

                    if (asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
                    {
                        data = udp.EndReceive(asyncResult, ref any);
                    }
                    else
                    {
                        // timeout
                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine($"could not send message to UDP tracker {this.TrackerUri} for torrent {this.TorrentInfoHash}: {ex.Message}");

                return null;
            }

            if (TrackerMessage.TryDecode(data, 0, MessageType.Response, out message))
            {
                if (message is ErrorMessage)
                {
                    Debug.WriteLine($"error message received from UDP tracker {this.TrackerUri}: \"{message.As<ErrorMessage>().ErrorText}\"");
                }

                return message;
            }
            else
            {
                return null;
            }
        }

        #endregion Private Methods
    }
}
