using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol;

namespace TorrentClient.Test.PeerWireProtocol
{
    /// <summary>
    /// The piece manager test.
    /// </summary>
    [TestClass]
    public class PieceManagerTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the piece.
        /// </summary>
        [TestMethod]
        public void TestPieceManager()
        {
            PieceManager pieceManager;
            string[] pieceHashes;
            byte[] data;
            byte[] blockData = new byte[2];
            byte[] pieceData = new byte[4];
            Piece piece;
            PieceStatus[] bitField;

            data = "3e 57 0d 55 18 c1 d4 14 0c f1 3e 97 ce 13 7d 16".Replace(" ", string.Empty).ToByteArray();

            pieceHashes = new string[]
            {
                "ab da 6a ad 3b 55 4e 19 04 71 d0 d9 9c 1e 3c 2f 62 94 9a 96".Replace(" ", string.Empty),
                "80 a5 92 ae 03 ef d0 c8 bf b7 09 5b 2f 16 b4 00 07 5b 36 08".Replace(" ", string.Empty),
                "8a 46 51 54 30 f7 34 22 77 fe 24 51 5d 0a 7d dd c8 6b 90 6d".Replace(" ", string.Empty),
                "a7 30 37 bd fe 86 fc 6e 66 8b 18 df 51 a8 65 2a 1f ad ef af".Replace(" ", string.Empty)
            };

            bitField = new PieceStatus[pieceHashes.Count()];

            pieceManager = new PieceManager(data.CalculateSha1Hash().ToHexaDecimalString(), data.Length, pieceHashes, 4, 2, bitField);
            pieceManager.PieceCompleted += (sender, e) =>
                {
                    Assert.IsNotNull(sender);
                    Assert.IsNotNull(e);
                    Assert.AreEqual(pieceManager, sender);

                    Buffer.BlockCopy(data, e.PieceIndex * 4, pieceData, 0, pieceData.Length);

                    CollectionAssert.AreEqual(pieceData, e.PieceData);
                    Assert.AreEqual(PieceStatus.Present, pieceManager.BitField[e.PieceIndex]);

                    if (pieceManager.IsComplete)
                    {
                        CollectionAssert.AreEqual(new PieceStatus[] { PieceStatus.Present, PieceStatus.Present, PieceStatus.Present, PieceStatus.Present }, pieceManager.BitField);
                    }
                };

            CollectionAssert.AreEqual(new PieceStatus[] { PieceStatus.Missing, PieceStatus.Missing, PieceStatus.Missing, PieceStatus.Missing }, pieceManager.BitField);
            Assert.AreEqual(2, pieceManager.BlockCount);
            Assert.AreEqual(2, pieceManager.BlockLength);
            Assert.AreEqual(4, pieceManager.PieceLength);
            Assert.AreEqual(false, pieceManager.IsComplete);

            foreach (var pieceIndex in new int[] { 0, 3, 2, 1 })
            {
                piece = pieceManager.CheckOut(pieceIndex);

                Assert.AreEqual(2, piece.BlockCount);
                Assert.AreEqual(2, piece.BlockLength);
                Assert.AreEqual(false, piece.IsCompleted);
                Assert.AreEqual(false, piece.IsCorrupted);
                Assert.AreEqual(pieceHashes[pieceIndex], piece.PieceHash);
                Assert.AreEqual(pieceIndex, piece.PieceIndex);
                Assert.AreEqual(4, piece.PieceData.Length);
                Assert.AreEqual(PieceStatus.CheckedOut, pieceManager.BitField[pieceIndex]);
                CollectionAssert.AreEqual(new byte[] { 0x0, 0x0, 0x0, 0x0 }, piece.PieceData);

                foreach (var blockOffseet in new int[] { 0, 2 })
                {
                    Buffer.BlockCopy(data, (pieceIndex * 4) + blockOffseet, blockData, 0, blockData.Length);

                    piece.PutBlock(blockOffseet, blockData);

                    Assert.AreEqual(2, piece.BlockCount);
                    Assert.AreEqual(2, piece.BlockLength);
                    Assert.AreEqual(blockOffseet == 2, piece.IsCompleted);
                    Assert.AreEqual(false, piece.IsCorrupted);
                    Assert.AreEqual(pieceHashes[pieceIndex], piece.PieceHash);
                    Assert.AreEqual(pieceIndex, piece.PieceIndex);
                    Assert.AreEqual(4, piece.PieceData.Length);
                }

                Buffer.BlockCopy(data, pieceIndex * 4, pieceData, 0, pieceData.Length);

                CollectionAssert.AreEqual(pieceData, piece.PieceData);
                Assert.AreEqual(PieceStatus.Present, pieceManager.BitField[pieceIndex]);
            }
        }

        #endregion Public Methods
    }
}