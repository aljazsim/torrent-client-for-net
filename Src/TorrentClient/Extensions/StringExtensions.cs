using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DefensiveProgrammingFramework;

namespace TorrentClient.Extensions
{
    /// <summary>
    /// The string extensions.
    /// </summary>
    public static class StringExtensions
    {
        #region Public Methods

        /// <summary>
        /// Formats the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The formatted text.</returns>
        public static string Format2(this string text, params object[] arguments)
        {
            return string.Format(CultureInfo.InvariantCulture, text, arguments);
        }

        /// <summary>
        /// Generates a random string from the characters of the string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="size">The string size.</param>
        /// <returns>Random string.</returns>
        public static string Random(this string str, int size)
        {
            return RandomHelper.RandomString(size, str);
        }

        /// <summary>
        /// Converts the hexadecimal string to byte array.
        /// </summary>
        /// <param name="value">The hexadecimal string.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ToByteArray(this string value)
        {
            value.CannotBeNull();

            return Enumerable.Range(0, value.Length / 2).Select(x => Convert.ToByte(value.Substring(x * 2, 2), 16)).ToArray();
        }

        /// <summary>
        /// Converts byte count to respective measurement.
        /// </summary>
        /// <param name="byteCount">The length.</param>
        /// <param name="accuracy">The accuracy.</param>
        /// <returns>
        /// The string representation of byte count.
        /// </returns>
        public static string ToBytes(this long byteCount, decimal accuracy = 1)
        {
            long kb = 1024;
            long mb = kb * 1024;
            long gb = mb * 1024;
            long tb = gb * 1024;
            long pb = tb * 1024;

            if (byteCount < kb)
            {
                return "{0:##0}B".Format2(byteCount);
            }
            else if (byteCount < mb)
            {
                return "{0:0}kB".Format2(Math.Round(byteCount / kb / accuracy) * accuracy);
            }
            else if (byteCount < gb)
            {
                return "{0:0}MB".Format2(Math.Round(byteCount / mb / accuracy) * accuracy);
            }
            else if (byteCount < tb)
            {
                return "{0:0}GB".Format2(Math.Round(byteCount / gb / accuracy) * accuracy);
            }
            else if (byteCount < pb)
            {
                return "{0:0}TB".Format2(Math.Round(byteCount / tb / accuracy) * accuracy);
            }
            else
            {
                return "{0:0}PB".Format2(Math.Round(byteCount / pb / accuracy) * accuracy);
            }
        }

        /// <summary>
        /// Converts byte count to respective measurement.
        /// </summary>
        /// <param name="byteCount">The length.</param>
        /// <param name="accuracy">The accuracy.</param>
        /// <returns>
        /// The string representation of byte count.
        /// </returns>
        public static string ToBytes(this decimal byteCount, decimal accuracy = 1)
        {
            long kb = 1024;
            long mb = kb * 1024;
            long gb = mb * 1024;
            long tb = gb * 1024;
            long pb = tb * 1024;

            if (byteCount < kb)
            {
                return "{0:##0}B".Format2(byteCount);
            }
            else if (byteCount < mb)
            {
                return "{0:0}kB".Format2(Math.Round(byteCount / kb / accuracy) * accuracy);
            }
            else if (byteCount < gb)
            {
                return "{0:0}MB".Format2(Math.Round(byteCount / mb / accuracy) * accuracy);
            }
            else if (byteCount < tb)
            {
                return "{0:0}GB".Format2(Math.Round(byteCount / gb / accuracy) * accuracy);
            }
            else if (byteCount < pb)
            {
                return "{0:0}TB".Format2(Math.Round(byteCount / tb / accuracy) * accuracy);
            }
            else
            {
                return "{0:0}PB".Format2(Math.Round(byteCount / pb / accuracy) * accuracy);
            }
        }

        /// <summary>
        /// To the capital word case.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="separators">The separators.</param>
        /// <returns>
        /// String in capital word case.
        /// </returns>
        public static string ToCapitalWordCase(this string str, params char[] separators)
        {
            str.CannotBeNull();
            separators.CannotBeNull();

            List<string> words = new List<string>();

            if (separators.Length == 0)
            {
                separators = new char[] { ' ' };
            }

            foreach (var word in str.Split(separators))
            {
                if (word.Length == 0)
                {
                    words.Add(string.Empty);
                }
                else if (word.Length == 1)
                {
                    words.Add(word.ToUpper(CultureInfo.InvariantCulture));
                }
                else
                {
                    words.Add(char.ToUpper(word[0], CultureInfo.InvariantCulture) + word.Substring(1));
                }
            }

            return string.Join(" ", words);
        }

        /// <summary>
        /// Converts the byte array the hexadecimal string.
        /// </summary>
        /// <param name="value">The byte array.</param>
        /// <returns>
        /// The hexadecimal string.
        /// </returns>
        public static string ToHexaDecimalString(this byte[] value)
        {
            value.CannotBeNull();

            return BitConverter.ToString(value).Replace("-", string.Empty, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Converts string to sentence case.
        /// </summary>
        /// <param name="str">The string to transform.</param>
        /// <returns>String in sentence case.</returns>
        public static string ToSentenceCase(this string str)
        {
            return str.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture) + (str.Length > 1 ? str.Substring(1) : string.Empty);
        }

        /// <summary>
        /// To the snake case.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="separators">The separators.</param>
        /// <returns>
        /// String in snake case.
        /// </returns>
        public static string ToSnakeCase(this string str, params char[] separators)
        {
            str.CannotBeNull();
            separators.CannotBeNull();

            if (separators.Length == 0)
            {
                separators = new char[] { ' ' };
            }

            return string.Join("_", str.ToUpper(CultureInfo.InvariantCulture).Split(separators, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <param name="str">The string to transform.</param>
        /// <param name="stringCasing">The string casing.</param>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public static string ToString(this string str, StringCasing stringCasing)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                switch (stringCasing)
                {
                    case StringCasing.Upper:
                        return str.ToUpper(CultureInfo.CurrentCulture);

                    case StringCasing.Lower:
                        return str.ToLower(CultureInfo.CurrentCulture);

                    case StringCasing.Sentence:
                        return ToSentenceCase(str);

                    case StringCasing.Title:
                        return ToTitleCase(str, CultureInfo.CurrentCulture);

                    case StringCasing.CapitalWord:
                        return ToCapitalWordCase(str);

                    case StringCasing.SnakeCase:
                        return ToSnakeCase(str);

                    case StringCasing.CamelCase:
                        return ToSnakeCase(str);

                    case StringCasing.None:
                        return str;

                    default:
                        throw new FormatException("Invalid string casing format.");
                }
            }
        }

        /// <summary>
        /// Converts string to title case.
        /// </summary>
        /// <param name="str">The string to transform.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>String in title case.</returns>
        public static string ToTitleCase(string str, CultureInfo culture)
        {
            str.CannotBeNull();

            if (culture != null)
            {
                return culture.TextInfo.ToTitleCase(str.ToLower(CultureInfo.CurrentCulture));
            }
            else
            {
                throw new ArgumentNullException(nameof(culture), "Culture info cannot be null.");
            }
        }

        #endregion Public Methods
    }
}
