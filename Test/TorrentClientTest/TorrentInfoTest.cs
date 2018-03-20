using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TorrentClient.Test
{
    /// <summary>
    /// The TorrentInfo unit test.
    /// </summary>
    [TestClass]
    public class TorrentInfoTest
    {
        #region Public Methods

        /// <summary>
        /// Tests the TorrentInfo_TryLoad() method.
        /// </summary>
        [TestMethod]
        public void TorrentInfo_TryLoad()
        {
            TorrentInfo info;
            string filePath;
            Regex regex = new Regex("[a-zA-Z0-9+/=]{28}", RegexOptions.Compiled);
            int i = 0;

            filePath = @"..\..\..\TorrentClientTest.Data\Torrent1.torrent";

            // multi file
            if (TorrentInfo.TryLoad(filePath, out info))
            {
                Assert.AreEqual(1, info.AnnounceList.Count());
                Assert.AreEqual("http://192.168.1.2:4009/announce", info.AnnounceList.ElementAt(0).AbsoluteUri);
                Assert.AreEqual(null, info.Comment);
                Assert.AreEqual("uTorrent/3320", info.CreatedBy);
                Assert.AreEqual(new DateTime(2014, 1, 26, 16, 1, 33), info.CreationDate);
                Assert.AreEqual(Encoding.ASCII, info.Encoding);
                Assert.AreEqual(36, info.Files.Count());
                Assert.AreEqual(info.Length, info.Files.Sum(x => x.Length));
                Assert.AreEqual(16384, info.BlockLength);
                Assert.AreEqual(4, info.BlocksCount);
                Assert.AreEqual(info.PieceLength / info.BlockLength, info.BlocksCount);
                Assert.AreEqual(772, info.PiecesCount);
                Assert.AreEqual(65536, info.PieceLength);
                Assert.AreEqual(true, info.IsPrivate);
                Assert.AreEqual(772, info.PieceHashes.Count());

                Assert.AreEqual(true, info.Files.ElementAt(i).Download);
                Assert.AreEqual(@"Torrent1\0", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(2946749, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\1", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(179587, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\2", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(2994660, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\3", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(106194, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\4", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(993456, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\5", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(44557, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\6", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(355807, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\7", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(518967, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\8", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(1672937, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\9", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(8001, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\a", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(1352543, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\b", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(62629, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\c", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(2052729, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\d", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(5583717, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\e", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(2630552, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\f", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(72133, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\g", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(999736, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\h", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(510815, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\i", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(155862, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\j", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(207147, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\k", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(75435, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\l", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(6274678, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\m", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(1574206, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\n", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(1834856, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\o", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(13675, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\p", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(1004240, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\q", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(3106766, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\r", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(1009289, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\s", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(3207364, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\t", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(66021, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\u", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(3492239, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\v", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(200822, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\w", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(2077420, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\x", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(2170102, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\y", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(159041, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                Assert.AreEqual(true, info.Files.ElementAt(++i).Download);
                Assert.AreEqual(@"Torrent1\z", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(877610, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                foreach (var hash in info.PieceHashes)
                {
                    Assert.IsTrue(regex.IsMatch(hash));
                }
            }
            else
            {
                Assert.Fail($"Could not parse torrent file {filePath}");
            }
        }

        /// <summary>
        /// Tests the TorrentInfo_TryLoad() method.
        /// </summary>
        [TestMethod]
        public void TorrentInfo_TryLoad2()
        {
            TorrentInfo info;
            string filePath;
            Regex regex = new Regex("[a-zA-Z0-9+/=]{28}", RegexOptions.Compiled);
            int i = 0;

            filePath = @"..\..\..\TorrentClientTest.Data\Torrent2.torrent";

            // single file
            if (TorrentInfo.TryLoad(filePath, out info))
            {
                Assert.AreEqual(1, info.AnnounceList.Count());
                Assert.AreEqual("http://192.168.1.2:4009/announce", info.AnnounceList.ElementAt(0).AbsoluteUri);
                Assert.AreEqual(null, info.Comment);
                Assert.AreEqual("uTorrent/3320", info.CreatedBy);
                Assert.AreEqual(new DateTime(2014, 1, 25, 12, 16, 44), info.CreationDate);
                Assert.AreEqual(Encoding.ASCII, info.Encoding);
                Assert.AreEqual(1, info.Files.Count());
                Assert.AreEqual(info.Length, info.Files.Sum(x => x.Length));
                Assert.AreEqual(16384, info.BlockLength);
                Assert.AreEqual(64, info.BlocksCount);
                Assert.AreEqual(info.PieceLength / info.BlockLength, info.BlocksCount);
                Assert.AreEqual(700, info.PiecesCount);
                Assert.AreEqual(1048576, info.PieceLength);
                Assert.AreEqual(true, info.IsPrivate);
                Assert.AreEqual(700, info.PieceHashes.Count());

                Assert.AreEqual(true, info.Files.ElementAt(i).Download);
                Assert.AreEqual(@"Torrent2\a", info.Files.ElementAt(i).FilePath);
                Assert.AreEqual(734003200, info.Files.ElementAt(i).Length);
                Assert.AreEqual(null, info.Files.ElementAt(i).Md5Hash);

                foreach (var hash in info.PieceHashes)
                {
                    Assert.IsTrue(regex.IsMatch(hash));
                }
            }
            else
            {
                Assert.Fail($"Could not parse torrent file {filePath}.");
            }
        }

        #endregion Public Methods
    }
}