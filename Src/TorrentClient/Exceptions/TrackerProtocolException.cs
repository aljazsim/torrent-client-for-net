using System;
using System.Runtime.Serialization;

namespace TorrentClient.Exceptions
{
    /// <summary>
    /// The tracker protocol exception exception.
    /// </summary>
    [Serializable]
    public class TrackerProtocolException : Exception
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackerProtocolException"/> class.
        /// </summary>
        public TrackerProtocolException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackerProtocolException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TrackerProtocolException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackerProtocolException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public TrackerProtocolException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackerProtocolException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected TrackerProtocolException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Protected Constructors
    }
}
