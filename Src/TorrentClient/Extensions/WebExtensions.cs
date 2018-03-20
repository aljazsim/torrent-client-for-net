using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using DefensiveProgrammingFramework;

namespace TorrentClient.Extensions
{
    /// <summary>
    /// The web related extensions.
    /// </summary>
    public static class WebExtensions
    {
        #region Public Methods

        /// <summary>
        /// Executes the binary request.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// The binary data.
        /// </returns>
        public static byte[] ExecuteBinaryRequest(this Uri uri, TimeSpan? timeout = null)
        {
            Stopwatch stopwatch;
            byte[] responseContent;
            Func<HttpWebResponse, byte[]> getDataFunc;

            getDataFunc = (response) =>
            {
                int count;
                List<byte> content = new List<byte>();
                byte[] buffer = new byte[4096];

                // download request contents into memory
                using (Stream responseStream = response.GetResponseStream())
                {
                    while ((count = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        content.AddRange(buffer.Take(count));
                    }
                }

                return content.ToArray();
            };

            stopwatch = Stopwatch.StartNew();

            responseContent = uri.ExecuteRequest(getDataFunc, null, "text/plain", 0, timeout ?? TimeSpan.FromMinutes(1));

            stopwatch.Stop();

            return responseContent;
        }

        /// <summary>
        /// Executes the UDP request.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="bytes">The payload bytes.</param>
        public static void ExecuteUdpRequest(this IPEndPoint endpoint, byte[] bytes)
        {
            endpoint.CannotBeNull();
            bytes.CannotBeNullOrEmpty();

            int bufferSize = 4096;
            TimeSpan sendTimeout = TimeSpan.FromSeconds(5);
            TimeSpan receiveTimeout = TimeSpan.FromSeconds(5);
            IAsyncResult asyncResult;
            int count;

            // execute request
            using (UdpClient udp = new UdpClient())
            {
                udp.Client.SendTimeout = (int)sendTimeout.TotalMilliseconds;
                udp.Client.SendBufferSize = bytes.Length;
                udp.Client.ReceiveTimeout = (int)receiveTimeout.TotalMilliseconds;
                udp.Client.ReceiveBufferSize = bufferSize;
                udp.Connect(endpoint);

                asyncResult = udp.BeginSend(bytes, bytes.Length, null, null);

                if (asyncResult.AsyncWaitHandle.WaitOne(receiveTimeout))
                {
                    // stop reading
                    count = udp.EndSend(asyncResult);

                    if (count == bytes.Length)
                    {
                        // ok
                    }
                }
                else
                {
                    // timeout
                }

                udp.Close();
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Executes the HTTP request.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="getDataFunc">The get data function.</param>
        /// <param name="data">The data.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="redirectCount">The redirect count.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// The response result.
        /// </returns>
        private static T ExecuteRequest<T>(this Uri uri, Func<HttpWebResponse, T> getDataFunc, byte[] data = null, string requestContentType = "text/plain", int redirectCount = 0, TimeSpan? timeout = null)
        {
            HttpWebRequest request;
            T responseContent = default(T);
            int maxRedirects = 30;
            string location = "Location";

            uri.CannotBeNull();
            getDataFunc.CannotBeNull();
            requestContentType.CannotBeNullOrEmpty();
            redirectCount.MustBeGreaterThanOrEqualTo(0);

            // make request
            request = HttpWebRequest.Create(uri) as HttpWebRequest;
            request.Method = data == null ? "GET" : "POST";
            request.KeepAlive = false;
            request.ContentType = requestContentType;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.CookieContainer = new CookieContainer();
            request.UserAgent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1300.0 Iron/23.0.1300.0 Safari/537.11";
            request.AllowAutoRedirect = false;
            request.Timeout = (int)(timeout == null ? TimeSpan.FromSeconds(10) : (TimeSpan)timeout).TotalMilliseconds;

            // setup request contents
            if (data != null)
            {
                request.ContentLength = data.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(data, 0, data.Length);
                }
            }

            // get response
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode == HttpStatusCode.Redirect)
                {
                    if (redirectCount <= maxRedirects)
                    {
                        if (response.Headers.AllKeys.Contains(location) &&
                            response.Headers[location].IsNotNullOrEmpty())
                        {
                            if (Uri.TryCreate(response.Headers[location], UriKind.Absolute, out uri))
                            {
                                responseContent = uri.ExecuteRequest(getDataFunc, data, requestContentType, ++redirectCount);
                            }
                        }
                    }
                }
                else
                {
                    responseContent = getDataFunc(response);
                }
            }

            return responseContent;
        }

        #endregion Private Methods
    }
}
