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
    public class ScrapeResponseMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the try decode message.
        /// </summary>
        [TestMethod]
        public void ScrapeResponseMessage_TryDecode()
        {
            ScrapeResponseMessage message;
            byte[] data = "00000002 00003300 00000001 00000002 00000003 00000004 00000005 00000006".Replace(" ", string.Empty).ToByteArray();

            if (ScrapeResponseMessage.TryDecode(data, 0, out message))
            {
                Assert.AreEqual(32, message.Length);
                Assert.AreEqual(2, (int)message.Action);
                Assert.AreEqual(13056, message.TransactionId);
                Assert.AreEqual(2, message.Scrapes.Count());
                Assert.AreEqual(1, message.Scrapes.ElementAt(0).SeedersCount);
                Assert.AreEqual(2, message.Scrapes.ElementAt(0).CompleteCount);
                Assert.AreEqual(3, message.Scrapes.ElementAt(0).LeechesCount);
                Assert.AreEqual(4, message.Scrapes.ElementAt(1).SeedersCount);
                Assert.AreEqual(5, message.Scrapes.ElementAt(1).CompleteCount);
                Assert.AreEqual(6, message.Scrapes.ElementAt(1).LeechesCount);
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