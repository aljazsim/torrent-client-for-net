using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DefensiveProgrammingFramework;
using TorrentClient.Exceptions;
using TorrentClient.Extensions;
using TorrentClient.PeerWireProtocol;

namespace TorrentClient
{
    /// <summary>
    /// The persistence manager.
    /// </summary>
    public sealed class PersistenceManager : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// The file info / file stream dictionary.
        /// </summary>
        private Dictionary<TorrentFileInfo, FileStream> files;

        /// <summary>
        /// The thread locker.
        /// </summary>
        private object locker = new object();

        /// <summary>
        /// The piece hashes.
        /// </summary>
        private IEnumerable<string> pieceHashes;

        /// <summary>
        /// The piece length.
        /// </summary>
        private long pieceLength;

        /// <summary>
        /// The torrent length in bytes.
        /// </summary>
        private long torrentLength;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceManager" /> class.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <param name="torrentLength">Length of the torrent.</param>
        /// <param name="pieceLength">Length of the piece.</param>
        /// <param name="pieceHashes">The piece hashes.</param>
        /// <param name="files">The files.</param>
        public PersistenceManager(string directoryPath, long torrentLength, long pieceLength, IEnumerable<string> pieceHashes, IEnumerable<TorrentFileInfo> files)
        {
            directoryPath.CannotBeNullOrEmpty();
            directoryPath.MustBeValidDirectoryPath();
            files.CannotBeNullOrEmpty();
            pieceLength.MustBeGreaterThan(0);
            pieceHashes.CannotBeNullOrEmpty();

            Debug.WriteLine($"creating persistence manager for {Path.GetFullPath(directoryPath)}");

            this.DirectoryPath = directoryPath;
            this.torrentLength = torrentLength;
            this.pieceLength = pieceLength;
            this.pieceHashes = pieceHashes;

            // initialize file handlers
            this.files = new Dictionary<TorrentFileInfo, FileStream>();

            foreach (var file in files)
            {
                if (file.Download)
                {
                    this.CreateFile(Path.Combine(this.DirectoryPath, file.FilePath), file.Length);

                    this.files.Add(file, new FileStream(Path.Combine(this.DirectoryPath, file.FilePath), FileMode.Open, FileAccess.ReadWrite, FileShare.None));
                }
            }
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="PersistenceManager"/> class from being created.
        /// </summary>
        private PersistenceManager()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the directory path.
        /// </summary>
        /// <value>
        /// The directory path.
        /// </value>
        public string DirectoryPath
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the object is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if object is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.IsDisposed = true;

                Debug.WriteLine($"disposing persistence manager for {this.DirectoryPath}");

                foreach (var file in this.files)
                {
                    file.Value.Close();
                    file.Value.Dispose();
                }

                this.files.Clear();
            }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="pieceIndex">Index of the piece.</param>
        /// <returns>
        /// The piece data.
        /// </returns>
        public byte[] Get(int pieceIndex)
        {
            pieceIndex.MustBeGreaterThanOrEqualTo(0);

            long pieceStart;
            long pieceEnd;
            long torrentStartOffset = 0;
            long torrentEndOffset;
            long fileOffset;
            byte[] pieceData;
            int pieceOffset = 0;
            int length = 0;

            this.CheckIfObjectIsDisposed();

            lock (this.locker)
            {
                // calculate length of the data read (it could be less than the specified piece length)
                foreach (var file in this.files)
                {
                    torrentEndOffset = torrentStartOffset + file.Key.Length;

                    pieceStart = (torrentStartOffset - (torrentStartOffset % this.pieceLength)) / this.pieceLength;

                    pieceEnd = (torrentEndOffset - (torrentEndOffset % this.pieceLength)) / this.pieceLength;
                    pieceEnd -= torrentEndOffset % this.pieceLength == 0 ? 1 : 0;

                    if (pieceIndex >= pieceStart &&
                        pieceIndex <= pieceEnd)
                    {
                        fileOffset = (pieceIndex - pieceStart) * this.pieceLength;
                        fileOffset -= pieceIndex > pieceStart ? torrentStartOffset % this.pieceLength : 0;

                        length += (int)Math.Min(this.pieceLength - length, file.Key.Length - fileOffset);
                    }
                    else if (pieceIndex < pieceStart)
                    {
                        break;
                    }

                    torrentStartOffset += file.Key.Length;
                }

                if (length > 0)
                {
                    pieceData = new byte[length];
                    torrentStartOffset = 0;
                    length = 0;

                    // read the piece
                    foreach (var file in this.files)
                    {
                        torrentEndOffset = torrentStartOffset + file.Key.Length;

                        pieceStart = (torrentStartOffset - (torrentStartOffset % this.pieceLength)) / this.pieceLength;

                        pieceEnd = (torrentEndOffset - (torrentEndOffset % this.pieceLength)) / this.pieceLength;
                        pieceEnd -= torrentEndOffset % this.pieceLength == 0 ? 1 : 0;

                        if (pieceIndex >= pieceStart &&
                            pieceIndex <= pieceEnd)
                        {
                            fileOffset = (pieceIndex - pieceStart) * this.pieceLength;
                            fileOffset -= pieceIndex > pieceStart ? torrentStartOffset % this.pieceLength : 0;

                            length = (int)Math.Min(this.pieceLength - pieceOffset, file.Key.Length - fileOffset);

                            if (file.Key.Download)
                            {
                                this.Read(file.Value, fileOffset, length, pieceData, pieceOffset);
                            }

                            pieceOffset += length;
                        }
                        else if (pieceIndex < pieceStart)
                        {
                            break;
                        }

                        torrentStartOffset = torrentEndOffset;
                    }
                }
                else
                {
                    throw new TorrentPersistanceException("File cannot be empty.");
                }
            }

            return pieceData;
        }

