using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol;
using TorrentClient.PeerWireProtocol.Messages;

namespace TorrentClient
{
    /// <summary>
    /// The peer.
    /// </summary>
    public sealed class Peer : IDisposable
    {
        /// <summary>
        /// The thread locking object.
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// The communicator.
        /// </summary>
        private PeerCommunicator communicator;

        /// <summary>
        /// The download message queue locker
        /// </summary>
        private object downloadMessageQueueLocker = new object();

        /// <summary>
        /// The download message queue.
        /// </summary>
        private Queue<PeerMessage> downloadMessageQueue = new Queue<PeerMessage>();

        /// <summary>
        /// The download speed in bytes per second.
        /// </summary>
        private decimal downloadSpeed = 0;

        /// <summary>
        /// The peer is currently downloading flag.
        /// </summary>
        private bool isDownloading = false;

        /// <summary>
        /// The peer is keeping alive flag.
        /// </summary>
        private bool isKeepingAlive = false;

        /// <summary>
        /// The the peer is currently uploading flag.
        /// </summary>
        private bool isUploading = false;

        /// <summary>
        /// The last message received time.
        /// </summary>
        private DateTime? lastMessageReceivedTime;

        /// <summary>
        /// The last message sent time.
        /// </summary>
        private DateTime? lastMessageSentTime;

        /// <summary>
        /// The local peer identifier.
        /// </summary>
        private string localPeerId;

        /// <summary>
        /// The piece manager.
        /// </summary>
        private PieceManager pieceManager;

        /// <summary>
        /// The previously downloaded byte count.
        /// </summary>
        private long previouslyDownloaded = 0;

        /// <summary>
        /// The previously uploaded byte count.
        /// </summary>
        private long previouslyUploaded = 0;

        /// <summary>
        /// The send message queue locker.
        /// </summary>
        private object sendMessageQueueLocker = new object();

        /// <summary>
        /// The send message queue.
        /// </summary>
        private Queue<PeerMessage> sendMessageQueue = new Queue<PeerMessage>();

        /// <summary>
        /// The stopwatch.
        /// </summary>
        private Stopwatch stopwatch = Stopwatch.StartNew();

        /// <summary>
        /// The upload message queue locker.
        /// </summary>
        private object uploadMessageQueueLocker = new object();

        /// <summary>
        /// The upload message queue.
        /// </summary>
        private Queue<PeerMessage> uploadMessageQueue = new Queue<PeerMessage>();

