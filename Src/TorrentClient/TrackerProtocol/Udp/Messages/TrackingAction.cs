namespace TorrentClient.TrackerProtocol.Udp.Messages.Messages
{
	/// <summary>
	/// The tracking action
	/// </summary>
	public enum TrackingAction : int
	{
		/// <summary>
		/// The connect tracking action.
		/// </summary>
		Connect = 0,

		/// <summary>
		/// The announce tracking action.
		/// </summary>
		Announce = 1,

		/// <summary>
		/// The scrape tracking action.
		/// </summary>
		Scrape = 2,

		/// <summary>
		/// The error tracking action.
		/// </summary>
		Error = 3
	}
}