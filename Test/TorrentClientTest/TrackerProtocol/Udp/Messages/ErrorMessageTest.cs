using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.TrackerProtocol.Udp.Messages;

namespace TorrentClient.Test.TrackerProtocol.Udp.Messages
{
    /// <summary>
    /// The error message test.
    /// </summary>
    [TestClass]
    public class ErrorMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the try decode message.
        /// </summary>
        [TestMethod]
        public void ErrorMessage_TryDecode()
        {
            ErrorMessage message;
            byte[] data = "00000003 00003300 54484953204953204552524F522054455854".Replace(" ", string.Empty).ToByteArray();

            if (ErrorMessage.TryDecode(data, 0, out message))
            {
                Assert.AreEqual(26, message.Length);
                Assert.AreEqual(3, (int)message.Action);
                Assert.AreEqual(13056, message.TransactionId);
                Assert.AreEqual("THIS IS ERROR TEXT", message.ErrorText);
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