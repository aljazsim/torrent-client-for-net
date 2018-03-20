using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.Test.PeerWireProtocol.Messages
{
    /// <summary>
    /// The interested message test.
    /// </summary>
    [TestClass]
    public class InterestedMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TryDecode() method.
        /// </summary>
        [TestMethod]
        public void InterestedMessage_TryDecode()
        {
            InterestedMessage message;
            int offsetFrom = 0;
            bool isIncomplete;
            byte[] data = "0000000102".ToByteArray();

            if (InterestedMessage.TryDecode(data, ref offsetFrom, data.Length, out message, out isIncomplete))
            {
                Assert.AreEqual(5, message.Length);
                Assert.AreEqual(false, isIncomplete);
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