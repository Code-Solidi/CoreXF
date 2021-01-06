using System;
using System.Text;

namespace CoreXF.Messaging
{
    /// <summary>
    /// A mix of useful extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts a string to base64 string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToBase64(this string value)
        {
            var inputBytes = Encoding.UTF8.GetBytes(value);

            // Each 3 byte sequence in inputBytes must be converted to a 4 byte sequence
            var arrLength = (long)(4.0d * inputBytes.Length / 3.0d);
            if (arrLength % 4 != 0)
            {
                // increment the array length to the next multiple of 4 if it is not already divisible by 4
                arrLength += 4 - (arrLength % 4);
            }

            var encodedCharArray = new char[arrLength];
            Convert.ToBase64CharArray(inputBytes, 0, inputBytes.Length, encodedCharArray, 0);
            var result = new string(encodedCharArray);
            return result;
        }

        /// <summary>
        /// Converts a base64 string to string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string FromBase64(this string value)
        {
            var decodedCharArray = Convert.FromBase64String(value);
            var result = Encoding.UTF8.GetString(decodedCharArray);
            return result;
        }
    }
}