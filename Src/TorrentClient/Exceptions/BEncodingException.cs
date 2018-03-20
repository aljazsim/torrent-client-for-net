using System;
using System.Runtime.Serialization;

namespace TorrentClient.Exceptions
{
    /// <summary>
    /// The BEncoding exception.
    /// </summary>
    [Serializable]
    public class BEncodingException : Exception
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodingException"/> class.
        /// </summary>
        public BEncodingException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodingException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BEncodingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodingException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public BEncodingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodingException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected BEncodingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Protected Constructors
    }
}
