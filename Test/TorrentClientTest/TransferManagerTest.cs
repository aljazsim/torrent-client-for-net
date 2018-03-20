using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;

namespace TorrentClient.Test
{
    /// <summary>
    /// The transfer manager test.
    /// </summary>
    [TestClass]
    public class TransferManagerTest
    {
        #region Public Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            @"Test\".DeleteDirectoryRecursively();
        }

        /// <summary>
        /// Tests the peer transfer.
        /// </summary>
        [TestMethod]
        public void TestTransferManager()
        {
            TorrentInfo torrent;
            PersistenceManager pm;
            ThrottlingManager tm;
            TransferManager transfer;

            TorrentInfo.TryLoad(@"C:\Users\aljaz\Desktop\Torrent1\Torrent2.torrent", out torrent);

            tm = new ThrottlingManager();
            tm.WriteSpeedLimit = 1024 * 1024;
            tm.ReadSpeedLimit = 1024 * 1024;

            pm = new PersistenceManager(@"Test\", torrent.Length, torrent.PieceLength, torrent.PieceHashes, torrent.Files);

            transfer = new TransferManager(4000, torrent, tm, pm);
            transfer.Start();

            Thread.Sleep(1000000);
        }

        #endregion Public Methods
    }
}