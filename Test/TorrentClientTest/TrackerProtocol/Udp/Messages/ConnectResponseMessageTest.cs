using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.TrackerProtocol.Udp.Messages;

namespace TorrentClient.Test.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The connect response message test.
    /// </summary>
    [TestClass]
    public class ConnectResponseMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the try decode message.
        /// </summary>
        [TestMethod]
        public void ConnectResponseMessage_TryDecode()
        {
            ConnectResponseMessage message;
            byte[] data = "00000000000033000000041727101980".ToByteArray();

            if (ConnectResponseMessage.TryDecode(data, 0, out message))
            {
                Assert.AreEqual(16, message.Length);
                Assert.AreEqual(0, (int)message.Action);
                Assert.AreEqual(13056, message.TransactionId);
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