        /// <summary>
        /// Puts the data.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="pieceLength">Length of the piece.</param>
        /// <param name="pieceIndex">Index of the piece.</param>
        /// <param name="pieceData">The piece data.</param>
        public void Put(IEnumerable<TorrentFileInfo> files, long pieceLength, long pieceIndex, byte[] pieceData)
        {
            files.CannotBeNullOrEmpty();
            pieceLength.MustBeGreaterThan(0);
            pieceIndex.MustBeGreaterThanOrEqualTo(0);
            pieceData.CannotBeNullOrEmpty();

            long pieceStart;
            long pieceEnd;
            long torrentStartOffset = 0;
            long torrentEndOffset = 0;
            long fileOffset;
            int pieceOffset = 0;
            int length = 0;

            this.CheckIfObjectIsDisposed();

            lock (this.locker)
            {
                // verify length of the data written
                foreach (var file in files)
                {
                    torrentEndOffset = torrentStartOffset + file.Length;

                    pieceStart = (torrentStartOffset - (torrentStartOffset % pieceLength)) / pieceLength;

                    pieceEnd = (torrentEndOffset - (torrentEndOffset % pieceLength)) / pieceLength;
                    pieceEnd -= torrentEndOffset % pieceLength == 0 ? 1 : 0;

                    if (pieceIndex >= pieceStart &&
                        pieceIndex <= pieceEnd)
                    {
                        fileOffset = (pieceIndex - pieceStart) * pieceLength;
                        fileOffset -= pieceIndex > pieceStart ? torrentStartOffset % pieceLength : 0;

                        length += (int)Math.Min(pieceLength - length, file.Length - fileOffset);
                    }
                    else if (pieceIndex < pieceStart)
                    {
                        break;
                    }

                    torrentStartOffset = torrentEndOffset;
                }

                if (length == pieceData.Length)
                {
                    torrentStartOffset = 0;
                    length = 0;

                    // write the piece
                    foreach (var file in this.files)
                    {
                        torrentEndOffset = torrentStartOffset + file.Key.Length;

                        pieceStart = (torrentStartOffset - (torrentStartOffset % pieceLength)) / pieceLength;

                        pieceEnd = (torrentEndOffset - (torrentEndOffset % pieceLength)) / pieceLength;
                        pieceEnd -= torrentEndOffset % pieceLength == 0 ? 1 : 0;

                        if (pieceIndex >= pieceStart &&
                            pieceIndex <= pieceEnd)
                        {
                            fileOffset = (pieceIndex - pieceStart) * pieceLength;
                            fileOffset -= pieceIndex > pieceStart ? torrentStartOffset % pieceLength : 0;

                            length = (int)Math.Min(pieceLength - pieceOffset, file.Key.Length - fileOffset);

                            if (file.Key.Download)
                            {
                                this.Write(file.Value, fileOffset, length, pieceData, pieceOffset);
                            }

                            pieceOffset += length;
                        }
                        else if (pieceIndex < pieceStart)
                        {
                            break;
                        }

                        torrentStartOffset = torrentEndOffset;
                    }
                }
                else
                {
                    throw new TorrentPersistanceException("Invalid length.");
                }
            }
        }

