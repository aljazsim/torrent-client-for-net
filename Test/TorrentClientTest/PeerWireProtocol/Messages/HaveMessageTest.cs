using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.Test.PeerWireProtocol.Messages
{
    /// <summary>
    /// The have message test.
    /// </summary>
    [TestClass]
    public class HaveMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TryDecode() method.
        /// </summary>
        [TestMethod]
        public void HaveMessage_TryDecode()
        {
            HaveMessage message;
            int offset = 0;
            bool isIncomplete;
            byte[] data = "0000000504000000AA".ToByteArray();

            if (HaveMessage.TryDecode(data, ref offset, data.Length, out message, out isIncomplete))
            {
                Assert.AreEqual(9, message.Length);
                Assert.AreEqual(170, message.PieceIndex);
                Assert.AreEqual(false, isIncomplete);
                Assert.AreEqual(data.Length, offset);
                CollectionAssert.AreEqual(data, message.Encode());
            }
            else
            {
                Assert.Fail();
            }
        }

        #endregion Public Methods
    }
}