# TorrentClient for .NET Framework

This is an implementation of a torrent peer using the BitTorrent Protocol 1.0 written in C#.

## Using the TorrentClient

 1. Download the source code.
 2. Include project TorrentClient.
 3. Use the following code:

```csharp
public void Run()
{
	TorrentClient torrentClient;
	 
	if (TorrentInfo.TryLoad(@".\TorrentFile.torrent", out TorrentInfo torrent))
	{
		client = new TorrentClient.TorrentClient(4000, @".\Test");
		client.DownloadSpeedLimit = 100 * 1024;
		client.TorrentHashing += TorrentClient_TorrentHashing;
		client.TorrentLeeching += TorrentClient_TorrentLeeching;
		client.TorrentSeeding += TorrentClient_TorrentSeeding;
		client.TorrentStarted += TorrentClient_TorrentStarted;
		client.TorrentStopped += TorrentClient_TorrentStopped;
		client.Start();
		client.Start(torrent);
	}
}

private void TorrentClient_TorrentHashing(object sender, TorrentHashingEventArgs e)
{
	Console.WriteLine("hashing");
}

private void TorrentClient_TorrentLeeching(object sender, TorrentLeechingEventArgs e)
{
}

private void TorrentClient_TorrentSeeding(object sender, TorrentSeedingEventArgs e)
{
	Console.WriteLine("seeding");
}

private void TorrentClient_TorrentStarted(object sender, TorrentStartedEventArgs e)
{
	Console.WriteLine("started");
}

 private void TorrentClient_TorrentStopped(object sender, TorrentStoppedEventArgs e)
{
	Console.WriteLine("stopped");
}
```
## Getting execution details
```csharp
TorrentProgressInfo info = client.GetProgressInfo(torrent.InfoHash);
 
Console.WriteLine($"duration: {info.Duration.ToString(@"hh\\:mm\\:ss", CultureInfo.InvariantCulture)}");
Console.WriteLine($"completed: {(int)Math.Round(info.CompletedPercentage * 100)}%");
Console.WriteLine($"download speed: {info.DownloadSpeed.ToBytes()}/s");
Console.WriteLine($"upload speed: {info.UploadSpeed.ToBytes()}/s");
Console.WriteLine($"downloaded: {info.Downloaded.ToBytes()}");
Console.WriteLine($"uploaded: {info.Uploaded.ToBytes()}");
Console.WriteLine($"seeders: {info.SeederCount})";
Console.WriteLine($"leechers: {info.LeecherCount}");
Console.WriteLine($"duration: {info.Duration.ToString(@"hh\\:mm\\:ss", CultureInfo.InvariantCulture)}");
Console.WriteLine($"completed: {(int)Math.Round(info.CompletedPercentage * 100)}%");
Console.WriteLine($"download speed: {info.DownloadSpeed.ToBytes()}/s");
Console.WriteLine($"upload speed: {info.UploadSpeed.ToBytes()}/s");
Console.WriteLine($"downloaded: {info.Downloaded.ToBytes()}");
Console.WriteLine($"uploaded: {info.Uploaded.ToBytes()}");
Console.WriteLine($"seeders: {info.SeederCount}");
Console.WriteLine($"leechers: {info.LeecherCount}");
```
## Protocol execution
Example of the protocol being executed:
```csharp
creating torrent client
listening port: 4000
base directory: .\Test\
setting download speed limit to 102400B/s
starting torrent client
starting torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0
creating persistence manager for .\Test\
creating torrent manager for torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0
local peer id -AB1100-99D4EC68117C1DBAEA960B3E
starting torrent manager for torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0
torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0 hashing
verifying file .\Test\Torrent\a
starting tracking udp://192.168.0.10:4009/announce for torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0
torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0 started
udp://192.168.0.10:4009/announce -> UdpTrackerAnnounceMessage
udp://192.168.0.10:4009/announce <- UdpTrackerAnnounceResponseMessage
adding seeding peer 192.168.0.10:4000 to torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0
192.168.0.10:4000 -> HandshakeMessage: PeerID = -AB1100-99D4EC68117C1DBAEA960B3E, InfoHash = C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0, FastPeer = False, ExtendedMessaging = False
adding leeching peer 192.168.0.10:54977 to torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0
192.168.0.10:54977 -> HandshakeMessage: PeerID = -AB1100-99D4EC68117C1DBAEA960B3E, InfoHash = C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0, FastPeer = False, ExtendedMessaging = False
192.168.0.10:4000 <- HandshakeMessage: PeerID = -AB1100-99D4EC68117C1DBAEA960B3E, InfoHash = C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0, FastPeer = False, ExtendedMessaging = False
fatal communication error occurred for peer 192.168.0.10:4000 on torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0: Invalid handshake message.
disposing peer 192.168.0.10:4000
disposing peer communicator for 192.168.0.10:4000
received no data from 192.168.0.10:54977
adding leeching peer 192.168.0.10:54982 to torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0
192.168.0.10:54982 -> HandshakeMessage: PeerID = -AB1100-99D4EC68117C1DBAEA960B3E, InfoHash = C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0, FastPeer = False, ExtendedMessaging = False
192.168.0.10:54982 <- BitfieldMessage: Bitfield = 11111111111111111111111111111111111111111111101111101111111111111111111111111111111111101111111111111011111111111111111111111111111111111111111111111111111111111111111111110111111111111111111110111111111111111111111111101111110111011111111111111111111111111111111111101111111111111111111111111111111111111111101111111111111111111111111111111111011111111111111111111111011111111111011111111111111111111111011111111111111111111111111111111111110111111110111111111111111101111111111111111111111111111111111111011111111111111111111110111111011111111111111111111111111111111111111111111011111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111011101111111111111100001111
192.168.0.10:54982 <- HaveMessage: Index = 452
192.168.0.10:54982 <- HaveMessage: Index = 80
192.168.0.10:54982 <- HaveMessage: Index = 674
192.168.0.10:54982 <- HaveMessage: Index = 198
192.168.0.10:54982 <- HaveMessage: Index = 52
192.168.0.10:54982 <- HaveMessage: Index = 375
192.168.0.10:54982 <- HaveMessage: Index = 351
192.168.0.10:54982 <- HaveMessage: Index = 98
192.168.0.10:54982 <- HaveMessage: Index = 509
192.168.0.10:54982 <- HaveMessage: Index = 379
192.168.0.10:54982 <- HaveMessage: Index = 268
192.168.0.10:54982 <- HaveMessage: Index = 171
192.168.0.10:54982 <- HaveMessage: Index = 467
192.168.0.10:54982 <- HaveMessage: Index = 220
192.168.0.10:54982 <- HaveMessage: Index = 534
192.168.0.10:54982 <- HaveMessage: Index = 686
192.168.0.10:54982 <- HaveMessage: Index = 225
192.168.0.10:54982 <- HaveMessage: Index = 543
192.168.0.10:54982 <- HaveMessage: Index = 578
192.168.0.10:54982 <- HaveMessage: Index = 445
192.168.0.10:54982 <- HaveMessage: Index = 229
192.168.0.10:54982 <- HaveMessage: Index = 403
192.168.0.10:54982 <- HaveMessage: Index = 306
192.168.0.10:54982 <- HaveMessage: Index = 42
192.168.0.10:54977 -> InterestedMessage
192.168.0.10:54982 -> InterestedMessage
192.168.0.10:54982 <- UnChokeMessage
192.168.0.10:54977 -> InterestedMessage
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 0, BlockLength = 16384
fatal communication error occurred for peer 192.168.0.10:54977 on torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0: Unable to write data to the transport connection: An established connection was aborted by the software in your host machine.
disposing peer 192.168.0.10:54977
disposing peer communicator for 192.168.0.10:54977
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 16384, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 32768, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 49152, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 65536, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 81920, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 98304, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 114688, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 131072, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 147456, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 163840, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 180224, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 196608, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 212992, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 229376, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 245760, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 262144, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 278528, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 294912, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 311296, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 327680, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 344064, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 360448, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 376832, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 393216, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 409600, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 425984, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 442368, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 458752, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 475136, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 491520, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 507904, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 524288, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 540672, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 557056, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 573440, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 589824, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 606208, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 622592, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 638976, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 655360, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 671744, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 688128, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 704512, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 720896, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 737280, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 753664, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 770048, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 786432, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 802816, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 819200, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 835584, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 851968, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 868352, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 884736, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 901120, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 917504, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 933888, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 950272, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 966656, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 983040, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 999424, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 1015808, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 1, BlockOffset = 1032192, BlockLength = 16384
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 0, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 16384, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 32768, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 49152, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 65536, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 81920, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 98304, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 114688, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 131072, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 147456, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 163840, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 180224, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 196608, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 212992, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 229376, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 278528, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 262144, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 245760, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 294912, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 344064, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 327680, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 311296, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 360448, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 409600, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 393216, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 376832, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 425984, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 475136, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 458752, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 442368, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 491520, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 540672, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 524288, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 507904, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 557056, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 606208, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 589824, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 573440, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 622592, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 671744, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 655360, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 638976, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 688128, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 737280, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 720896, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 704512, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 753664, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 802816, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 786432, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 770048, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 819200, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 868352, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 851968, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 835584, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 884736, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 933888, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 917504, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 901120, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 950272, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 999424, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 983040, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 966656, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 1015808, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 1, BlockOffset = 1032192, BlockData = byte[1048576]
piece 1 completed for torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0
torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0 leeching
192.168.0.10:54982 -> HaveMessage: Index = 1
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 0, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 16384, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 32768, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 49152, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 65536, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 81920, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 98304, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 114688, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 131072, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 147456, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 163840, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 180224, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 196608, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 212992, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 229376, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 245760, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 262144, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 278528, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 294912, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 311296, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 327680, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 344064, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 360448, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 376832, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 393216, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 409600, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 425984, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 442368, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 458752, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 475136, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 491520, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 507904, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 524288, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 540672, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 557056, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 573440, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 589824, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 606208, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 622592, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 638976, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 655360, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 671744, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 688128, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 704512, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 720896, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 737280, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 753664, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 770048, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 786432, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 802816, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 819200, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 835584, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 851968, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 868352, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 884736, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 901120, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 917504, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 933888, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 950272, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 966656, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 983040, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 999424, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 1015808, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 2, BlockOffset = 1032192, BlockLength = 16384
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 0, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 32768, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 65536, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 49152, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 16384, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 81920, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 147456, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 131072, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 114688, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 98304, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 163840, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 229376, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 212992, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 196608, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 180224, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 245760, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 311296, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 294912, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 278528, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 262144, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 327680, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 393216, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 376832, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 360448, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 344064, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 409600, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 475136, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 458752, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 442368, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 425984, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 491520, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 557056, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 540672, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 524288, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 507904, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 573440, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 638976, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 622592, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 606208, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 589824, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 655360, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 720896, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 704512, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 688128, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 671744, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 737280, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 802816, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 786432, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 770048, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 753664, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 819200, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 884736, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 868352, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 851968, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 835584, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 901120, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 966656, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 950272, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 933888, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 917504, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 983040, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 1032192, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 1015808, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 2, BlockOffset = 999424, BlockData = byte[1048576]
piece 2 completed for torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0
torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0 leeching
192.168.0.10:54982 -> HaveMessage: Index = 2
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 0, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 16384, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 32768, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 49152, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 65536, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 81920, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 98304, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 114688, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 131072, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 147456, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 163840, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 180224, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 196608, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 212992, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 229376, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 245760, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 262144, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 278528, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 294912, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 311296, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 327680, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 344064, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 360448, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 376832, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 393216, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 409600, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 425984, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 442368, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 458752, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 475136, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 491520, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 507904, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 524288, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 540672, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 557056, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 573440, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 589824, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 606208, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 622592, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 638976, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 655360, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 671744, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 688128, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 704512, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 720896, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 737280, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 753664, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 770048, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 786432, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 802816, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 819200, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 835584, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 851968, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 868352, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 884736, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 901120, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 917504, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 933888, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 950272, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 966656, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 983040, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 999424, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 1015808, BlockLength = 16384
192.168.0.10:54982 -> RequestMessage: PieceIndex = 3, BlockOffset = 1032192, BlockLength = 16384
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 0, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 16384, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 49152, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 32768, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 65536, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 114688, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 98304, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 81920, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 131072, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 180224, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 163840, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 147456, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 196608, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 245760, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 229376, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 212992, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 262144, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 311296, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 294912, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 278528, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 327680, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 376832, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 360448, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 344064, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 393216, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 442368, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 425984, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 409600, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 458752, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 507904, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 491520, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 475136, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 524288, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 573440, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 557056, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 540672, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 589824, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 638976, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 622592, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 606208, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 655360, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 704512, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 688128, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 671744, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 720896, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 770048, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 753664, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 737280, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 786432, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 835584, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 819200, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 802816, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 851968, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 901120, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 884736, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 868352, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 917504, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 966656, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 950272, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 933888, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 983040, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 1032192, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 1015808, BlockData = byte[1048576]
192.168.0.10:54982 <- PieceMessage: PieceIndex = 3, BlockOffset = 999424, BlockData = byte[1048576]
piece 3 completed for torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0
torrent C4BB6FEC9FE8E0C240302785AE1A3AD2964CEEB0 leeching
192.168.0.10:54982 -> HaveMessage: Index = 3
...
```



