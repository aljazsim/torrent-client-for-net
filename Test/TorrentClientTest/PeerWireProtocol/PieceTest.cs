using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol;

namespace TorrentClient.Test.PeerWireProtocol
{
    /// <summary>
    /// The piece test.
    /// </summary>
    [TestClass]
    public class PieceTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the piece.
        /// </summary>
        [TestMethod]
        public void TestPiece()
        {
            Piece piece;
            byte[] pieceHash = "b6 a2 8b cf cc aa be 37 c3 ea e0 73 c6 d9 f2 a0 4b 46 70 89".Replace(" ", string.Empty).ToByteArray();
            byte[] pieceData;
            byte[] block0Data = new byte[4];
            byte[] block1Data = new byte[4];
            byte[] block2Data = new byte[4];
            byte[] block3Data = new byte[4];

            pieceData = "3e 57 0d 55 18 c1 d4 14 0c f1 3e 97 ce 13 7d 16".Replace(" ", string.Empty).ToByteArray();

            Buffer.BlockCopy(pieceData, 0, block0Data, 0, block0Data.Length);
            Buffer.BlockCopy(pieceData, 4, block1Data, 0, block1Data.Length);
            Buffer.BlockCopy(pieceData, 8, block2Data, 0, block2Data.Length);
            Buffer.BlockCopy(pieceData, 12, block3Data, 0, block3Data.Length);

            piece = new Piece(6, pieceHash.ToHexaDecimalString(), 16, 4, 4);
            piece.Completed += (sender, e) =>
                {
                    Assert.AreEqual(piece, sender);
                    Assert.AreEqual(4, piece.BlockCount);
                    Assert.AreEqual(4, piece.BlockLength);
                    Assert.AreEqual(true, piece.IsCompleted);
                    Assert.AreEqual(false, piece.IsCorrupted);
                    Assert.AreEqual(pieceHash.ToHexaDecimalString(), piece.PieceHash);
                    Assert.AreEqual(6, piece.PieceIndex);
                    Assert.AreEqual(16, piece.PieceData.Length);
                    CollectionAssert.AreEqual(pieceData, e.PieceData);
                    CollectionAssert.AreEqual(new bool[] { true, true, true, true }, piece.BitField);
                };
            piece.Corrupted += (sender, e) =>
                {
                    Assert.Fail();
                };

            Assert.AreEqual(4, piece.BlockCount);
            Assert.AreEqual(4, piece.BlockLength);
            Assert.AreEqual(false, piece.IsCompleted);
            Assert.AreEqual(false, piece.IsCorrupted);
            Assert.AreEqual(pieceHash.ToHexaDecimalString(), piece.PieceHash);
            Assert.AreEqual(6, piece.PieceIndex);
            Assert.AreEqual(16, piece.PieceData.Length);
            CollectionAssert.AreEqual(new bool[] { false, false, false, false }, piece.BitField);

            piece.PutBlock(0, block0Data);

            Assert.AreEqual(4, piece.BlockCount);
            Assert.AreEqual(4, piece.BlockLength);
            Assert.AreEqual(false, piece.IsCompleted);
            Assert.AreEqual(false, piece.IsCorrupted);
            Assert.AreEqual(pieceHash.ToHexaDecimalString(), piece.PieceHash);
            Assert.AreEqual(6, piece.PieceIndex);
            Assert.AreEqual(16, piece.PieceData.Length);
            CollectionAssert.AreEqual(new bool[] { true, false, false, false }, piece.BitField);

            piece.PutBlock(4, block1Data);

            Assert.AreEqual(4, piece.BlockCount);
            Assert.AreEqual(4, piece.BlockLength);
            Assert.AreEqual(false, piece.IsCompleted);
            Assert.AreEqual(false, piece.IsCorrupted);
            Assert.AreEqual(pieceHash.ToHexaDecimalString(), piece.PieceHash);
            Assert.AreEqual(6, piece.PieceIndex);
            Assert.AreEqual(16, piece.PieceData.Length);
            CollectionAssert.AreEqual(new bool[] { true, true, false, false }, piece.BitField);

            piece.PutBlock(8, block2Data);

            Assert.AreEqual(4, piece.BlockCount);
            Assert.AreEqual(4, piece.BlockLength);
            Assert.AreEqual(false, piece.IsCompleted);
            Assert.AreEqual(false, piece.IsCorrupted);
            Assert.AreEqual(pieceHash.ToHexaDecimalString(), piece.PieceHash);
            Assert.AreEqual(6, piece.PieceIndex);
            Assert.AreEqual(16, piece.PieceData.Length);
            CollectionAssert.AreEqual(new bool[] { true, true, true, false }, piece.BitField);

            piece.PutBlock(12, block3Data);

            Assert.AreEqual(4, piece.BlockCount);
            Assert.AreEqual(4, piece.BlockLength);
            Assert.AreEqual(true, piece.IsCompleted);
            Assert.AreEqual(false, piece.IsCorrupted);
            Assert.AreEqual(pieceHash.ToHexaDecimalString(), piece.PieceHash);
            Assert.AreEqual(6, piece.PieceIndex);
            Assert.AreEqual(16, piece.PieceData.Length);
            CollectionAssert.AreEqual(new bool[] { true, true, true, true }, piece.BitField);
        }

