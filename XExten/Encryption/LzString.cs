﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Encryption
{
    /// <summary>
    /// implements LzstringJS
    /// </summary>
    sealed class LzString
    {
        private const string KeyStrBase64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        private const string KeyStrUriSafe = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-$";
        private static readonly IDictionary<char, char> KeyStrBase64Dict = CreateBaseDict(KeyStrBase64);
        private static readonly IDictionary<char, char> KeyStrUriSafeDict = CreateBaseDict(KeyStrUriSafe);
        private static IDictionary<char, char> CreateBaseDict(string alphabet)
        {
            var dict = new Dictionary<char, char>();
            for (var i = 0; i < alphabet.Length; i++)
            {
                dict[alphabet[i]] = (char)i;
            }
            return dict;
        }
        /// <summary>
        /// 采用Base64编码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CompressToBase64(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            var res = Compress(input, 6, code => KeyStrBase64[code]);
            switch (res.Length % 4)
            {
                default: throw new InvalidOperationException("When could this happen ?");
                case 0: return res;
                case 1: return res + "===";
                case 2: return res + "==";
                case 3: return res + "=";
            }
        }
        /// <summary>
        /// 解析Base64编码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DecompressFromBase64(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            return Decompress(input.Length, 32, index => KeyStrBase64Dict[input[index]]);
        }
        /// <summary>
        /// 采用UTF16编码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CompressToUTF16(string input)
        {
            return Compress(input, 15, code => (char)(code + 32));
        }
        /// <summary>
        /// 解析UTF16编码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DecompressFromUTF16(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            return Decompress(input.Length, 16384, index => (char)(input[index] - 32));
        }
        /// <summary>
        /// 采用URI编码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CompressToEncodedURIComponent(string input)
        {
            if (input == null) return "";

            return Compress(input, 6, code => KeyStrUriSafe[code]);
        }
        /// <summary>
        /// 解析URI编码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DecompressFromEncodedURIComponent(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            input = input.Replace(" ", "+");
            return Decompress(input.Length, 32, index => KeyStrUriSafeDict[input[index]]);
        }
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="uncompressed"></param>
        /// <returns></returns>
        public static string Compress(string uncompressed)
        {
            return Compress(uncompressed, 16, code => (char)code);
        }
        private static string Compress(string uncompressed, int bitsPerChar, Func<int, char> getCharFromInt)
        {
            if (uncompressed == null) throw new ArgumentNullException(nameof(uncompressed));

            int i, value;
            var context_dictionary = new Dictionary<string, int>();
            var context_dictionaryToCreate = new Dictionary<string, bool>();
            var context_wc = "";
            var context_w = "";
            var context_enlargeIn = 2; // Compensate for the first entry which should not count
            var context_dictSize = 3;
            var context_numBits = 2;
            var context_data = new StringBuilder();
            var context_data_val = 0;
            var context_data_position = 0;

            foreach (var context_c in uncompressed)
            {
                if (!context_dictionary.ContainsKey(context_c.ToString()))
                {
                    context_dictionary[context_c.ToString()] = context_dictSize++;
                    context_dictionaryToCreate[context_c.ToString()] = true;
                }

                context_wc = context_w + context_c;
                if (context_dictionary.ContainsKey(context_wc))
                {
                    context_w = context_wc;
                }
                else
                {
                    if (context_dictionaryToCreate.ContainsKey(context_w))
                    {
                        if (context_w.FirstOrDefault() < 256)
                        {
                            for (i = 0; i < context_numBits; i++)
                            {
                                context_data_val = (context_data_val << 1);
                                if (context_data_position == bitsPerChar - 1)
                                {
                                    context_data_position = 0;
                                    context_data.Append(getCharFromInt(context_data_val));
                                    context_data_val = 0;
                                }
                                else
                                {
                                    context_data_position++;
                                }
                            }
                            value = context_w.FirstOrDefault();
                            for (i = 0; i < 8; i++)
                            {
                                context_data_val = (context_data_val << 1) | (value & 1);
                                if (context_data_position == bitsPerChar - 1)
                                {
                                    context_data_position = 0;
                                    context_data.Append(getCharFromInt(context_data_val));
                                    context_data_val = 0;
                                }
                                else
                                {
                                    context_data_position++;
                                }
                                value = value >> 1;
                            }
                        }
                        else
                        {
                            value = 1;
                            for (i = 0; i < context_numBits; i++)
                            {
                                context_data_val = (context_data_val << 1) | value;
                                if (context_data_position == bitsPerChar - 1)
                                {
                                    context_data_position = 0;
                                    context_data.Append(getCharFromInt(context_data_val));
                                    context_data_val = 0;
                                }
                                else
                                {
                                    context_data_position++;
                                }
                                value = 0;
                            }
                            value = context_w.FirstOrDefault();
                            for (i = 0; i < 16; i++)
                            {
                                context_data_val = (context_data_val << 1) | (value & 1);
                                if (context_data_position == bitsPerChar - 1)
                                {
                                    context_data_position = 0;
                                    context_data.Append(getCharFromInt(context_data_val));
                                    context_data_val = 0;
                                }
                                else
                                {
                                    context_data_position++;
                                }
                                value = value >> 1;
                            }
                        }
                        context_enlargeIn--;
                        if (context_enlargeIn == 0)
                        {
                            context_enlargeIn = (int)Math.Pow(2, context_numBits);
                            context_numBits++;
                        }
                        context_dictionaryToCreate.Remove(context_w);
                    }
                    else
                    {
                        value = context_dictionary[context_w];
                        for (i = 0; i < context_numBits; i++)
                        {
                            context_data_val = (context_data_val << 1) | (value & 1);
                            if (context_data_position == bitsPerChar - 1)
                            {
                                context_data_position = 0;
                                context_data.Append(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            }
                            else
                            {
                                context_data_position++;
                            }
                            value = value >> 1;
                        }


                    }
                    context_enlargeIn--;
                    if (context_enlargeIn == 0)
                    {
                        context_enlargeIn = (int)Math.Pow(2, context_numBits);
                        context_numBits++;
                    }
                    // Add wc to the dictionary.
                    context_dictionary[context_wc] = context_dictSize++;
                    context_w = context_c.ToString();
                }
            }

            // Output the code for w.
            if (context_w != "")
            {
                if (context_dictionaryToCreate.ContainsKey(context_w))
                {
                    if (context_w.FirstOrDefault() < 256)
                    {
                        for (i = 0; i < context_numBits; i++)
                        {
                            context_data_val = (context_data_val << 1);
                            if (context_data_position == bitsPerChar - 1)
                            {
                                context_data_position = 0;
                                context_data.Append(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            }
                            else
                            {
                                context_data_position++;
                            }
                        }
                        value = context_w.FirstOrDefault();
                        for (i = 0; i < 8; i++)
                        {
                            context_data_val = (context_data_val << 1) | (value & 1);
                            if (context_data_position == bitsPerChar - 1)
                            {
                                context_data_position = 0;
                                context_data.Append(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            }
                            else
                            {
                                context_data_position++;
                            }
                            value = value >> 1;
                        }
                    }
                    else
                    {
                        value = 1;
                        for (i = 0; i < context_numBits; i++)
                        {
                            context_data_val = (context_data_val << 1) | value;
                            if (context_data_position == bitsPerChar - 1)
                            {
                                context_data_position = 0;
                                context_data.Append(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            }
                            else
                            {
                                context_data_position++;
                            }
                            value = 0;
                        }
                        value = context_w.FirstOrDefault();
                        for (i = 0; i < 16; i++)
                        {
                            context_data_val = (context_data_val << 1) | (value & 1);
                            if (context_data_position == bitsPerChar - 1)
                            {
                                context_data_position = 0;
                                context_data.Append(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            }
                            else
                            {
                                context_data_position++;
                            }
                            value = value >> 1;
                        }
                    }
                    context_enlargeIn--;
                    if (context_enlargeIn == 0)
                    {
                        context_enlargeIn = (int)Math.Pow(2, context_numBits);
                        context_numBits++;
                    }
                    context_dictionaryToCreate.Remove(context_w);
                }
                else
                {
                    value = context_dictionary[context_w];
                    for (i = 0; i < context_numBits; i++)
                    {
                        context_data_val = (context_data_val << 1) | (value & 1);
                        if (context_data_position == bitsPerChar - 1)
                        {
                            context_data_position = 0;
                            context_data.Append(getCharFromInt(context_data_val));
                            context_data_val = 0;
                        }
                        else
                        {
                            context_data_position++;
                        }
                        value = value >> 1;
                    }


                }
                context_enlargeIn--;
                if (context_enlargeIn == 0)
                {
                    context_enlargeIn = (int)Math.Pow(2, context_numBits);
                    context_numBits++;
                }
            }

            // Mark the end of the stream
            value = 2;
            for (i = 0; i < context_numBits; i++)
            {
                context_data_val = (context_data_val << 1) | (value & 1);
                if (context_data_position == bitsPerChar - 1)
                {
                    context_data_position = 0;
                    context_data.Append(getCharFromInt(context_data_val));
                    context_data_val = 0;
                }
                else
                {
                    context_data_position++;
                }
                value = value >> 1;
            }

            // Flush the last char
            while (true)
            {
                context_data_val = (context_data_val << 1);
                if (context_data_position == bitsPerChar - 1)
                {
                    context_data.Append(getCharFromInt(context_data_val));
                    break;
                }
                else context_data_position++;
            }
            return context_data.ToString();
        }
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public static string Decompress(string compressed)
        {
            if (compressed == null) throw new ArgumentNullException(nameof(compressed));

            //TODO: Use an enumerator
            return Decompress(compressed.Length, 32768, index => compressed[index]);
        }
        private static string Decompress(int length, int resetValue, Func<int, char> getNextValue)
        {
            var dictionary = new List<string>();
            var enlargeIn = 4;
            var numBits = 3;
            string entry;
            var result = new StringBuilder();
            int i;
            string w;
            int bits = 0, resb, maxpower, power;
            var c = '\0';

            var data_val = getNextValue(0);
            var data_position = resetValue;
            var data_index = 1;

            for (i = 0; i < 3; i += 1)
            {
                dictionary.Add(((char)i).ToString());
            }

            maxpower = (int)Math.Pow(2, 2);
            power = 1;
            while (power != maxpower)
            {
                resb = data_val & data_position;
                data_position >>= 1;
                if (data_position == 0)
                {
                    data_position = resetValue;
                    data_val = getNextValue(data_index++);
                }
                bits |= (resb > 0 ? 1 : 0) * power;
                power <<= 1;
            }

            switch (bits)
            {
                case 0:
                    bits = 0;
                    maxpower = (int)Math.Pow(2, 8);
                    power = 1;
                    while (power != maxpower)
                    {
                        resb = data_val & data_position;
                        data_position >>= 1;
                        if (data_position == 0)
                        {
                            data_position = resetValue;
                            data_val = getNextValue(data_index++);
                        }
                        bits |= (resb > 0 ? 1 : 0) * power;
                        power <<= 1;
                    }
                    c = (char)bits;
                    break;
                case 1:
                    bits = 0;
                    maxpower = (int)Math.Pow(2, 16);
                    power = 1;
                    while (power != maxpower)
                    {
                        resb = data_val & data_position;
                        data_position >>= 1;
                        if (data_position == 0)
                        {
                            data_position = resetValue;
                            data_val = getNextValue(data_index++);
                        }
                        bits |= (resb > 0 ? 1 : 0) * power;
                        power <<= 1;
                    }
                    c = (char)bits;
                    break;
                case 2:
                    return "";
            }
            w = c.ToString();
            dictionary.Add(w);
            result.Append(c);
            while (true)
            {
                if (data_index > length)
                {
                    return "";
                }

                bits = 0;
                maxpower = (int)Math.Pow(2, numBits);
                power = 1;
                while (power != maxpower)
                {
                    resb = data_val & data_position;
                    data_position >>= 1;
                    if (data_position == 0)
                    {
                        data_position = resetValue;
                        data_val = getNextValue(data_index++);
                    }
                    bits |= (resb > 0 ? 1 : 0) * power;
                    power <<= 1;
                }

                int c2;
                switch (c2 = bits)
                {
                    case (char)0:
                        bits = 0;
                        maxpower = (int)Math.Pow(2, 8);
                        power = 1;
                        while (power != maxpower)
                        {
                            resb = data_val & data_position;
                            data_position >>= 1;
                            if (data_position == 0)
                            {
                                data_position = resetValue;
                                data_val = getNextValue(data_index++);
                            }
                            bits |= (resb > 0 ? 1 : 0) * power;
                            power <<= 1;
                        }

                        c2 = dictionary.Count;
                        dictionary.Add(((char)bits).ToString());
                        enlargeIn--;
                        break;
                    case (char)1:
                        bits = 0;
                        maxpower = (int)Math.Pow(2, 16);
                        power = 1;
                        while (power != maxpower)
                        {
                            resb = data_val & data_position;
                            data_position >>= 1;
                            if (data_position == 0)
                            {
                                data_position = resetValue;
                                data_val = getNextValue(data_index++);
                            }
                            bits |= (resb > 0 ? 1 : 0) * power;
                            power <<= 1;
                        }
                        c2 = dictionary.Count;
                        dictionary.Add(((char)bits).ToString());
                        enlargeIn--;
                        break;
                    case (char)2:
                        return result.ToString();
                }

                if (enlargeIn == 0)
                {
                    enlargeIn = (int)Math.Pow(2, numBits);
                    numBits++;
                }

                if (dictionary.Count - 1 >= c2)
                {
                    entry = dictionary[c2];
                }
                else
                {
                    if (c2 == dictionary.Count)
                    {
                        entry = w + w[0];
                    }
                    else
                    {
                        return null;
                    }
                }
                result.Append(entry);

                // Add w+entry[0] to the dictionary.
                dictionary.Add(w + entry[0]);
                enlargeIn--;

                w = entry;

                if (enlargeIn == 0)
                {
                    enlargeIn = (int)Math.Pow(2, numBits);
                    numBits++;
                }
            }
        }
    }
}