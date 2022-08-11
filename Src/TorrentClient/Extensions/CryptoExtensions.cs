using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DefensiveProgrammingFramework;

namespace TorrentClient.Extensions
{
    /// <summary>
    /// The cryptography extensions.
    /// </summary>
    public static class CryptoExtensions
    {
        #region Public Methods

        /// <summary>
        /// Calculates the 128 bit SHA hash.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>The 128 bit SHA hash.</returns>
        public static byte[] CalculateSha1Hash(this byte[] data, int offset, int length)
        {
            return CalculateHashSha(data, offset, length, 128);
        }

        /// <summary>
        /// Calculates the 128 bit SHA hash.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>
        /// The 128 bit SHA hash.
        /// </returns>
        public static byte[] CalculateSha1Hash(this byte[] data)
        {
            return CalculateHashSha(data, 0, data.Length, 128);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Calculates the hash of a string.
        /// </summary>
        /// <param name="data">The data to hash.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <returns>
        /// Calculated hash
        /// </returns>
        private static byte[] CalculateHash(this byte[] data, int offset, int count, HashAlgorithm hashAlgorithm)
        {
            byte[] hashRaw = hashAlgorithm.ComputeHash(data, offset, count);
            hashAlgorithm.Clear();

            return hashRaw;
        }

        /// <summary>
        /// Calculates the hash of a data using SHA hash algorithm.
        /// </summary>
        /// <param name="data">The data to hash.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="hashSize">Size of the hash.</param>
        /// <returns>
        /// Calculated hash
        /// </returns>
        private static byte[] CalculateHashSha(this byte[] data, int offset, int length, int hashSize)
        {
            data.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);
            length.MustBeGreaterThanOrEqualTo(0);
            hashSize.MustBeOneOf(128, 256, 384, 512);

            byte[] hashRaw = null;

            if (data != null)
            {
                if (hashSize == 128)
                {
                    using HashAlgorithm sha = SHA1.Create();

                    hashRaw = CalculateHash(data, offset, length, sha);
                }
                else if (hashSize == 256)
                {
                    using HashAlgorithm sha = SHA256.Create();

                    hashRaw = CalculateHash(data, offset, length, sha);
                }
                else if (hashSize == 384)
                {
                    using HashAlgorithm sha = SHA384.Create();

                    hashRaw = CalculateHash(data, offset, length, sha);
                }
                else if (hashSize == 512)
                {
                    using HashAlgorithm sha = SHA512.Create();

                    hashRaw = CalculateHash(data, offset, length, sha);
                }
            }

            return hashRaw;
        }

        #endregion Private Methods
    }
}
