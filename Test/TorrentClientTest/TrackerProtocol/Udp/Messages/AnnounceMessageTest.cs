using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.TrackerProtocol.Udp.Messages;

namespace TorrentClient.Test.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The Announce message test.
    /// </summary>
    [TestClass]
    public class AnnounceMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the try decode message.
        /// </summary>
        [TestMethod]
        public void AnnounceMessage_TryDecode()
        {
            AnnounceMessage message;
            byte[] data = "0000041727101980 00000001 00003300 6385736543351543134351535341535355433445 2D5443313031302D313131313131313131313131 0000000000000001 0000000000000002 0000000000000003 00000002 09080706 0000000A 0000000B 1354".Replace(" ", string.Empty).ToByteArray();

            if (AnnounceMessage.TryDecode(data, 0, out message))
            {
                Assert.AreEqual(98, message.Length);
                Assert.AreEqual(1, (int)message.Action);
                Assert.AreEqual(13056, message.TransactionId);
                Assert.AreEqual("6385736543351543134351535341535355433445", message.InfoHash);
                Assert.AreEqual("-TC1010-313131313131313131313131", message.PeerId);
                Assert.AreEqual(1, message.Downloaded);
                Assert.AreEqual(2, message.Left);
                Assert.AreEqual(3, message.Uploaded);
                Assert.AreEqual(2, (int)message.TrackingEvent);
                Assert.AreEqual(IPAddress.Parse("6.7.8.9"), message.Endpoint.Address);
                Assert.AreEqual((uint)0xA, message.Key);
                Assert.AreEqual((int)0xB, message.NumberWanted);
                Assert.AreEqual(0x1354, message.Endpoint.Port);
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