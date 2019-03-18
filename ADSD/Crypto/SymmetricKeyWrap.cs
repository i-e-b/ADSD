using System;
using System.Security.Cryptography;

namespace ADSD.Crypto
{
    internal static class SymmetricKeyWrap
    {
        private static readonly byte[] s_rgbTripleDES_KW_IV = new byte[8]
        {
            (byte) 74,
            (byte) 221,
            (byte) 162,
            (byte) 44,
            (byte) 121,
            (byte) 232,
            (byte) 33,
            (byte) 5
        };
        private static readonly byte[] s_rgbAES_KW_IV = new byte[8]
        {
            (byte) 166,
            (byte) 166,
            (byte) 166,
            (byte) 166,
            (byte) 166,
            (byte) 166,
            (byte) 166,
            (byte) 166
        };

        internal static byte[] TripleDESKeyWrapEncrypt(byte[] rgbKey, byte[] rgbWrappedKeyData)
        {
            byte[] hash = new SHA1CryptoServiceProvider().ComputeHash(rgbWrappedKeyData);
            RNGCryptoServiceProvider cryptoServiceProvider1 = new RNGCryptoServiceProvider();
            byte[] numArray1 = new byte[8];
            cryptoServiceProvider1.GetBytes(numArray1);
            byte[] inputBuffer1 = new byte[rgbWrappedKeyData.Length + 8];
            TripleDESCryptoServiceProvider cryptoServiceProvider2 = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider2.Padding = PaddingMode.None;
            ICryptoTransform encryptor = cryptoServiceProvider2.CreateEncryptor(rgbKey, numArray1);
            Buffer.BlockCopy((Array) rgbWrappedKeyData, 0, (Array) inputBuffer1, 0, rgbWrappedKeyData.Length);
            Buffer.BlockCopy((Array) hash, 0, (Array) inputBuffer1, rgbWrappedKeyData.Length, 8);
            byte[] numArray2 = encryptor.TransformFinalBlock(inputBuffer1, 0, inputBuffer1.Length);
            byte[] inputBuffer2 = new byte[numArray1.Length + numArray2.Length];
            Buffer.BlockCopy((Array) numArray1, 0, (Array) inputBuffer2, 0, numArray1.Length);
            Buffer.BlockCopy((Array) numArray2, 0, (Array) inputBuffer2, numArray1.Length, numArray2.Length);
            Array.Reverse((Array) inputBuffer2);
            return cryptoServiceProvider2.CreateEncryptor(rgbKey, SymmetricKeyWrap.s_rgbTripleDES_KW_IV).TransformFinalBlock(inputBuffer2, 0, inputBuffer2.Length);
        }

        internal static byte[] TripleDESKeyWrapDecrypt(byte[] rgbKey, byte[] rgbEncryptedWrappedKeyData)
        {
            if (rgbEncryptedWrappedKeyData.Length != 32 && rgbEncryptedWrappedKeyData.Length != 40 && rgbEncryptedWrappedKeyData.Length != 48)
                throw new CryptographicException("Cryptography problem: Bad key size (1)");
            TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider.Padding = PaddingMode.None;
            byte[] numArray1 = cryptoServiceProvider.CreateDecryptor(rgbKey, SymmetricKeyWrap.s_rgbTripleDES_KW_IV).TransformFinalBlock(rgbEncryptedWrappedKeyData, 0, rgbEncryptedWrappedKeyData.Length);
            Array.Reverse((Array) numArray1);
            byte[] rgbIV = new byte[8];
            Buffer.BlockCopy((Array) numArray1, 0, (Array) rgbIV, 0, 8);
            byte[] inputBuffer = new byte[numArray1.Length - rgbIV.Length];
            Buffer.BlockCopy((Array) numArray1, 8, (Array) inputBuffer, 0, inputBuffer.Length);
            byte[] numArray2 = cryptoServiceProvider.CreateDecryptor(rgbKey, rgbIV).TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            byte[] buffer = new byte[numArray2.Length - 8];
            Buffer.BlockCopy((Array) numArray2, 0, (Array) buffer, 0, buffer.Length);
            byte[] hash = new SHA1CryptoServiceProvider().ComputeHash(buffer);
            int length = buffer.Length;
            int index = 0;
            while (length < numArray2.Length)
            {
                if ((int) numArray2[length] != (int) hash[index])
                    throw new CryptographicException("Cryptography problem: Bad key size (2)");
                ++length;
                ++index;
            }
            return buffer;
        }

