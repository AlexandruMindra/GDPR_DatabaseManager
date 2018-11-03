using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Drawing;

namespace DbServer
{
    class GoogleTOTP
    {
        RNGCryptoServiceProvider rnd;
        protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        private int intervalLength;
        private int pinCodeLength;
        private int pinModulo;

        private byte[] randomBytes = new byte[10];

        public GoogleTOTP()
        {
            rnd = new RNGCryptoServiceProvider();

            pinCodeLength = 6;
            intervalLength = 30;
            pinModulo = (int)Math.Pow(10, pinCodeLength);

            rnd.GetBytes(randomBytes);
        }

        public byte[] getPrivateKey()
        {
            return randomBytes;
        }

        private String generateResponseCode(long challenge, byte[] randomBytes)
        {
            HMACSHA1 myHmac = new HMACSHA1(randomBytes);
            myHmac.Initialize();

            byte[] value = BitConverter.GetBytes(challenge);
            Array.Reverse(value); //reverses the challenge array due to differences in c# vs java
            myHmac.ComputeHash(value);
            byte[] hash = myHmac.Hash;
            int offset = hash[hash.Length - 1] & 0xF;
            byte[] SelectedFourBytes = new byte[4];
            //selected bytes are actually reversed due to c# again, thus the weird stuff here
            SelectedFourBytes[0] = hash[offset];
            SelectedFourBytes[1] = hash[offset + 1];
            SelectedFourBytes[2] = hash[offset + 2];
            SelectedFourBytes[3] = hash[offset + 3];
            Array.Reverse(SelectedFourBytes);
            int finalInt = BitConverter.ToInt32(SelectedFourBytes, 0);
            int truncatedHash = finalInt & 0x7FFFFFFF; //remove the most significant bit for interoperability as per HMAC standards
            int pinValue = truncatedHash % pinModulo; //generate 10^d digits where d is the number of digits
            return padOutput(pinValue);
        }

        public long getCurrentInterval()
        {
            TimeSpan TS = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long currentTimeSeconds = (long)Math.Floor(TS.TotalSeconds);
            long currentInterval = currentTimeSeconds / intervalLength; // 30 Seconds
            return currentInterval;
        }

        private String padOutput(int value)
        {
            String result = value.ToString();
            for (int i = result.Length; i < pinCodeLength; i++)
            {
                result = "0" + result;
            }
            return result;
        }

        public string GeneratePin(string code)
        {
            return generateResponseCode(getCurrentInterval(), Encoding.ASCII.GetBytes(code));
        }

        public string randomString()
        {
            return CreativeCommons.Transcoder.Base32Encode(randomBytes);
        }

    }
}