        /// <summary>
        /// Tests the piece.
        /// </summary>
        [TestMethod]
        public void TestPieceInvald()
        {
            Piece piece;
            byte[] pieceHash = "b6 a2 8b cf cc aa be 27 c3 ea e0 73 c6 d9 f2 a0 4b 46 70 89".Replace(" ", string.Empty).ToByteArray();
            byte[] pieceData;
            byte[] block0Data = new byte[4];
            byte[] block1Data = new byte[4];
            byte[] block2Data = new byte[4];
            byte[] block3Data = new byte[4];

            pieceData = "3e 57 0d 55 18 c1 d4 14 0c f1 3e 97 ce 13 7d 16".Replace(" ", string.Empty).ToByteArray();

            Buffer.BlockCopy(pieceData, 0, block0Data, 0, block0Data.Length);
            Buffer.BlockCopy(pieceData, 4, block1Data, 0, block1Data.Length);
            Buffer.BlockCopy(pieceData, 8, block2Data, 0, block2Data.Length);
            Buffer.BlockCopy(pieceData, 12, block3Data, 0, block3Data.Length);

            piece = new Piece(6, pieceHash.ToHexaDecimalString(), 16, 4, 4);
            piece.Completed += (sender, e) =>
            {
                Assert.Fail();
            };
            piece.Corrupted += (sender, e) =>
            {
                Assert.AreEqual(piece, sender);
                Assert.AreEqual(4, piece.BlockCount);
                Assert.AreEqual(4, piece.BlockLength);
                Assert.AreEqual(false, piece.IsCompleted);
                Assert.AreEqual(true, piece.IsCorrupted);
                Assert.AreEqual(pieceHash.ToHexaDecimalString(), piece.PieceHash);
                Assert.AreEqual(6, piece.PieceIndex);
                Assert.AreEqual(16, piece.PieceData.Length);
                CollectionAssert.AreEqual(pieceData, piece.PieceData);
            };

            Assert.AreEqual(4, piece.BlockCount);
            Assert.AreEqual(4, piece.BlockLength);
            Assert.AreEqual(false, piece.IsCompleted);
            Assert.AreEqual(false, piece.IsCorrupted);
            Assert.AreEqual(pieceHash.ToHexaDecimalString(), piece.PieceHash);
            Assert.AreEqual(6, piece.PieceIndex);
            Assert.AreEqual(16, piece.PieceData.Length);

            piece.PutBlock(0, block0Data);

            Assert.AreEqual(4, piece.BlockCount);
            Assert.AreEqual(4, piece.BlockLength);
            Assert.AreEqual(false, piece.IsCompleted);
            Assert.AreEqual(false, piece.IsCorrupted);
            Assert.AreEqual(pieceHash.ToHexaDecimalString(), piece.PieceHash);
            Assert.AreEqual(6, piece.PieceIndex);
            Assert.AreEqual(16, piece.PieceData.Length);

            piece.PutBlock(4, block1Data);

            Assert.AreEqual(4, piece.BlockCount);
            Assert.AreEqual(4, piece.BlockLength);
            Assert.AreEqual(false, piece.IsCompleted);
            Assert.AreEqual(false, piece.IsCorrupted);
            Assert.AreEqual(pieceHash.ToHexaDecimalString(), piece.PieceHash);
            Assert.AreEqual(6, piece.PieceIndex);
            Assert.AreEqual(16, piece.PieceData.Length);

            piece.PutBlock(8, block2Data);

            Assert.AreEqual(4, piece.BlockCount);
            Assert.AreEqual(4, piece.BlockLength);
            Assert.AreEqual(false, piece.IsCompleted);
            Assert.AreEqual(false, piece.IsCorrupted);
            Assert.AreEqual(pieceHash.ToHexaDecimalString(), piece.PieceHash);
            Assert.AreEqual(6, piece.PieceIndex);
            Assert.AreEqual(16, piece.PieceData.Length);

            piece.PutBlock(12, block3Data);

            Assert.AreEqual(4, piece.BlockCount);
            Assert.AreEqual(4, piece.BlockLength);
            Assert.AreEqual(false, piece.IsCompleted);
            Assert.AreEqual(true, piece.IsCorrupted);
            Assert.AreEqual(pieceHash.ToHexaDecimalString(), piece.PieceHash);
            Assert.AreEqual(6, piece.PieceIndex);
            Assert.AreEqual(16, piece.PieceData.Length);
        }

        #endregion Public Methods
    }
}