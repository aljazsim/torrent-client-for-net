using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using DefensiveProgrammingFramework;
using TorrentClient.BEncoding;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient.TrackerProtocol.Http.Messages
{
    /// <summary>
    /// The announce message.
    /// </summary>
    public class AnnounceResponseMessage
    {
        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="AnnounceResponseMessage"/> class from being created.
        /// </summary>
        private AnnounceResponseMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnounceResponseMessage" /> class.
        /// </summary>
        /// <param name="failureReason">The failure reason.</param>
        /// <param name="updateInterval">The update interval.</param>
        /// <param name="seedersCount">The seeders count.</param>
        /// <param name="leecherCount">The leecher count.</param>
        /// <param name="peers">The peers.</param>
        private AnnounceResponseMessage(string failureReason, TimeSpan updateInterval, int seedersCount, int leecherCount, IEnumerable<IPEndPoint> peers)
        {
            updateInterval.CannotBeEqualTo(TimeSpan.Zero);
            seedersCount.MustBeGreaterThanOrEqualTo(0);
            leecherCount.MustBeGreaterThanOrEqualTo(0);
            peers.CannotBeNull();

            this.FailureReason = failureReason;
            this.UpdateInterval = updateInterval;
            this.SeederCount = seedersCount;
            this.LeecherCount = leecherCount;
            this.Peers = peers;
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the failure reason.
        /// </summary>
        /// <value>
        /// The failure reason.
        /// </value>
        public string FailureReason
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the leecher count.
        /// </summary>
        /// <value>
        /// The leecher count.
        /// </value>
        public int LeecherCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the peer id / peer endpoint dictionary.
        /// </summary>
        /// <value>
        /// The peers.
        /// </value>
        public IEnumerable<IPEndPoint> Peers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the seeder count.
        /// </summary>
        /// <value>
        /// The seeder count.
        /// </value>
        public int SeederCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the update interval.
        /// </summary>
        /// <value>
        /// The update interval.
        /// </value>
        public TimeSpan UpdateInterval
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the warning message.
        /// </summary>
        /// <value>
        /// The warning message.
        /// </value>
        public string WarningMessage
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Tries to decode the message from the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="message">The message.</param>
        /// <returns>True if decoding was successful; false otherwise.</returns>
        public static bool TryDecode(byte[] data, out AnnounceResponseMessage message)
        {
            BEncodedValue value;
            string faliureReason = null;
            TimeSpan interval = TimeSpan.Zero;
            int complete = -1;
            int incomplete = -1;
            string peerId = null;
            string peerIp = null;
            int peerPort = -1;
            IDictionary<string, IPEndPoint> peers = new Dictionary<string, IPEndPoint>();
            IPAddress tmpIpAddress;
            IPEndPoint endpoint;
            BEncodedString failureReasonKey = new BEncodedString("failure reason");
            BEncodedString intervalKey = new BEncodedString("interval");
            BEncodedString completeKey = new BEncodedString("complete");
            BEncodedString incompleteKey = new BEncodedString("incomplete");
            BEncodedString peersKey = new BEncodedString("peers");
            BEncodedString peerIdKey = new BEncodedString("peer id");
            BEncodedString ipaddressKey = new BEncodedString("ip");
            BEncodedString portKey = new BEncodedString("port");

            message = null;

            if (data.IsNotNullOrEmpty())
            {
                value = BEncodedValue.Decode(data);

                if (value is BEncodedDictionary)
                {
                    if (value.As<BEncodedDictionary>().ContainsKey(failureReasonKey) &&
                        value.As<BEncodedDictionary>()[failureReasonKey] is BEncodedString)
                    {
                        faliureReason = value.As<BEncodedDictionary>()[failureReasonKey].As<BEncodedString>().Text;
                    }

                    if (value.As<BEncodedDictionary>().ContainsKey(intervalKey) &&
                        value.As<BEncodedDictionary>()[intervalKey] is BEncodedNumber)
                    {
                        interval = TimeSpan.FromSeconds(value.As<BEncodedDictionary>()[intervalKey].As<BEncodedNumber>().Number);
                    }
                    else
                    {
                        return false;
                    }

                    if (value.As<BEncodedDictionary>().ContainsKey(completeKey) &&
                        value.As<BEncodedDictionary>()[completeKey] is BEncodedNumber)
                    {
                        complete = (int)value.As<BEncodedDictionary>()[completeKey].As<BEncodedNumber>().Number;
                    }
                    else
                    {
                        return false;
                    }

                    if (value.As<BEncodedDictionary>().ContainsKey(incompleteKey) &&
                        value.As<BEncodedDictionary>()[incompleteKey] is BEncodedNumber)
                    {
                        incomplete = (int)value.As<BEncodedDictionary>()[incompleteKey].As<BEncodedNumber>().Number;
                    }
                    else
                    {
                        return false;
                    }

                    if (value.As<BEncodedDictionary>().ContainsKey(peersKey) &&
                        value.As<BEncodedDictionary>()[peersKey] is BEncodedList)
                    {
                        foreach (var item in value.As<BEncodedDictionary>()[peersKey].As<BEncodedList>())
                        {
                            if (item is BEncodedDictionary)
                            {
                                if (item.As<BEncodedDictionary>().ContainsKey(peerIdKey) &&
                                    item.As<BEncodedDictionary>()[peerIdKey] is BEncodedString &&
                                    item.As<BEncodedDictionary>().ContainsKey(ipaddressKey) &&
                                    item.As<BEncodedDictionary>()[ipaddressKey] is BEncodedString &&
                                    item.As<BEncodedDictionary>().ContainsKey(portKey) &&
                                    item.As<BEncodedDictionary>()[portKey] is BEncodedNumber)
                                {
                                    peerId = Message.ToPeerId(Encoding.ASCII.GetBytes(item.As<BEncodedDictionary>()[peerIdKey].As<BEncodedString>().Text));
                                    peerIp = item.As<BEncodedDictionary>()[ipaddressKey].As<BEncodedString>().Text;
                                    peerPort = (int)item.As<BEncodedDictionary>()[portKey].As<BEncodedNumber>().Number;

                                    if (IPAddress.TryParse(peerIp, out tmpIpAddress) &&
                                        peerPort >= IPEndPoint.MinPort &&
                                        peerPort <= IPEndPoint.MaxPort)
                                    {
                                        endpoint = new IPEndPoint(tmpIpAddress, (ushort)peerPort);

                                        if (!peers.ContainsKey(endpoint.ToString()))
                                        {
                                            peers.Add(endpoint.ToString(), endpoint);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var data2 = item.Encode();

                                for (int i = 0; i < data2.Length; i += 6)
                                {
                                    endpoint = new IPEndPoint(new IPAddress(IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data2, i))), IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data2, i + 4)));

                                    if (!peers.ContainsKey(endpoint.ToString()))
                                    {
                                        peers.Add(endpoint.ToString(), endpoint);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                message = new AnnounceResponseMessage(faliureReason, interval, complete, incomplete, peers.Values);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "HttpTrackerAnnounceResponseMessage";
        }

        #endregion Public Methods
    }
}
