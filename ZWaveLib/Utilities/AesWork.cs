// http://www.codeproject.com/Tips/704372/How-to-use-Rijndael-ManagedEncryption-with-Csharp

using System;
using System.IO;
using System.Security.Cryptography;

namespace ZWaveLib.Utilities
{
    public class AesWork
    {
        private static readonly byte[] zeroIV = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public static byte[] GenerateKey1(byte[] nc, byte[] plainText)
        {
            var tmp = new byte[zeroIV.Length];
            Array.Copy(EncryptMessage(nc, zeroIV, plainText, CipherMode.CBC), tmp, 16);
            return tmp;
        }

        public static byte[] EncryptOfbMessage(byte[] nc, byte[] iv, byte[] plaintext)
        {
            var processed = new byte[plaintext.Length];
            var len = (plaintext.Length % 16) * 16;
            var l_plaintext = new byte[len];
            var encrypted = EncryptMessage(nc, iv, l_plaintext, CipherMode.CBC);
            for (var i = 0; i < plaintext.Length; i++)
            {
                processed[i] = (byte)(plaintext[i] ^ encrypted[i]);
            }
            return processed;
        }

        public static byte[] EncryptEcbMessage(byte[] nc, byte[] plaintext)
        {
            var tmp = new byte[zeroIV.Length];
            Array.Copy(EncryptMessage(nc, zeroIV, plaintext, CipherMode.ECB), tmp, 16);
            return tmp;
        }

        private static byte[] EncryptMessage(byte[] nc, byte[] iv, byte[] plaintext, CipherMode cm)
        {
            if (nc == null)
            {
                Utility.DebugLog(DebugMessageType.Error, "The used key has not been generated.");
                return zeroIV;
            }

            var algorithm = new AesManaged
            {
                Key = nc,
                IV = iv,
                Mode = cm
            };
            return EncryptBytes(algorithm, plaintext);
        }

        private static byte[] EncryptBytes(SymmetricAlgorithm algorithm, byte[] message)
        {
            if ((message == null) || (message.Length == 0))
            {
                return message;
            }
            if (algorithm == null)
            {
                throw new ArgumentNullException(nameof(algorithm));
            }

            using (var stream = new MemoryStream())
            using (var encryptor = algorithm.CreateEncryptor())
            using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
            {
                encrypt.Write(message, 0, message.Length);
                encrypt.FlushFinalBlock();
                return stream.ToArray();
            }
        }
    }
}
