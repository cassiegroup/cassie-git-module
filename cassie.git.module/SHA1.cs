// Copyright 2020 The Cassie Authors. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace cassie.git.module
{
    // SHA1 is the SHA-1 hash of a Git object.
    public class SHA1
    {
        // EmptyID is an ID with empty SHA-1 hash.
        public const string EmptyID = "0000000000000000000000000000000000000000";
        public byte[] Bytes;
        private string str;

        public bool Equal(string str) {
            return this.str == str;
        }

        public bool Equal(byte[] bytes)
        {
            return this.Bytes == bytes;
        }

        public bool Equal(SHA1 sha1)
        {
            return this.Bytes == sha1.Bytes;
        }

        public string String()
        {
            var result = new byte[40];
            var hexvalues = convert2ByteArray("0123456789abcdef");

            for(var i=0;i<20;i++)
            {
                result[i] = hexvalues[Bytes[i] >> 4];
                result[i+1] = hexvalues[Bytes[i] & 0xf];
            }
            this.str = convert2String(result);
            return this.str;
        }

        // MustID returns a new SHA1 from a [20]byte array.
        public static SHA1 MustID(byte[] bytes)
        {
            var sha1 = new SHA1();
            sha1.Bytes = new byte[20];
            for (int i = 0; i < 20; i++)
            {
                sha1.Bytes[i] = bytes[i];
            }
            return sha1;
        }
        // NewID returns a new SHA1 from a [20]byte array.
        public static SHA1 NewID(byte[] bytes)
        {
            if(bytes.Length != 20){
                throw new LenghNotMatchException("byte array length must equal 20");
            }
            return MustID(bytes);
        }
        // MustIDFromString always returns a new sha from a ID with no validation of input.
        public static SHA1 MustIDFromString(string s)
        {
            var b = stringToByteArray(s);
            return MustID(b);
        }
        // NewIDFromString returns a new SHA1 from a ID string of length 40.
        public static SHA1 NewIDFromString(string s){
            s = s.Trim();
            if(s.Length != 40){
                throw new LenghNotMatchException("byte array length must equal 40");
            }
            var b = stringToByteArray(s);
            return NewID(b);
        }

        public static byte[] stringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        private byte[] convert2ByteArray(string str)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, str);
                return ms.ToArray();
            }
        }
        private string convert2String(byte[] bytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(bytes, 0, bytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj.ToString();
            }
        }

    }
}