        internal static byte[] AESKeyWrapEncrypt(byte[] rgbKey, byte[] rgbWrappedKeyData)
        {
            int num1 = rgbWrappedKeyData.Length >> 3;
            if (rgbWrappedKeyData.Length % 8 != 0 || num1 <= 0)
                throw new CryptographicException("Cryptography problem: Bad key size (3)");
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Key = rgbKey;
            rijndaelManaged.Mode = CipherMode.ECB;
            rijndaelManaged.Padding = PaddingMode.None;
            ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor();
            if (num1 == 1)
            {
                byte[] inputBuffer = new byte[SymmetricKeyWrap.s_rgbAES_KW_IV.Length + rgbWrappedKeyData.Length];
                Buffer.BlockCopy((Array) SymmetricKeyWrap.s_rgbAES_KW_IV, 0, (Array) inputBuffer, 0, SymmetricKeyWrap.s_rgbAES_KW_IV.Length);
                Buffer.BlockCopy((Array) rgbWrappedKeyData, 0, (Array) inputBuffer, SymmetricKeyWrap.s_rgbAES_KW_IV.Length, rgbWrappedKeyData.Length);
                return encryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            }
            byte[] numArray1 = new byte[num1 + 1 << 3];
            Buffer.BlockCopy((Array) rgbWrappedKeyData, 0, (Array) numArray1, 8, rgbWrappedKeyData.Length);
            byte[] numArray2 = new byte[8];
            byte[] inputBuffer1 = new byte[16];
            Buffer.BlockCopy((Array) SymmetricKeyWrap.s_rgbAES_KW_IV, 0, (Array) numArray2, 0, 8);
            for (int index1 = 0; index1 <= 5; ++index1)
            {
                for (int index2 = 1; index2 <= num1; ++index2)
                {
                    long num2 = (long) (index2 + index1 * num1);
                    Buffer.BlockCopy((Array) numArray2, 0, (Array) inputBuffer1, 0, 8);
                    Buffer.BlockCopy((Array) numArray1, 8 * index2, (Array) inputBuffer1, 8, 8);
                    byte[] numArray3 = encryptor.TransformFinalBlock(inputBuffer1, 0, 16);
                    for (int index3 = 0; index3 < 8; ++index3)
                    {
                        byte num3 = (byte) ((ulong) (num2 >> 8 * (7 - index3)) & (ulong) byte.MaxValue);
                        numArray2[index3] = (byte) ((uint) num3 ^ (uint) numArray3[index3]);
                    }
                    Buffer.BlockCopy((Array) numArray3, 8, (Array) numArray1, 8 * index2, 8);
                }
            }
            Buffer.BlockCopy((Array) numArray2, 0, (Array) numArray1, 0, 8);
            return numArray1;
        }

        internal static byte[] AESKeyWrapDecrypt(byte[] rgbKey, byte[] rgbEncryptedWrappedKeyData)
        {
            int num1 = (rgbEncryptedWrappedKeyData.Length >> 3) - 1;
            if (rgbEncryptedWrappedKeyData.Length % 8 != 0 || num1 <= 0)
                throw new CryptographicException("Cryptography problem: Bad key size (4)");
            byte[] numArray1 = new byte[num1 << 3];
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Key = rgbKey;
            rijndaelManaged.Mode = CipherMode.ECB;
            rijndaelManaged.Padding = PaddingMode.None;
            ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor();
            if (num1 == 1)
            {
                byte[] numArray2 = decryptor.TransformFinalBlock(rgbEncryptedWrappedKeyData, 0, rgbEncryptedWrappedKeyData.Length);
                for (int index = 0; index < 8; ++index)
                {
                    if ((int) numArray2[index] != (int) SymmetricKeyWrap.s_rgbAES_KW_IV[index])
                        throw new CryptographicException("Cryptography problem: Bad key size (5)");
                }
                Buffer.BlockCopy((Array) numArray2, 8, (Array) numArray1, 0, 8);
                return numArray1;
            }
            Buffer.BlockCopy((Array) rgbEncryptedWrappedKeyData, 8, (Array) numArray1, 0, numArray1.Length);
            byte[] numArray3 = new byte[8];
            byte[] inputBuffer = new byte[16];
            Buffer.BlockCopy((Array) rgbEncryptedWrappedKeyData, 0, (Array) numArray3, 0, 8);
            for (int index1 = 5; index1 >= 0; --index1)
            {
                for (int index2 = num1; index2 >= 1; --index2)
                {
                    long num2 = (long) (index2 + index1 * num1);
                    for (int index3 = 0; index3 < 8; ++index3)
                    {
                        byte num3 = (byte) ((ulong) (num2 >> 8 * (7 - index3)) & (ulong) byte.MaxValue);
                        numArray3[index3] ^= num3;
                    }
                    Buffer.BlockCopy((Array) numArray3, 0, (Array) inputBuffer, 0, 8);
                    Buffer.BlockCopy((Array) numArray1, 8 * (index2 - 1), (Array) inputBuffer, 8, 8);
                    byte[] numArray2 = decryptor.TransformFinalBlock(inputBuffer, 0, 16);
                    Buffer.BlockCopy((Array) numArray2, 8, (Array) numArray1, 8 * (index2 - 1), 8);
                    Buffer.BlockCopy((Array) numArray2, 0, (Array) numArray3, 0, 8);
                }
            }
            for (int index = 0; index < 8; ++index)
            {
                if ((int) numArray3[index] != (int) SymmetricKeyWrap.s_rgbAES_KW_IV[index])
                    throw new CryptographicException("Cryptography problem: Bad key size (6)");
            }
            return numArray1;
        }
    }
}