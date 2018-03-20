namespace TorrentClient.PeerWireProtocol
{
    /// <summary>
    /// The piece status enumerator.
    /// </summary>
    public enum PieceStatus : int
    {
        /// <summary>
        /// The missing piece status.
        /// </summary>
        Missing = 0,

        /// <summary>
        /// The checked out piece status.
        /// </summary>
        CheckedOut = 1,

        /// <summary>
        /// The present piece status.
        /// </summary>
        Present = 2,

        /// <summary>
        /// The partially present piece status.
        /// </summary>
        Partial = 3,

        /// <summary>
        /// The ignore piece status.
        /// </summary>
        Ignore = 4
    }
}
