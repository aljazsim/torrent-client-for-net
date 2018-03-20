using System;
using System.Collections.Generic;
using System.Net;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient.TrackerProtocol
{
    /// <summary>
    /// The announced event arguments.
    /// </summary>
    public sealed class AnnouncedEventArgs : EventArgs
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncedEventArgs" /> class.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <param name="leecherCount">The leecher count.</param>
        /// <param name="seederCount">The seeder count.</param>
        /// <param name="peers">The peer endpoints.</param>
        public AnnouncedEventArgs(TimeSpan interval, int leecherCount, int seederCount, IEnumerable<IPEndPoint> peers)
        {
            interval.MustBeGreaterThan(TimeSpan.Zero);
            leecherCount.MustBeGreaterThanOrEqualTo(0);
            seederCount.MustBeGreaterThanOrEqualTo(0);
            peers.CannotContainOnlyNull();

            this.Interval = interval;
            this.LeecherCount = leecherCount;
            this.SeederCount = seederCount;
            this.Peers = peers;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="AnnouncedEventArgs"/> class from being created.
        /// </summary>
        private AnnouncedEventArgs()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public TimeSpan Interval
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the leecher count.
        /// </summary>
        /// <value>
        /// The leecher count.
        /// </value>
        public int LeecherCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the peers.
        /// </summary>
        /// <value>
        /// The peers.
        /// </value>
        public IEnumerable<IPEndPoint> Peers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the seeder count.
        /// </summary>
        /// <value>
        /// The seeder count.
        /// </value>
        public int SeederCount
        {
            get;
            private set;
        }

        #endregion Public Properties
    }
}
