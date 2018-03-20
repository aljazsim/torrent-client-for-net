using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The scrape details.
    /// </summary>
    public class ScrapeDetails
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeDetails" /> class.
        /// </summary>
        /// <param name="seedersCount">The seeders count.</param>
        /// <param name="leechesCount">The leeches count.</param>
        /// <param name="completeCount">The complete count.</param>
        public ScrapeDetails(int seedersCount, int leechesCount, int completeCount)
        {
            seedersCount.MustBeGreaterThanOrEqualTo(0);
            leechesCount.MustBeGreaterThanOrEqualTo(0);
            completeCount.MustBeGreaterThanOrEqualTo(0);

            this.CompleteCount = completeCount;
            this.LeechesCount = leechesCount;
            this.SeedersCount = seedersCount;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="ScrapeDetails"/> class from being created.
        /// </summary>
        private ScrapeDetails()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the complete count.
        /// </summary>
        /// <value>
        /// The complete count.
        /// </value>
        public int CompleteCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the leeches count.
        /// </summary>
        /// <value>
        /// The leeches count.
        /// </value>
        public int LeechesCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the seeders count.
        /// </summary>
        /// <value>
        /// The seeders count.
        /// </value>
        public int SeedersCount
        {
            get;
            private set;
        }

        #endregion Public Properties
    }
}