        /// <summary>
        /// The upload speed in bytes per second.
        /// </summary>
        private decimal uploadSpeed = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Peer" /> class.
        /// </summary>
        /// <param name="communicator">The communicator.</param>
        /// <param name="pieceManager">The piece manager.</param>
        /// <param name="localPeerId">The local peer identifier.</param>
        /// <param name="peerId">The peer identifier.</param>
        public Peer(PeerCommunicator communicator, PieceManager pieceManager, string localPeerId, string peerId = null)
        {
            communicator.CannotBeNull();
            pieceManager.CannotBeNull();
            localPeerId.CannotBeNullOrEmpty();

            this.PeerId = peerId;

            this.localPeerId = localPeerId;

            this.BitField = new bool[pieceManager.PieceCount];

            this.HandshakeState = peerId == null ? HandshakeState.SentButNotReceived : HandshakeState.SendAndReceived;
            this.SeedingState = SeedingState.Choked;
            this.LeechingState = LeechingState.Uninterested;

            this.Downloaded = 0;
            this.Uploaded = 0;

            this.communicator = communicator;
            this.communicator.MessageReceived += this.Communicator_MessageReceived;
            this.communicator.CommunicationError += this.Communicator_CommunicationError;

            this.pieceManager = pieceManager;
            this.pieceManager.PieceCompleted += this.PieceManager_PieceCompleted;

            this.Endpoint = this.communicator.Endpoint;

            this.StartSending();
            this.StartDownloading();
            this.StartUploading();
            this.StartKeepingConnectionAlive();

            // send handshake
            this.EnqueueSendMessage(new HandshakeMessage(this.pieceManager.TorrentInfoHash, localPeerId, HandshakeMessage.ProtocolName));
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Peer"/> class from being created.
        /// </summary>
        private Peer()
        {
        }

        /// <summary>
        /// Occurs when peer communication error has occurred.
        /// </summary>
        public event EventHandler<PeerCommunicationErrorEventArgs> CommunicationErrorOccurred;

        /// <summary>
        /// Gets the bit field.
        /// </summary>
        /// <value>
        /// The bit field.
        /// </value>
        public bool[] BitField
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the downloaded byte count.
        /// </summary>
        /// <value>
        /// The bytes downloaded.
        /// </value>
        public long Downloaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the download speed in bytes per second.
        /// </summary>
        /// <value>
        /// The download speed in bytes per second.
        /// </value>
        public decimal DownloadSpeed
        {
            get
            {
                this.UpdateTrafficParameters(0, 0);

                return this.downloadSpeed;
            }
        }

        /// <summary>
        /// Gets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public IPEndPoint Endpoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the state of the handshake.
        /// </summary>
        /// <value>
        /// The state of the handshake.
        /// </value>
        public HandshakeState HandshakeState
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this object is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this object is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the state of the leeching.
        /// </summary>
        /// <value>
        /// The state of the leeching.
        /// </value>
        public LeechingState LeechingState
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the peer identifier.
        /// </summary>
        /// <value>
        /// The peer identifier.
        /// </value>
        public string PeerId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the state of the seeding.
        /// </summary>
        /// <value>
        /// The state of the seeding.
        /// </value>
        public SeedingState SeedingState
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the uploaded byte count.
        /// </summary>
        /// <value>
        /// The uploaded byte count.
        /// </value>
        public long Uploaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the upload speed in bytes per second.
        /// </summary>
        /// <value>
        /// The upload speed in bytes per second.
        /// </value>
        public decimal UploadSpeed
        {
            get
            {
                this.UpdateTrafficParameters(0, 0);

                return this.uploadSpeed;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.IsDisposed = true;

                Debug.WriteLine($"disposing peer {this.Endpoint}");

                if (this.communicator != null &&
                    !this.communicator.IsDisposed)
                {
                    this.communicator.Dispose();
                    this.communicator = null;
                }
            }
        }

        /// <summary>
        /// Checks if object is disposed.
        /// </summary>
        private void CheckIfObjectIsDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        /// <summary>
        /// Handles the CommunicationError event of the communicator control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Communicator_CommunicationError(object sender, CommunicationErrorEventArgs e)
        {
            this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs(e.ErrorMessage, true));
        }

        /// <summary>
        /// Handles the MessageReceived event of the Communicator control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PeerMessgeReceivedEventArgs"/> instance containing the event data.</param>
        private void Communicator_MessageReceived(object sender, PeerMessgeReceivedEventArgs e)
        {
            this.ProcessRecievedMessage(e.Message);

            this.UpdateTrafficParameters(e.Message.Length, 0);
        }

        /// <summary>
        /// De-queues the download messages.
        /// </summary>
        /// <returns>
        /// The de-queued messages.
        /// </returns>
        private IEnumerable<PeerMessage> DequeueDownloadMessages()
        {
            IEnumerable<PeerMessage> messages;

            lock (this.downloadMessageQueueLocker)
            {
                messages = this.downloadMessageQueue;

                // TODO: optimize this by using a wait handler
                this.downloadMessageQueue = new Queue<PeerMessage>();
            }

            return messages;
        }

        /// <summary>
        /// De-queues the send messages.
        /// </summary>
        /// <returns>
        /// The de-queued messages.
        /// </returns>
        private IEnumerable<PeerMessage> DequeueSendMessages()
        {
            IEnumerable<PeerMessage> messages;

            lock (this.sendMessageQueueLocker)
            {
                messages = this.sendMessageQueue;

                this.sendMessageQueue = new Queue<PeerMessage>();
            }

            return messages;
        }

