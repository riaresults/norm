#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace D10.Norm
{
    public static class BitUtils
    {
        public const int ByteLength = 8;

        public static string BitArrayToString(BitArray bits)
        {
            StringBuilder sb = new StringBuilder(bits.Length);

            for (int i = bits.Length - 1; i >= 0; i--)
            {
                bool bit = bits[i];

                sb.Append(Convert.ToInt32(bit));
            }

            return sb.ToString();
        }

        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            return BitArrayToByteArray(bits, 0, bits.Length);
        }

        public static byte[] BitArrayToByteArray(BitArray bits, int startIndex, int count)
        {
            // Get the size of bytes needed to store all bytes
            int bytesize = count / ByteLength;

            // Any bit left over another byte is necessary
            if (count % ByteLength > 0)
                bytesize++;

            // For the result
            byte[] bytes = new byte[bytesize];

            // Must init to good value, all zero bit byte has value zero
            // Lowest significant bit has a place value of 1, each position to
            // to the left doubles the value
            byte value = 0;
            byte significance = 1;

            // Remember where in the input/output arrays
            int bytepos = 0;
            int bitpos = startIndex;

            while (bitpos - startIndex < count)
            {
                // If the bit is set add its value to the byte
                if (bits[bitpos])
                    value += significance;

                bitpos++;

                if (bitpos % ByteLength == 0)
                {
                    // A full byte has been processed, store it
                    // increase output buffer index and reset work values
                    bytes[bytepos] = value;
                    bytepos++;
                    value = 0;
                    significance = 1;
                }
                else
                {
                    // Another bit processed, next has doubled value
                    significance *= 2;
                }
            }
            return bytes;
        }

        public static T Not<T>(T value, T not)// where T : IEquatable<T>
        {
            if (!GenericOperatorFactory<T, T, T, T>.And(value, not).Equals(default(T)))
                return GenericOperatorFactory<T, T, T, T>.Xor(value, not);

            return value;
        }
    }
}
