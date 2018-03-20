using System;
using System.Diagnostics;
using TorrentClient.Extensions;
using TorrentClient.TrackerProtocol.Http.Messages;

namespace TorrentClient.TrackerProtocol.Http
{
    /// <summary>
    /// The HTTP tracker.
    /// </summary>
    public sealed class HttpTracker : Tracker
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpTracker" /> class.
        /// </summary>
        /// <param name="trackerUri">The tracker URI.</param>
        /// <param name="peerId">The peer identifier.</param>
        /// <param name="torrentInfoHash">The torrent information hash.</param>
        /// <param name="listeningPort">The listening port.</param>
        public HttpTracker(Uri trackerUri, string peerId, string torrentInfoHash, int listeningPort)
            : base(trackerUri, peerId, torrentInfoHash, listeningPort)
        {
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <summary>
        /// Called when announce is requested.
        /// </summary>
        protected override void OnAnnounce()
        {
            AnnounceResponseMessage message;
            Uri uri;

            this.OnAnnouncing(this, EventArgs.Empty);

            try
            {
                uri = this.GetUri();

                Debug.WriteLine($"{this.TrackerUri} -> {uri}");

                if (AnnounceResponseMessage.TryDecode(uri.ExecuteBinaryRequest(), out message))
                {
                    Debug.WriteLine($"{this.TrackerUri} <- {message}");

                    this.UpdateInterval = message.UpdateInterval;

                    this.OnAnnounced(this, new AnnouncedEventArgs(message.UpdateInterval, message.LeecherCount, message.SeederCount, message.Peers));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"could not send message to HTTP tracker {this.TrackerUri} for torrent {this.TorrentInfoHash}: {ex.Message}");
            }
        }

        /// <summary>
        /// Called when starting tracking.
        /// </summary>
        protected override void OnStart()
        {
        }

        /// <summary>
        /// Called when stopping tracking.
        /// </summary>
        protected override void OnStop()
        {
            this.TrackingEvent = Udp.Messages.Messages.TrackingEvent.Stopped;

            this.OnAnnounce();
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Gets the tracker URI.
        /// </summary>
        /// <returns>The tracker URI.</returns>
        private Uri GetUri()
        {
            string uri;

            uri = this.TrackerUri.ToString();
            uri += "?";
            uri += new AnnounceMessage(this.TorrentInfoHash, this.PeerId, this.ListeningPort, this.BytesUploaded, this.BytesDownloaded, this.BytesLeftToDownload, this.WantedPeerCount, this.TrackingEvent).Encode();

            return new Uri(uri);
        }

        #endregion Private Methods
    }
}