        /// <summary>
        /// Verifies the specified file if it corresponds with the piece hashes.
        /// </summary>
        /// <returns>
        /// The bit field.
        /// </returns>
        public PieceStatus[] Verify()
        {
            PieceStatus[] bitField = new PieceStatus[this.pieceHashes.Count()];
            long pieceStart;
            long pieceEnd;
            long previousPieceIndex = 0;
            long torrentStartOffset = 0;
            long torrentEndOffset;
            long fileOffset;
            byte[] pieceData = new byte[this.pieceLength];
            int pieceOffset = 0;
            int length = 0;
            bool ignore = false;
            bool download = false;

            this.CheckIfObjectIsDisposed();

            lock (this.locker)
            {
                foreach (var file in this.files)
                {
                    torrentEndOffset = torrentStartOffset + file.Key.Length;

                    pieceStart = (torrentStartOffset - (torrentStartOffset % this.pieceLength)) / this.pieceLength;

                    pieceEnd = (torrentEndOffset - (torrentEndOffset % this.pieceLength)) / this.pieceLength;
                    pieceEnd -= torrentEndOffset % this.pieceLength == 0 ? 1 : 0;

                    Debug.WriteLine($"verifying file {file.Value.Name}");

                    for (long pieceIndex = pieceStart; pieceIndex <= pieceEnd; pieceIndex++)
                    {
                        if (pieceIndex > previousPieceIndex)
                        {
                            bitField[previousPieceIndex] = this.GetStatus(ignore, download, this.pieceHashes.ElementAt((int)previousPieceIndex), pieceData.CalculateSha1Hash(0, pieceOffset).ToHexaDecimalString());

                            previousPieceIndex = pieceIndex;
                            pieceOffset = 0;
                            ignore = false;
                            download = false;
                        }

                        fileOffset = (pieceIndex - pieceStart) * this.pieceLength;
                        fileOffset -= pieceIndex > pieceStart ? torrentStartOffset % this.pieceLength : 0;

                        length = (int)Math.Min(this.pieceLength - pieceOffset, file.Key.Length - fileOffset);

                        if (file.Key.Download)
                        {
                            this.Read(file.Value, fileOffset, length, pieceData, pieceOffset);

                            download = true;
                        }
                        else
                        {
                            ignore = true;
                        }

                        ignore = ignore && !file.Key.Download;
                        download = download || file.Key.Download;

                        pieceOffset += length;
                    }

                    torrentStartOffset = torrentEndOffset;
                }

                // last piece
                bitField[previousPieceIndex] = this.GetStatus(ignore, download, this.pieceHashes.ElementAt((int)previousPieceIndex), pieceData.CalculateSha1Hash(0, pieceOffset).ToHexaDecimalString());
            }

            return bitField;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Checks if object is disposed.
        /// </summary>
        private void CheckIfObjectIsDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException("TorrentClient");
            }
        }

        /// <summary>
        /// Creates the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileLength">Length of the file in bytes.</param>
        /// <returns>True if file was created; false otherwise.</returns>
        private bool CreateFile(string filePath, long fileLength)
        {
            filePath.CannotBeNullOrEmpty();
            filePath.MustBeValidFilePath();
            fileLength.MustBeGreaterThan(0);

            this.CheckIfObjectIsDisposed();

            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Debug.WriteLine($"creating directory {Path.GetDirectoryName(filePath)}");

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            if (!File.Exists(filePath))
            {
                Debug.WriteLine($"creating file {filePath}");

                using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.SetLength(fileLength);
                    stream.Close();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <param name="ignore">if set to <c>true</c> one of the piece parts is to be ignored.</param>
        /// <param name="download">if set to <c>true</c> one of the piece parts is to be downloaded.</param>
        /// <param name="pieceHash">The piece hash.</param>
        /// <param name="calculatedPieceHash">The calculated piece hash.</param>
        /// <returns>The piece status.</returns>
        private PieceStatus GetStatus(bool ignore, bool download, string pieceHash, string calculatedPieceHash)
        {
            pieceHash.CannotBeNullOrEmpty();
            calculatedPieceHash.CannotBeNullOrEmpty();
            pieceHash.Length.MustBeEqualTo(calculatedPieceHash.Length);

            if (download &&
                !ignore)
            {
                for (int i = 0; i < pieceHash.Length; i++)
                {
                    if (pieceHash[i] != calculatedPieceHash[i])
                    {
                        return PieceStatus.Missing;
                    }
                }

                return PieceStatus.Present;
            }
            else if (download &&
                     ignore)
            {
                return PieceStatus.Partial;
            }
            else if (!download &&
                     ignore)
            {
                return PieceStatus.Ignore;
            }
            else
            {
                throw new TorrentPersistanceException("Invalid piece status.");
            }
        }

        /// <summary>
        /// Reads the specified data at the offset to the file path.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bufferOffset">The buffer offset.</param>
        /// <exception cref="System.Exception">Incorrect file length.</exception>
        private void Read(FileStream stream, long offset, int length, byte[] buffer, int bufferOffset)
        {
            stream.CannotBeNull();
            offset.MustBeGreaterThanOrEqualTo(0);
            length.MustBeGreaterThan(0);
            buffer.CannotBeNullOrEmpty();
            bufferOffset.MustBeGreaterThanOrEqualTo(0);
            bufferOffset.MustBeLessThanOrEqualTo(buffer.Length - length);

            if (stream.Length >= offset + length)
            {
                stream.Position = offset;
                stream.Read(buffer, bufferOffset, length);
            }
            else
            {
                throw new TorrentPersistanceException("Incorrect file length.");
            }
        }

        /// <summary>
        /// Writes the specified data at the offset to the file path.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bufferOffset">The buffer offset.</param>
        /// <exception cref="System.Exception">Incorrect file length.</exception>
        private void Write(FileStream stream, long offset, int length, byte[] buffer, int bufferOffset = 0)
        {
            stream.CannotBeNull();
            offset.MustBeGreaterThanOrEqualTo(0);
            buffer.CannotBeNullOrEmpty();
            length.MustBeGreaterThan(0);
            length.MustBeLessThanOrEqualTo(buffer.Length);
            bufferOffset.MustBeGreaterThanOrEqualTo(0);
            bufferOffset.MustBeLessThanOrEqualTo((int)(buffer.Length - length));

            if (stream.Length >= offset + length)
            {
                stream.Position = offset;
                stream.Write(buffer, bufferOffset, length);
            }
            else
            {
                throw new TorrentPersistanceException("Incorrect file length.");
            }
        }

        #endregion Private Methods
    }
}
