using System;
using System.Diagnostics;
using System.Threading;
using DefensiveProgrammingFramework;
using TorrentClient.Extensions;

namespace TorrentClient
{
    /// <summary>
    /// The throttling manager.
    /// </summary>
    public sealed class ThrottlingManager
    {
        #region Private Fields

        /// <summary>
        /// The minimum read time in milliseconds.
        /// </summary>
        private decimal minReadTime;

        /// <summary>
        /// The minimum write time in milliseconds.
        /// </summary>
        private decimal minWriteTime;

        /// <summary>
        /// The read bytes count.
        /// </summary>
        private long read = 0;

        /// <summary>
        /// The count of bytes in read delta.
        /// </summary>
        private decimal readDelta = 0;

        /// <summary>
        /// The reading thread locker.
        /// </summary>
        private object readingLocker = new object();

        /// <summary>
        /// The read limit in bytes per second.
        /// </summary>
        private long readLimit;

        /// <summary>
        /// The read stopwatch.
        /// </summary>
        private Stopwatch readStopwatch = new Stopwatch();

        /// <summary>
        /// The count of bytes in write delta.
        /// </summary>
        private decimal writeDelta = 0;

        /// <summary>
        /// The write limit in bytes per second.
        /// </summary>
        private long writeLimit;

        /// <summary>
        /// The write stopwatch.
        /// </summary>
        private Stopwatch writeStopwatch = new Stopwatch();

        /// <summary>
        /// The writing thread locker.
        /// </summary>
        private object writingLocker = new object();

        /// <summary>
        /// The written bytes count.
        /// </summary>
        private long written = 0;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottlingManager"/> class.
        /// </summary>
        public ThrottlingManager()
        {
            this.ReadSpeedLimit = int.MaxValue;
            this.WriteSpeedLimit = int.MaxValue;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the read speed in bytes per second.
        /// </summary>
        /// <value>
        /// The read speed in bytes per second.
        /// </value>
        public decimal ReadSpeed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the read speed limit in bytes per second.
        /// </summary>
        /// <value>
        /// The read speed limit in bytes per seconds.
        /// </value>
        public long ReadSpeedLimit
        {
            get
            {
                return this.readLimit;
            }

            set
            {
                value.MustBeGreaterThan(0);

                this.readLimit = value;
                this.readDelta = value;

                this.minReadTime = this.CalculateMinExecutionTime(value);
            }
        }

        /// <summary>
        /// Gets the write speed in bytes per second.
        /// </summary>
        /// <value>
        /// The write speed in bytes per second.
        /// </value>
        public decimal WriteSpeed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the write speed limit in bytes per second.
        /// </summary>
        /// <value>
        /// The write speed limit in bytes per seconds.
        /// </value>
        public long WriteSpeedLimit
        {
            get
            {
                return this.writeLimit;
            }

            set
            {
                value.MustBeGreaterThan(0);

                this.writeLimit = value;
                this.writeDelta = value;

                this.minWriteTime = this.CalculateMinExecutionTime(value);
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="bytesRead">The bytes read count.</param>
        public void Read(long bytesRead)
        {
            bytesRead.MustBeGreaterThanOrEqualTo(0);

            decimal wait;

            lock (this.readingLocker)
            {
                if (!this.readStopwatch.IsRunning)
                {
                    this.readStopwatch.Start();
                }

                this.read += bytesRead;

                if (this.read > this.readDelta)
                {
                    this.readStopwatch.Stop();

                    this.ReadSpeed = this.read / (decimal)this.readStopwatch.Elapsed.TotalSeconds;

                    wait = (this.read / this.readDelta) * this.minReadTime;
                    wait = wait - this.readStopwatch.ElapsedMilliseconds;

                    if (wait > 0)
                    {
                        Thread.Sleep((int)Math.Round(wait));
                    }

                    this.read = 0;
                    this.readStopwatch.Restart();
                }
            }
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="bytesWritten">The bytes written.</param>
        public void Write(long bytesWritten)
        {
            bytesWritten.MustBeGreaterThanOrEqualTo(0);

            decimal wait;

            lock (this.writingLocker)
            {
                if (!this.writeStopwatch.IsRunning)
                {
                    this.writeStopwatch.Start();
                }

                this.written += bytesWritten;

                if (this.written > this.writeDelta)
                {
                    this.writeStopwatch.Stop();

                    this.WriteSpeed = this.written / (decimal)this.writeStopwatch.Elapsed.TotalSeconds;

                    wait = (this.written / this.writeDelta) * this.minWriteTime;
                    wait = wait - this.writeStopwatch.ElapsedMilliseconds;

                    if (wait > 0)
                    {
                        Thread.Sleep((int)Math.Round(wait));
                    }

                    this.written = 0;
                    this.writeStopwatch.Restart();
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Calculates the minimum execution time.
        /// </summary>
        /// <param name="speed">The speed in bytes per second.</param>
        /// <returns>The minimal time to process the bytes in milliseconds.</returns>
        private decimal CalculateMinExecutionTime(decimal speed)
        {
            return 1000m * this.readDelta / speed;
        }

        #endregion Private Methods
    }
}
