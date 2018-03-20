using System;

namespace TorrentClient.Extensions
{
    /// <summary>
    /// The exception extensions.
    /// </summary>
    public static class ExceptionExtensions
    {
        #region Public Methods

        /// <summary>
        /// Formats the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>Fhe formatted exception.</returns>
        public static string Format(this Exception exception)
        {
            string message = string.Empty;

            while (exception != null)
            {
                message += exception.GetType().ToString() + ": " + exception.Message.Trim();
                message += Environment.NewLine;
                message += exception.StackTrace;
                message += Environment.NewLine;

                exception = exception.InnerException;
            }

            return message;
        }

        #endregion Public Methods
    }
}
