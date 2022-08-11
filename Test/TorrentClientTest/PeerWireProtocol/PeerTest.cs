using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol;

namespace TorrentClient.Test.PeerWireProtocol
{
    /// <summary>
    /// The peer test.
    /// </summary>
    [TestClass]
    public class PeerTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the peer transfer.
        /// </summary>
        [TestMethod]
        public void TestPeerTransfer()
        {
            TorrentInfo torrent;
            Peer peer;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("192.168.0.10"), 4009);
            TcpClient tcp;
            byte[] data;
            PieceManager pm;
            ThrottlingManager tm;
            PieceStatus[] bitField;

            TorrentInfo.TryLoad(@"C:\Users\aljaz\Desktop\Torrent1\Torrent2.torrent", out torrent);

            tcp = new TcpClient();
            tcp.ReceiveBufferSize = 1024 * 1024;
            tcp.SendBufferSize = 1024 * 1024;
            tcp.Connect(endpoint);

            data = new byte[torrent.Length];

            tm = new ThrottlingManager();
            tm.WriteSpeedLimit = 1024 * 1024;
            tm.ReadSpeedLimit = 1024 * 1024;

            bitField = new PieceStatus[torrent.PiecesCount];

            pm = new PieceManager(torrent.InfoHash, torrent.Length, torrent.PieceHashes, torrent.PieceLength, torrent.BlockLength, bitField);
            pm.PieceCompleted += (sender, e) => Assert.AreEqual(torrent.PieceHashes.ElementAt(e.PieceIndex), e.PieceData.CalculateSha1Hash().ToHexaDecimalString());

            peer = new Peer(new PeerCommunicator(tm, tcp), pm, "-UT3530-B9731F4C29D30E7DEA1F9FA7");
            peer.CommunicationErrorOccurred += (sender, e) => Assert.Fail();

            Thread.Sleep(1000000);
        }

        #endregion Public Methods
    }
}