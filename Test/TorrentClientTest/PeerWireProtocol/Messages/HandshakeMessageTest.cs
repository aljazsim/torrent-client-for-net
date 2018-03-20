using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.Test.PeerWireProtocol.Messages
{
    /// <summary>
    /// The handshake message test.
    /// </summary>
    [TestClass]
    public class HandshakeMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TryDecode() method.
        /// </summary>
        [TestMethod]
        public void HandshakeMessage_TryDecode()
        {
            HandshakeMessage message;
            int offset = 0;
            bool isIncomplete;
            byte[] data = "13426974546F7272656E742070726F746F636F6C000000000010000426F4A8F5C078FADE521E41827A40D52230A58C7E2D5554333330302DB9731F4C29D30E7DEA1F9FA7".ToByteArray();

            if (HandshakeMessage.TryDecode(data, ref offset, data.Length, out message, out isIncomplete))
            {
                Assert.AreEqual(68, message.Length);
                Assert.AreEqual(19, message.ProtocolStringLength);
                Assert.AreEqual("BitTorrent protocol", message.ProtocolString);
                Assert.AreEqual("26F4A8F5C078FADE521E41827A40D52230A58C7E", message.InfoHash);
                Assert.AreEqual("-UT3300-B9731F4C29D30E7DEA1F9FA7", message.PeerId);
                Assert.AreEqual(true, message.SupportsExtendedMessaging);
                Assert.AreEqual(true, message.SupportsFastPeer);
                Assert.AreEqual(false, isIncomplete);
                Assert.AreEqual(data.Length, offset);
                CollectionAssert.AreEqual(data, message.Encode());
            }
            else
            {
                Assert.Fail();
            }

            data = "13426974546f7272656e742070726f746f636f6c000000000010000426f4a8f5c078fade521e41827a40d52230a58c7e4d372d382d322d2de675c32a49ad9d9472449c05".ToByteArray();
            offset = 0;

            if (HandshakeMessage.TryDecode(data, ref offset, data.Length, out message, out isIncomplete))
            {
                Assert.AreEqual(68, message.Length);
                Assert.AreEqual("BitTorrent protocol", message.ProtocolString);
                Assert.AreEqual("26F4A8F5C078FADE521E41827A40D52230A58C7E", message.InfoHash);
                Assert.AreEqual("M7-8-2--E675C32A49AD9D9472449C05", message.PeerId);
                Assert.AreEqual(true, message.SupportsExtendedMessaging);
                Assert.AreEqual(true, message.SupportsFastPeer);
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