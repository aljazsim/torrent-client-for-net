using System;
using System.Runtime.Serialization;

namespace TorrentClient.Exceptions
{
    [Serializable]
    public class TorrentPersistanceException : Exception
    {
        #region Public Constructors

        public TorrentPersistanceException()
        {
        }

        public TorrentPersistanceException(string message)
            : base(message)
        {
        }

        public TorrentPersistanceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected TorrentPersistanceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Protected Constructors
    }
}
