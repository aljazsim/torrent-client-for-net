using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.TrackerProtocol.Udp.Messages;

namespace TorrentClient.Test.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The scrape message test.
    /// </summary>
    [TestClass]
    public class ScrapeMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the try decode message.
        /// </summary>
        [TestMethod]
        public void ScrapeMessage_TryDecode()
        {
            ScrapeMessage message;
            byte[] data = "0000041727101980 00000002 00003300 551914F642D7E0B5781FB9A9BCF6AC7DE4BDE7ED F5E7C4D5A7BD299CD36CB7547F1C0B8145AE0C9D".Replace(" ", string.Empty).ToByteArray();

            if (ScrapeMessage.TryDecode(data, 0, out message))
            {
                Assert.AreEqual(56, message.Length);
                Assert.AreEqual(2, (int)message.Action);
                Assert.AreEqual(13056, message.TransactionId);
                Assert.AreEqual(2, message.InfoHashes.Count());
                Assert.AreEqual("551914F642D7E0B5781FB9A9BCF6AC7DE4BDE7ED", message.InfoHashes.ElementAt(0));
                Assert.AreEqual("F5E7C4D5A7BD299CD36CB7547F1C0B8145AE0C9D", message.InfoHashes.ElementAt(1));
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