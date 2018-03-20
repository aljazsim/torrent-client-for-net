using System;
using System.Runtime.Serialization;

namespace TorrentClient.Exceptions
{
    [Serializable]
    public class TorrentInfoException : Exception
    {
        #region Public Constructors

        public TorrentInfoException()
        {
        }

        public TorrentInfoException(string message)
            : base(message)
        {
        }

        public TorrentInfoException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected TorrentInfoException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Protected Constructors
    }
}
