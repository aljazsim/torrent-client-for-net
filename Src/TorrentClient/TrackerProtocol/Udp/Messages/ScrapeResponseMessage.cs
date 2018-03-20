using System.Collections.Generic;
using System.Linq;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;
using TorrentClient.TrackerProtocol.Udp.Messages.Messages;

namespace TorrentClient.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The scrape response message.
    /// </summary>
    public sealed class ScrapeResponseMessage : TrackerMessage
    {
        #region Private Fields

        /// <summary>
        /// The action length in bytes.
        /// </summary>
        private const int ActionLength = 4;

        /// <summary>
        /// The completed length in bytes.
        /// </summary>
        private const int CompletedLength = 4;

        /// <summary>
        /// The leechers length in bytes.
        /// </summary>
        private const int LeechersLength = 4;

        /// <summary>
        /// The seeders length in bytes.
        /// </summary>
        private const int SeedersLength = 4;

        /// <summary>
        /// The transaction identifier length in bytes.
        /// </summary>
        private const int TransactionIdLength = 4;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeResponseMessage"/> class.
        /// </summary>
        public ScrapeResponseMessage()
            : this(0, new List<ScrapeDetails>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeResponseMessage"/> class.
        /// </summary>
        /// <param name="transactionId">The transaction unique identifier.</param>
        /// <param name="scrapes">The scrapes.</param>
        public ScrapeResponseMessage(int transactionId, IEnumerable<ScrapeDetails> scrapes)
            : base(TrackingAction.Scrape, transactionId)
        {
            scrapes.CannotBeNull();

            this.Scrapes = scrapes;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the length in bytes.
        /// </summary>
        /// <value>
        /// The length in bytes.
        /// </value>
        public override int Length
        {
            get
            {
                return ActionLength + TransactionIdLength + (this.Scrapes.Count() * (SeedersLength + CompletedLength + LeechersLength));
            }
        }

        /// <summary>
        /// Gets the scrapes.
        /// </summary>
        /// <value>
        /// The scrapes.
        /// </value>
        public IEnumerable<ScrapeDetails> Scrapes
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Decodes the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// True if decoding was successful; false otherwise.
        /// </returns>
        public static bool TryDecode(byte[] buffer, int offset, out ScrapeResponseMessage message)
        {
            int action;
            int transactionId;
            int seeds;
            int completed;
            int leechers;
            List<ScrapeDetails> scrapeInfo = new List<ScrapeDetails>();

            message = null;

            if (buffer != null &&
                buffer.Length >= offset + ActionLength + TransactionIdLength &&
                offset >= 0)
            {
                action = Message.ReadInt(buffer, ref offset);
                transactionId = Message.ReadInt(buffer, ref offset);

                if (action == (int)TrackingAction.Scrape &&
                    transactionId >= 0)
                {
                    while (offset <= buffer.Length - SeedersLength - CompletedLength - LeechersLength)
                    {
                        seeds = Message.ReadInt(buffer, ref offset);
                        completed = Message.ReadInt(buffer, ref offset);
                        leechers = Message.ReadInt(buffer, ref offset);

                        scrapeInfo.Add(new ScrapeDetails(seeds, leechers, completed));
                    }

                    message = new ScrapeResponseMessage(transactionId, scrapeInfo);
                }
            }

            return message != null;
        }

        /// <summary>
        /// Encodes the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The number of bytes written.</returns>
        public override int Encode(byte[] buffer, int offset)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            offset.MustBeLessThan(buffer.Length);

            int written = offset;

            Message.Write(buffer, ref written, (int)this.Action);
            Message.Write(buffer, ref written, this.TransactionId);

            foreach (var scrape in this.Scrapes)
            {
                Message.Write(buffer, ref written, scrape.SeedersCount);
                Message.Write(buffer, ref written, scrape.CompleteCount);
                Message.Write(buffer, ref written, scrape.LeechesCount);
            }

            return written - offset;
        }

        #endregion Public Methods
    }
}
