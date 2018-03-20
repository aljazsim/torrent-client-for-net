using System;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient.TrackerProtocol
{
    /// <summary>
    /// The tracking failed event arguments.
    /// </summary>
    public sealed class TrackingFailedEventArgs : EventArgs
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingFailedEventArgs"/> class.
        /// </summary>
        /// <param name="trackingUri">The tracking URI.</param>
        /// <param name="failureReason">The failure reason.</param>
        public TrackingFailedEventArgs(Uri trackingUri, string failureReason)
        {
            trackingUri.CannotBeNull();
            failureReason.CannotBeNullOrEmpty();

            this.TrackerUri = trackingUri;
            this.FailureReason = failureReason;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="TrackingFailedEventArgs"/> class from being created.
        /// </summary>
        private TrackingFailedEventArgs()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the failure reason.
        /// </summary>
        /// <value>
        /// The failure reason.
        /// </value>
        public string FailureReason
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

        #endregion Public Properties
    }
}
