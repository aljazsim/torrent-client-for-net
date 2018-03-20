namespace TorrentClient.PeerWireProtocol
{
	/// <summary>
	/// The handshake state.
	/// </summary>
	public enum HandshakeState
	{
		/// <summary>
		/// The none handshake state.
		/// </summary>
		None = 0,

		/// <summary>
		/// The sent but net received handshake state.
		/// </summary>
		SentButNotReceived = 1,

		/// <summary>
		/// The received but not sent handshake state.
		/// </summary>
		ReceivedButNotSent = 2,

		/// <summary>
		/// The sent and received handshake state.
		/// </summary>
		SendAndReceived = 3
	}
}