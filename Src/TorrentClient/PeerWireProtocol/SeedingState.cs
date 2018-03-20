namespace TorrentClient.PeerWireProtocol
{
    /// <summary>
    /// The seeding state.
    /// </summary>
    public enum SeedingState
    {
        /// <summary>
        /// The choked seed state.
        /// </summary>
        Choked = 1,

        /// <summary>
        /// The un-choked seed state.
        /// </summary>
        Unchoked = 2,
    }
}
