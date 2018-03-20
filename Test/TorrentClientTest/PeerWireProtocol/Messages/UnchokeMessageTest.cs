using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.Test.PeerWireProtocol.Messages
{
    /// <summary>
    /// The un-choke message test.
    /// </summary>
    [TestClass]
    public class UnchokeMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TryDecode() method.
        /// </summary>
        [TestMethod]
        public void UnchokeMessage_TryDecode()
        {
            UnchokeMessage message;
            int offset = 0;
            bool isIncomplete;
            byte[] data = "0000000101".ToByteArray();

            if (UnchokeMessage.TryDecode(data, ref offset, data.Length, out message, out isIncomplete))
            {
                Assert.AreEqual(5, message.Length);
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