        /// <summary>
        /// De-queues the upload messages.
        /// </summary>
        /// <returns>
        /// The de-queued messages.
        /// </returns>
        private IEnumerable<PeerMessage> DequeueUploadMessages()
        {
            IEnumerable<PeerMessage> messages;

            lock (this.uploadMessageQueueLocker)
            {
                messages = this.uploadMessageQueue;

                this.uploadMessageQueue = new Queue<PeerMessage>();
            }

            return messages;
        }

        /// <summary>
        /// The downloading thread.
        /// </summary>
        private void Download()
        {
            TimeSpan timeout = TimeSpan.FromMilliseconds(250);
            TimeSpan chokeTimeout = TimeSpan.FromSeconds(10);
            Stopwatch stopwatch = Stopwatch.StartNew();
            PieceMessage pm;
            Piece piece = null;
            bool[] bitFieldData = Array.Empty<bool>();
            byte[] pieceData = Array.Empty<byte>();
            int unchokeMessagesSent = 0;

            if (!this.isDownloading)
            {
                this.isDownloading = true;

                this.communicator.PieceData = new byte[this.pieceManager.PieceLength];

                while (!this.IsDisposed)
                {
                    if (this.pieceManager.IsComplete)
                    {
                        break;
                    }

                    // process messages
                    foreach (PeerMessage message in this.DequeueDownloadMessages())
                    {
                        if (message is PieceMessage)
                        {
                            pm = message as PieceMessage;

                            if (piece != null &&
                                piece.PieceIndex == pm.PieceIndex &&
                                piece.BitField[pm.BlockOffset / this.pieceManager.BlockLength] == false)
                            {
                                // update piece
                                piece.PutBlock(pm.BlockOffset);

                                if (piece.IsCompleted ||
                                    piece.IsCorrupted)
                                {
                                    // remove piece in order to start a next one
                                    piece = null;
                                }
                            }
                        }
                        else if (message is ChokeMessage)
                        {
                            this.SeedingState = SeedingState.Choked;

                            piece = null;
                        }
                        else if (message is UnchokeMessage)
                        {
                            this.SeedingState = SeedingState.Unchoked;

                            unchokeMessagesSent = 0;
                        }
                    }

                    if (this.HandshakeState == HandshakeState.SendAndReceived)
                    {
                        if (this.SeedingState == SeedingState.Choked)
                        {
                            if (stopwatch.Elapsed > chokeTimeout)
                            {
                                // choked -> send interested
                                this.EnqueueSendMessage(new InterestedMessage());

                                if (++unchokeMessagesSent > 10)
                                {
                                    this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs($"Choked for more than {TimeSpan.FromSeconds(chokeTimeout.TotalSeconds * 10)}.", true));
                                }

                                stopwatch.Restart();
                            }
                            else
                            {
                                Thread.Sleep(timeout);
                            }
                        }
                        else if (this.SeedingState == SeedingState.Unchoked)
                        {
                            if (piece == null)
                            {
                                // find a missing piece
                                for (int pieceIndex = 0; pieceIndex < this.BitField.Length; pieceIndex++)
                                {
                                    if (this.pieceManager.BitField[pieceIndex] == PieceStatus.Missing)
                                    {
                                        if (this.BitField[pieceIndex] ||
                                            this.pieceManager.IsEndGame)
                                        {
                                            pieceData = pieceData.Length == this.pieceManager.GetPieceLength(pieceIndex) ? pieceData : new byte[this.pieceManager.GetPieceLength(pieceIndex)];
                                            bitFieldData = bitFieldData.Length == this.pieceManager.GetBlockCount(pieceIndex) ? bitFieldData : new bool[this.pieceManager.GetBlockCount(pieceIndex)];

                                            // check it out
                                            piece = this.pieceManager.CheckOut(pieceIndex, pieceData, bitFieldData);

                                            if (piece != null)
                                            {
                                                this.communicator.PieceData = pieceData;

                                                break;
                                            }
                                        }
                                    }
                                }

                                if (piece != null)
                                {
                                    // request blocks from the missing piece
                                    for (int i = 0; i < piece.BitField.Length; i++)
                                    {
                                        if (!piece.BitField[i])
                                        {
                                            this.EnqueueSendMessage(new RequestMessage(piece.PieceIndex, (int)piece.GetBlockOffset(i), (int)piece.GetBlockLength(piece.GetBlockOffset(i))));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Thread.Sleep(timeout);
                }

                this.isDownloading = false;
            }
        }

        /// <summary>
        /// Enqueues the download message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void EnqueueDownloadMessage(PeerMessage message)
        {
            message.CannotBeNull();

            lock (this.downloadMessageQueueLocker)
            {
                this.downloadMessageQueue.Enqueue(message);
            }
        }

        /// <summary>
        /// Enqueues the send message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void EnqueueSendMessage(PeerMessage message)
        {
            message.CannotBeNull();

            lock (this.sendMessageQueueLocker)
            {
                this.sendMessageQueue.Enqueue(message);
            }
        }

        /// <summary>
        /// Enqueues the upload message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void EnqueueUploadMessage(PeerMessage message)
        {
            message.CannotBeNull();

            lock (this.uploadMessageQueueLocker)
            {
                this.uploadMessageQueue.Enqueue(message);
            }
        }

        /// <summary>
        /// Keeps the connection alive.
        /// </summary>
        private void KeepAlive()
        {
            TimeSpan keepAliveTimeout = TimeSpan.FromSeconds(60);
            TimeSpan timeout = TimeSpan.FromSeconds(10);

            if (!this.isKeepingAlive)
            {
                this.isKeepingAlive = true;

                while (!this.IsDisposed)
                {
                    if (!this.isDownloading &&
                        !this.isUploading)
                    {
                        break;
                    }
                    else if (this.lastMessageSentTime == null &&
                             this.lastMessageReceivedTime == null)
                    {
                        Thread.Sleep(timeout);
                    }
                    else if (DateTime.UtcNow - this.lastMessageSentTime > keepAliveTimeout ||
                             DateTime.UtcNow - this.lastMessageReceivedTime > keepAliveTimeout)
                    {
                        this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs($"No message exchanged in over {keepAliveTimeout}.", true));

                        break;
                    }
                    else
                    {
                        Thread.Sleep(timeout);
                    }
                }

                this.isKeepingAlive = false;
            }
        }

        /// <summary>
        /// Called when peer communication error has occurred.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCommunicationErrorOccurred(object sender, PeerCommunicationErrorEventArgs e)
        {
            sender.CannotBeNull();
            e.CannotBeNull();

            if (this.CommunicationErrorOccurred != null)
            {
                this.CommunicationErrorOccurred(sender, e);
            }
        }

        /// <summary>
        /// Handles the PieceCompleted event of the PieceManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PieceCompletedEventArgs"/> instance containing the event data.</param>
        private void PieceManager_PieceCompleted(object sender, PieceCompletedEventArgs e)
        {
            if (this.HandshakeState == HandshakeState.SendAndReceived)
            {
                this.EnqueueSendMessage(new HaveMessage(e.PieceIndex));
            }
        }

        /// <summary>
        /// Processes the received message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ProcessRecievedMessage(PeerMessage message)
        {
            this.CheckIfObjectIsDisposed();

            lock (this.locker)
            {
                Debug.WriteLine($"{this.Endpoint} <- {message}");

                this.lastMessageReceivedTime = DateTime.UtcNow;

                if (message is HandshakeMessage)
                {
                    this.ProcessRecievedMessage(message as HandshakeMessage);
                }
                else if (message is ChokeMessage)
                {
                    this.ProcessRecievedMessage(message as ChokeMessage);
                }
                else if (message is UnchokeMessage)
                {
                    this.ProcessRecievedMessage(message as UnchokeMessage);
                }
                else if (message is InterestedMessage)
                {
                    this.ProcessRecievedMessage(message as InterestedMessage);
                }
                else if (message is UninterestedMessage)
                {
                    this.ProcessRecievedMessage(message as UninterestedMessage);
                }
                else if (message is HaveMessage)
                {
                    this.ProcessRecievedMessage(message as HaveMessage);
                }
                else if (message is BitFieldMessage)
                {
                    this.ProcessRecievedMessage(message as BitFieldMessage);
                }
                else if (message is RequestMessage)
                {
                    this.ProcessRecievedMessage(message as RequestMessage);
                }
                else if (message is PieceMessage)
                {
                    this.ProcessRecievedMessage(message as PieceMessage);
                }
                else if (message is CancelMessage)
                {
                    this.ProcessRecievedMessage(message as CancelMessage);
                }
                else if (message is PortMessage)
                {
                    // TODO
                }
                else if (message is KeepAliveMessage)
                {
                    // do nothing
                }
            }
        }

        /// <summary>
        /// Processes the received message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ProcessRecievedMessage(CancelMessage message)
        {
            message.CannotBeNull();

            if (this.HandshakeState == HandshakeState.SendAndReceived)
            {
                if (message.PieceIndex >= 0 &&
                    message.PieceIndex < this.pieceManager.PieceCount &&
                    message.BlockOffset >= 0 &&
                    message.BlockOffset < this.pieceManager.PieceLength &&
                    message.BlockOffset % this.pieceManager.BlockLength == 0 &&
                    message.BlockLength == this.pieceManager.GetBlockLength(message.PieceIndex, message.BlockOffset / this.pieceManager.BlockLength))
                {
                    this.EnqueueUploadMessage(message);
                }
                else
                {
                    this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid cancel message.", false));
                }
            }
            else
            {
                this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid message sequence.", true));
            }
        }

        /// <summary>
        /// Processes the received message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ProcessRecievedMessage(PieceMessage message)
        {
            message.CannotBeNull();

            if (this.HandshakeState == HandshakeState.SendAndReceived)
            {
                if (message.PieceIndex >= 0 &&
                    message.PieceIndex < this.pieceManager.PieceCount &&
                    message.BlockOffset >= 0 &&
                    message.BlockOffset < this.pieceManager.PieceLength &&
                    message.BlockOffset % this.pieceManager.BlockLength == 0 &&
                    message.Data.Length == this.pieceManager.GetPieceLength(message.PieceIndex))
                {
                    this.EnqueueDownloadMessage(message);
                }
                else
                {
                    this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid piece message.", false));
                }
            }
            else
            {
                this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid message sequence.", true));
            }
        }

        /// <summary>
        /// Processes the received message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ProcessRecievedMessage(RequestMessage message)
        {
            message.CannotBeNull();

            if (this.HandshakeState == HandshakeState.SendAndReceived)
            {
                if (message.PieceIndex >= 0 &&
                    message.PieceIndex < this.pieceManager.BlockCount &&
                    message.BlockOffset >= 0 &&
                    message.BlockOffset < this.pieceManager.GetBlockCount(message.PieceIndex) &&
                    message.BlockOffset / this.pieceManager.BlockLength == 0 &&
                    message.BlockLength == this.pieceManager.GetBlockLength(message.PieceIndex, message.BlockOffset / this.pieceManager.BlockLength))
                {
                    this.EnqueueUploadMessage(message);
                }
                else
                {
                    this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid request message.", false));
                }
            }
            else
            {
                this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid message sequence.", true));
            }
        }

        /// <summary>
        /// Processes the received message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ProcessRecievedMessage(BitFieldMessage message)
        {
            message.CannotBeNull();

            if (this.HandshakeState == HandshakeState.SendAndReceived)
            {
                if (message.BitField.Length >= this.pieceManager.BlockCount)
                {
                    for (int i = 0; i < this.BitField.Length; i++)
                    {
                        this.BitField[i] = message.BitField[i];
                    }

                    // notify downloading thread
                    this.EnqueueDownloadMessage(message);
                }
                else
                {
                    this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid bit field message.", true));
                }
            }
            else
            {
                this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid message sequence.", true));
            }
        }

        /// <summary>
        /// Processes the received message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ProcessRecievedMessage(ChokeMessage message)
        {
            message.CannotBeNull();

            if (this.HandshakeState == HandshakeState.SendAndReceived)
            {
                this.EnqueueDownloadMessage(message);
            }
            else
            {
                this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid message sequence.", true));
            }
        }

        /// <summary>
        /// Processes the received message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ProcessRecievedMessage(UnchokeMessage message)
        {
            message.CannotBeNull();

            if (this.HandshakeState == HandshakeState.SendAndReceived)
            {
                this.EnqueueDownloadMessage(message);
            }
            else
            {
                this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid message sequence.", true));
            }
        }

        /// <summary>
        /// Processes the received message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ProcessRecievedMessage(InterestedMessage message)
        {
            message.CannotBeNull();

            if (this.HandshakeState == HandshakeState.SendAndReceived)
            {
                this.EnqueueDownloadMessage(message);
            }
            else
            {
                this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid message sequence.", true));
            }
        }

        /// <summary>
        /// Processes the received message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ProcessRecievedMessage(UninterestedMessage message)
        {
            message.CannotBeNull();

            if (this.HandshakeState == HandshakeState.SendAndReceived)
            {
                this.EnqueueUploadMessage(message);
            }
            else
            {
                this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid message sequence.", true));
            }
        }

        /// <summary>
        /// Processes the received message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ProcessRecievedMessage(HaveMessage message)
        {
            message.CannotBeNull();

            if (this.HandshakeState == HandshakeState.SendAndReceived)
            {
                if (message.PieceIndex >= 0 &&
                    message.PieceIndex < this.pieceManager.PieceCount)
                {
                    this.BitField[message.PieceIndex] = true;

                    // notify downloading thread
                    this.EnqueueDownloadMessage(message);
                }
                else
                {
                    this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid have message.", false));
                }
            }
            else
            {
                this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid message sequence.", true));
            }
        }

        /// <summary>
        /// Processes the received message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ProcessRecievedMessage(HandshakeMessage message)
        {
            message.CannotBeNull();

            if (this.HandshakeState == HandshakeState.None ||
                this.HandshakeState == HandshakeState.SentButNotReceived)
            {
                if (message.InfoHash == this.pieceManager.TorrentInfoHash &&
                    message.ProtocolString == HandshakeMessage.ProtocolName &&
                    message.PeerId.IsNotNullOrEmpty() &&
                    message.PeerId != this.localPeerId)
                {
                    if (this.HandshakeState == HandshakeState.None)
                    {
                        this.HandshakeState = HandshakeState.ReceivedButNotSent;
                        this.PeerId = message.PeerId;

                        // send a handshake
                        this.EnqueueSendMessage(new HandshakeMessage(this.pieceManager.TorrentInfoHash, this.localPeerId));

                        // send a bit field
                        this.EnqueueSendMessage(new BitFieldMessage(this.pieceManager.BitField.Select(x => x == PieceStatus.Present).ToArray()));
                    }
                    else if (this.HandshakeState == HandshakeState.SentButNotReceived)
                    {
                        this.HandshakeState = HandshakeState.SendAndReceived;
                        this.PeerId = message.PeerId;

                        // send a bit field
                        this.EnqueueSendMessage(new BitFieldMessage(this.pieceManager.BitField.Select(x => x == PieceStatus.Present).ToArray()));
                    }
                }
                else
                {
                    this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid handshake message.", true));
                }
            }
            else
            {
                this.OnCommunicationErrorOccurred(this, new PeerCommunicationErrorEventArgs("Invalid message sequence.", true));
            }
        }

        /// <summary>
        /// The message sending thread.
        /// </summary>
        private void Send()
        {
            TimeSpan timeout = TimeSpan.FromMilliseconds(250);
            IEnumerable<PeerMessage> messages;

            while (!this.IsDisposed)
            {
                messages = this.DequeueSendMessages();

                if (messages.Count() > 0)
                {
                    if (this.communicator != null &&
                        !this.communicator.IsDisposed)
                    {
                        foreach (var message in messages)
                        {
                            Debug.WriteLine($"{this.Endpoint} -> {message}");
                        }

                        // send message
                        this.communicator.Send(messages);

                        this.UpdateTrafficParameters(0, messages.Sum(x => x.Length));
                    }
                }

                this.lastMessageSentTime = DateTime.UtcNow;

                Thread.Sleep(timeout);
            }
        }

        /// <summary>
        /// Starts the downloading.
        /// </summary>
        private void StartDownloading()
        {
            Thread thread;

            if (!this.isDownloading)
            {
                thread = new Thread(this.Download);
                thread.IsBackground = true;
                thread.Name = this.PeerId + " downloader";
                thread.Start();
            }
        }

        /// <summary>
        /// Starts the keeping connection alive.
        /// </summary>
        private void StartKeepingConnectionAlive()
        {
            Thread thread;

            if (this.isDownloading ||
                this.isUploading)
            {
                thread = new Thread(this.KeepAlive);
                thread.IsBackground = true;
                thread.Name = this.PeerId + " keeping alive";
                thread.Start();
            }
        }

        /// <summary>
        /// Starts the sending.
        /// </summary>
        private void StartSending()
        {
            Thread thread;

            if (!this.isDownloading)
            {
                thread = new Thread(this.Send);
                thread.IsBackground = true;
                thread.Name = this.PeerId + " sender";
                thread.Start();
            }
        }

        /// <summary>
        /// Starts the uploading.
        /// </summary>
        private void StartUploading()
        {
            Thread thread;

            if (!this.isUploading)
            {
                thread = new Thread(this.Upload);
                thread.IsBackground = true;
                thread.Name = this.PeerId + "uploader";
                thread.Start();
            }
        }

        /// <summary>
        /// Updates the traffic parameters.
        /// </summary>
        /// <param name="downloaded">The downloaded byte count.</param>
        /// <param name="uploaded">The uploaded byte count.</param>
        private void UpdateTrafficParameters(long downloaded, long uploaded)
        {
            downloaded.MustBeGreaterThanOrEqualTo(0);
            uploaded.MustBeGreaterThanOrEqualTo(0);

            lock (this.locker)
            {
                this.previouslyDownloaded += downloaded;
                this.previouslyUploaded += uploaded;

                if (this.stopwatch.Elapsed > TimeSpan.FromSeconds(1))
                {
                    this.downloadSpeed = (decimal)this.previouslyDownloaded / (decimal)this.stopwatch.Elapsed.TotalSeconds;
                    this.uploadSpeed = (decimal)this.previouslyUploaded / (decimal)this.stopwatch.Elapsed.TotalSeconds;

                    this.Downloaded += this.previouslyDownloaded;
                    this.Uploaded += this.previouslyUploaded;

                    this.previouslyDownloaded = 0;
                    this.previouslyUploaded = 0;

                    this.stopwatch.Restart();
                }
            }
        }

        /// <summary>
        /// The uploading thread.
        /// </summary>
        private void Upload()
        {
            TimeSpan timeout = TimeSpan.FromMilliseconds(250);
            Piece piece = null;
            RequestMessage rm;

            if (!this.isUploading)
            {
                this.isUploading = true;

                while (!this.IsDisposed)
                {
                    foreach (PeerMessage message in this.DequeueUploadMessages())
                    {
                        if (message is RequestMessage)
                        {
                            rm = message as RequestMessage;

                            if (piece == null ||
                                piece.PieceIndex != rm.PieceIndex)
                            {
                                // get the piece
                                piece = this.pieceManager.GetPiece(rm.PieceIndex);
                            }

                            if (piece != null &&
                                piece.PieceLength > rm.BlockOffset)
                            {
                                // return the piece
                                this.EnqueueSendMessage(new PieceMessage(rm.PieceIndex, rm.BlockOffset, (int)piece.GetBlockLength(rm.PieceIndex), piece.GetBlock(rm.PieceIndex)));
                            }
                            else
                            {
                                // invalid requeste received -> ignore
                            }
                        }
                        else if (message is CancelMessage)
                        {
                            // TODO
                        }
                        else if (message is InterestedMessage)
                        {
                            this.LeechingState = LeechingState.Interested;
                        }
                        else if (message is UninterestedMessage)
                        {
                            this.LeechingState = LeechingState.Uninterested;
                        }
                    }

                    Thread.Sleep(timeout);
                }

                this.isUploading = false;
            }
        }
    }
}
