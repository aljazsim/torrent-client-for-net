using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol
{
    /// <summary>
    /// The asynchronous read data.
    /// </summary>
    public sealed class AsyncReadData
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncReadData"/> class.
        /// </summary>
        /// <param name="bufferLength">Length of the buffer.</param>
        public AsyncReadData(int bufferLength)
        {
            bufferLength.MustBeGreaterThan(0);

            this.Buffer = new byte[bufferLength];
            this.OffsetStart = 0;
            this.OffsetEnd = 0;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="AsyncReadData"/> class from being created.
        /// </summary>
        private AsyncReadData()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the data buffer.
        /// </summary>
        /// <value>
        /// The data buffer.
        /// </value>
        public byte[] Buffer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the buffer offset end.
        /// </summary>
        /// <value>
        /// The buffer offset end.
        /// </value>
        public int OffsetEnd
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the buffer offset start.
        /// </summary>
        /// <value>
        /// The buffer offset start.
        /// </value>
        public int OffsetStart
        {
            get;
            set;
        }

        #endregion Public Properties
    }
}
