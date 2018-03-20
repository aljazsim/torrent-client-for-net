namespace TorrentClient.TrackerProtocol.Udp.Messages.Messages
{
    /// <summary>
    /// The tracking event.
    /// </summary>
    public enum TrackingEvent : int
    {
        /// <summary>
        /// The none tracking event.
        /// </summary>
        None = 0,

        /// <summary>
        /// The started tracking event.
        /// </summary>
        Started = 2,

        /// <summary>
        /// The stopped tracking event.
        /// </summary>
        Stopped = 3,

        /// <summary>
        /// The completed tracking event.
        /// </summary>
        Completed = 1
    }
}
