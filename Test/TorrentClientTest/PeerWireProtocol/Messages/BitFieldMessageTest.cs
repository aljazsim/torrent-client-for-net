using Microsoft.VisualStudio.TestTools.UnitTesting;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.Test.PeerWireProtocol.Messages
{
    /// <summary>
    /// The un-choke message test.
    /// </summary>
    [TestClass]
    public class BitFieldMessageTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TryDecode() method.
        /// </summary>
        [TestMethod]
        public void TestTryDecodeBitfieldMessage()
        {
            BitFieldMessage message;
            int offset = 0;
            byte[] data = "0000000d05fffffffffffffffffffffff8".ToByteArray();
            bool isIncomplete;

            if (BitFieldMessage.TryDecode(data, ref offset, data.Length, out message, out isIncomplete))
            {
                Assert.AreEqual(17, message.Length);
                Assert.AreEqual(96, message.BitField.Length);
                Assert.AreEqual(true, message.BitField[0]);
                Assert.AreEqual(false, isIncomplete);
                Assert.AreEqual(data.Length, offset);

                for (int i = 0; i < message.BitField.Length; i++)
                {
                    if (i == 88 ||
                        i == 89 ||
                        i == 90)
                    {
                        Assert.IsFalse(message.BitField[i]);
                    }
                    else
                    {
                        Assert.IsTrue(message.BitField[i]);
                    }
                }

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