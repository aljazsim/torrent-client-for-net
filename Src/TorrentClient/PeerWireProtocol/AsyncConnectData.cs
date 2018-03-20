using System.Net;
using System.Net.Sockets;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient.PeerWireProtocol
{
    /// <summary>
    /// The asynchronous connect data.
    /// </summary>
    public sealed class AsyncConnectData
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncConnectData"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="tcp">The TCP.</param>
        public AsyncConnectData(IPEndPoint endpoint, TcpClient tcp)
        {
            endpoint.CannotBeNull();
            tcp.CannotBeNull();

            this.Endpoint = endpoint;
            this.Tcp = tcp;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="AsyncConnectData"/> class from being created.
        /// </summary>
        private AsyncConnectData()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public IPEndPoint Endpoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the TCP client.
        /// </summary>
        /// <value>
        /// The TCP client.
        /// </value>
        public TcpClient Tcp
        {
            get;
            private set;
        }

        #endregion Public Properties
    }
}
