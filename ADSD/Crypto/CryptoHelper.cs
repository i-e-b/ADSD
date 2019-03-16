using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

namespace ADSD
{
    internal static class CryptoHelper
    {
        private static Dictionary<string, Func<object>> algorithmDelegateDictionary = new Dictionary<string, Func<object>>();
        private static object AlgorithmDictionaryLock = new object();
        private static byte[] emptyBuffer;
        private static RandomNumberGenerator random;
        private static Rijndael rijndael;
        private static TripleDES tripleDES;
        public const int WindowsVistaMajorNumber = 6;
        private const string SHAString = "SHA";
        private const string SHA1String = "SHA1";
        private const string SHA256String = "SHA256";
        private const string SystemSecurityCryptographySha1String = "System.Security.Cryptography.SHA1";

        public static int CeilingDivide(int dividend, int divisor)
        {
            int num1 = dividend % divisor;
            int num2 = dividend / divisor;
            if (num1 > 0)
                ++num2;
            return num2;
        }

        internal static byte[] EmptyBuffer
        {
            get
            {
                if (CryptoHelper.emptyBuffer == null)
                    CryptoHelper.emptyBuffer = new byte[0];
                return CryptoHelper.emptyBuffer;
            }
        }

        internal static Rijndael Rijndael
        {
            get
            {
                if (CryptoHelper.rijndael == null)
                {
                    Rijndael rijndael = new RijndaelManaged();
                    rijndael.Padding = PaddingMode.ISO10126;
                    CryptoHelper.rijndael = rijndael;
                }
                return CryptoHelper.rijndael;
            }
        }

        internal static TripleDES TripleDES
        {
            get
            {
                if (CryptoHelper.tripleDES == null)
                {
                    TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
                    cryptoServiceProvider.Padding = PaddingMode.ISO10126;
                    CryptoHelper.tripleDES = (TripleDES) cryptoServiceProvider;
                }
                return CryptoHelper.tripleDES;
            }
        }

        internal static RandomNumberGenerator RandomNumberGenerator
        {
            get
            {
                if (CryptoHelper.random == null)
                    CryptoHelper.random = (RandomNumberGenerator) new RNGCryptoServiceProvider();
                return CryptoHelper.random;
            }
        }

        internal static SymmetricAlgorithm NewDefaultEncryption()
        {
            return CryptoHelper.GetSymmetricAlgorithm((byte[]) null, "http://www.w3.org/2001/04/xmlenc#aes256-cbc");
        }

        internal static HashAlgorithm NewSha1HashAlgorithm()
        {
            return CryptoHelper.CreateHashAlgorithm("http://www.w3.org/2000/09/xmldsig#sha1");
        }

        internal static HashAlgorithm NewSha256HashAlgorithm()
        {
            return CryptoHelper.CreateHashAlgorithm("http://www.w3.org/2001/04/xmlenc#sha256");
        }

        internal static KeyedHashAlgorithm NewHmacSha1KeyedHashAlgorithm()
        {
            KeyedHashAlgorithm algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig("http://www.w3.org/2000/09/xmldsig#hmac-sha1") as KeyedHashAlgorithm;
            if (algorithmFromConfig == null) throw new Exception("Unacceptable SHA1 HMAC. Failed to load matching algorithm from config");
            return algorithmFromConfig;
        }

        internal static KeyedHashAlgorithm NewHmacSha1KeyedHashAlgorithm(byte[] key)
        {
            return CryptoHelper.CreateKeyedHashAlgorithm(key, "http://www.w3.org/2000/09/xmldsig#hmac-sha1");
        }

        internal static KeyedHashAlgorithm NewHmacSha256KeyedHashAlgorithm(byte[] key)
        {
            return CryptoHelper.CreateKeyedHashAlgorithm(key, "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256");
        }

        internal static Rijndael NewRijndaelSymmetricAlgorithm()
        {
            Rijndael symmetricAlgorithm = CryptoHelper.GetSymmetricAlgorithm((byte[]) null, "http://www.w3.org/2001/04/xmlenc#aes128-cbc") as Rijndael;
            if (symmetricAlgorithm != null) return symmetricAlgorithm;

            throw new InvalidOperationException("CustomCryptoAlgorithmIsNotValidSymmetricAlgorithm");
        }

        internal static ICryptoTransform CreateDecryptor(
            byte[] key,
            byte[] iv,
            string algorithm)
        {
            object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            if (algorithmFromConfig != null)
            {
                SymmetricAlgorithm symmetricAlgorithm = algorithmFromConfig as SymmetricAlgorithm;
                if (symmetricAlgorithm != null) return symmetricAlgorithm.CreateDecryptor(key, iv);

                throw new InvalidOperationException("CustomCryptoAlgorithmIsNotValidSymmetricAlgorithm: "+ algorithm);
            }
            if (algorithm == "http://www.w3.org/2001/04/xmlenc#tripledes-cbc")
                return CryptoHelper.TripleDES.CreateDecryptor(key, iv);
            if (algorithm == "http://www.w3.org/2001/04/xmlenc#aes128-cbc" || algorithm == "http://www.w3.org/2001/04/xmlenc#aes192-cbc" || algorithm == "http://www.w3.org/2001/04/xmlenc#aes256-cbc")
                return CryptoHelper.Rijndael.CreateDecryptor(key, iv);
            throw new InvalidOperationException("UnsupportedEncryptionAlgorithm: "+ algorithm);
        }

        internal static ICryptoTransform CreateEncryptor(
            byte[] key,
            byte[] iv,
            string algorithm)
        {
            object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            if (algorithmFromConfig != null)
            {
                SymmetricAlgorithm symmetricAlgorithm = algorithmFromConfig as SymmetricAlgorithm;
                if (symmetricAlgorithm != null)
                    return symmetricAlgorithm.CreateEncryptor(key, iv);
                throw new InvalidOperationException("CustomCryptoAlgorithmIsNotValidSymmetricAlgorithm: " + algorithm);
            }
            if (algorithm == "http://www.w3.org/2001/04/xmlenc#tripledes-cbc")
                return CryptoHelper.TripleDES.CreateEncryptor(key, iv);
            if (algorithm == "http://www.w3.org/2001/04/xmlenc#aes128-cbc" || algorithm == "http://www.w3.org/2001/04/xmlenc#aes192-cbc" || algorithm == "http://www.w3.org/2001/04/xmlenc#aes256-cbc")
                return CryptoHelper.Rijndael.CreateEncryptor(key, iv);
            throw new InvalidOperationException("UnsupportedEncryptionAlgorithm:"+algorithm);
        }

        internal static HashAlgorithm CreateHashAlgorithm(string algorithm)
        {
            object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            if (algorithmFromConfig != null)
            {
                HashAlgorithm hashAlgorithm = algorithmFromConfig as HashAlgorithm;
                if (hashAlgorithm != null)
                    return hashAlgorithm;
                throw new InvalidOperationException("CustomCryptoAlgorithmIsNotValidHashAlgorithm: " + algorithm);
            }
            if (algorithm != "SHA" && algorithm != "SHA1" && algorithm != "System.Security.Cryptography.SHA1" && algorithm != "http://www.w3.org/2000/09/xmldsig#sha1")
            {
                if (algorithm == "SHA256" || algorithm == "http://www.w3.org/2001/04/xmlenc#sha256")
                {
                    return (HashAlgorithm) new SHA256Managed();
                }
                throw new InvalidOperationException("UnsupportedCryptoAlgorithm: " + algorithm);
            }
            return (HashAlgorithm) new SHA1Managed();
        }

        internal static KeyedHashAlgorithm CreateKeyedHashAlgorithm(
            byte[] key,
            string algorithm)
        {
            object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            if (algorithmFromConfig != null)
            {
                KeyedHashAlgorithm keyedHashAlgorithm = algorithmFromConfig as KeyedHashAlgorithm;
                if (keyedHashAlgorithm != null)
                {
                    keyedHashAlgorithm.Key = key;
                    return keyedHashAlgorithm;
                }
                throw new InvalidOperationException("CustomCryptoAlgorithmIsNotValidKeyedHashAlgorithm: "+algorithm);
            }
            if (algorithm == "http://www.w3.org/2000/09/xmldsig#hmac-sha1")
                return (KeyedHashAlgorithm) new HMACSHA1(key);
            if (algorithm == "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256")
            {
                return (KeyedHashAlgorithm)new HMACSHA256(key);
            }
            throw new InvalidOperationException("UnsupportedCryptoAlgorithm: " + algorithm);
        }

        internal static byte[] ComputeHash(byte[] buffer)
        {
            using (HashAlgorithm hashAlgorithm = CryptoHelper.NewSha1HashAlgorithm())
                return hashAlgorithm.ComputeHash(buffer);
        }

        internal static byte[] GenerateDerivedKey(
            byte[] key,
            string algorithm,
            byte[] label,
            byte[] nonce,
            int derivedKeySize,
            int position)
        {
            if (algorithm != "http://schemas.xmlsoap.org/ws/2005/02/sc/dk/p_sha1" && algorithm != "http://docs.oasis-open.org/ws-sx/ws-secureconversation/200512/dk/p_sha1")
                throw new InvalidOperationException("UnsupportedKeyDerivationAlgorithm: " + algorithm);

            return new Psha1DerivedKeyGenerator(key).GenerateDerivedKey(label, nonce, derivedKeySize, position);
        }

        internal static int GetIVSize(string algorithm)
        {
            object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            if (algorithmFromConfig != null)
            {
                var symmetricAlgorithm = algorithmFromConfig as SymmetricAlgorithm;
                if (symmetricAlgorithm != null) return symmetricAlgorithm.BlockSize;

                throw new InvalidOperationException("CustomCryptoAlgorithmIsNotValidSymmetricAlgorithm: " + algorithm);
            }
            if (algorithm == "http://www.w3.org/2001/04/xmlenc#tripledes-cbc") return CryptoHelper.TripleDES.BlockSize;
            if (algorithm == "http://www.w3.org/2001/04/xmlenc#aes128-cbc" || algorithm == "http://www.w3.org/2001/04/xmlenc#aes192-cbc" || algorithm == "http://www.w3.org/2001/04/xmlenc#aes256-cbc")
                return CryptoHelper.Rijndael.BlockSize;

            throw new InvalidOperationException("UnsupportedEncryptionAlgorithm: " + algorithm);
        }

        internal static void FillRandomBytes(byte[] buffer)
        {
            CryptoHelper.RandomNumberGenerator.GetBytes(buffer);
        }

        public static void GenerateRandomBytes(byte[] data)
        {
            CryptoHelper.RandomNumberGenerator.GetNonZeroBytes(data);
        }

        public static byte[] GenerateRandomBytes(int sizeInBits)
        {
            int length = sizeInBits / 8;
            if (sizeInBits <= 0) throw new ArgumentOutOfRangeException(nameof(sizeInBits));
            if (length * 8 != sizeInBits) throw new ArgumentOutOfRangeException(nameof(sizeInBits));

            byte[] data = new byte[length];
            CryptoHelper.GenerateRandomBytes(data);
            return data;
        }

        internal static SymmetricAlgorithm GetSymmetricAlgorithm(
            byte[] key,
            string algorithm)
        {
            object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            if (algorithmFromConfig != null)
            {
                SymmetricAlgorithm symmetricAlgorithm = algorithmFromConfig as SymmetricAlgorithm;
                if (symmetricAlgorithm != null)
                {
                    if (key != null) symmetricAlgorithm.Key = key;
                    return symmetricAlgorithm;
                }
                throw new InvalidOperationException("CustomCryptoAlgorithmIsNotValidSymmetricAlgorithm: " + algorithm);
            }
            SymmetricAlgorithm symmetricAlgorithm1;
            switch (algorithm)
            {
                case "http://www.w3.org/2001/04/xmlenc#aes128-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes192-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes256-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes128":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes192":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes256":
                    symmetricAlgorithm1 = new RijndaelManaged();
                    break;
                case "http://www.w3.org/2001/04/xmlenc#kw-tripledes":
                case "http://www.w3.org/2001/04/xmlenc#tripledes-cbc":
                    symmetricAlgorithm1 = (SymmetricAlgorithm) new TripleDESCryptoServiceProvider();
                    break;
                default:
                    throw new InvalidOperationException("UnsupportedEncryptionAlgorithm: " + algorithm);
            }
            if (key != null)
                symmetricAlgorithm1.Key = key;
            return symmetricAlgorithm1;
        }

        internal static byte[] CreateSignatureForSha256(AsymmetricSignatureFormatter formatter, HashAlgorithm hash)
        {
            return formatter.CreateSignature(hash);
        }

        internal static bool VerifySignatureForSha256(AsymmetricSignatureDeformatter deformatter, HashAlgorithm hash, byte[] signatureValue)
        {
            return deformatter.VerifySignature(hash, signatureValue);
        }

        internal static AsymmetricSignatureFormatter GetSignatureFormatterForSha256( AsymmetricSecurityKey key)
        {
            AsymmetricAlgorithm asymmetricAlgorithm = key.GetAsymmetricAlgorithm("http://www.w3.org/2001/04/xmldsig-more#rsa-sha256", true);
            RSACryptoServiceProvider rsaProvider = asymmetricAlgorithm as RSACryptoServiceProvider;
            if (rsaProvider != null)
                return CryptoHelper.GetSignatureFormatterForSha256(rsaProvider);
            return (AsymmetricSignatureFormatter) new RSAPKCS1SignatureFormatter(asymmetricAlgorithm);
        }

        internal static AsymmetricSignatureFormatter GetSignatureFormatterForSha256(
            RSACryptoServiceProvider rsaProvider)
        {
            CspParameters parameters = new CspParameters();
            parameters.ProviderType = 24;
            if (24 == rsaProvider.CspKeyContainerInfo.ProviderType)
                parameters.ProviderName = rsaProvider.CspKeyContainerInfo.ProviderName;
            parameters.KeyContainerName = rsaProvider.CspKeyContainerInfo.KeyContainerName;
            parameters.KeyNumber = (int) rsaProvider.CspKeyContainerInfo.KeyNumber;
            if (rsaProvider.CspKeyContainerInfo.MachineKeyStore)
                parameters.Flags = CspProviderFlags.UseMachineKeyStore;
            parameters.Flags |= CspProviderFlags.UseExistingKey;
            rsaProvider = new RSACryptoServiceProvider(parameters);
            return (AsymmetricSignatureFormatter) new RSAPKCS1SignatureFormatter((AsymmetricAlgorithm) rsaProvider);
        }

        internal static AsymmetricSignatureDeformatter GetSignatureDeFormatterForSha256(
            AsymmetricSecurityKey key)
        {
            AsymmetricAlgorithm asymmetricAlgorithm = key.GetAsymmetricAlgorithm("http://www.w3.org/2001/04/xmldsig-more#rsa-sha256", false);
            RSACryptoServiceProvider rsaProvider = asymmetricAlgorithm as RSACryptoServiceProvider;
            if (rsaProvider != null)
                return CryptoHelper.GetSignatureDeFormatterForSha256(rsaProvider);
            return (AsymmetricSignatureDeformatter) new RSAPKCS1SignatureDeformatter(asymmetricAlgorithm);
        }

        internal static AsymmetricSignatureDeformatter GetSignatureDeFormatterForSha256(
            RSACryptoServiceProvider rsaProvider)
        {
            CspParameters parameters = new CspParameters();
            parameters.ProviderType = 24;
            if (24 == rsaProvider.CspKeyContainerInfo.ProviderType)
                parameters.ProviderName = rsaProvider.CspKeyContainerInfo.ProviderName;
            parameters.KeyNumber = (int) rsaProvider.CspKeyContainerInfo.KeyNumber;
            if (rsaProvider.CspKeyContainerInfo.MachineKeyStore)
                parameters.Flags = CspProviderFlags.UseMachineKeyStore;
            parameters.Flags |= CspProviderFlags.UseExistingKey;
            RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(parameters);
            cryptoServiceProvider.ImportCspBlob(rsaProvider.ExportCspBlob(false));
            return (AsymmetricSignatureDeformatter) new RSAPKCS1SignatureDeformatter((AsymmetricAlgorithm) cryptoServiceProvider);
        }

        internal static bool IsAsymmetricAlgorithm(string algorithm)
        {
            object obj;
            try
            {
                obj = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            }
            catch (InvalidOperationException)
            {
                obj = (object) null;
            }
            if (obj != null)
            {
                AsymmetricAlgorithm asymmetricAlgorithm = obj as AsymmetricAlgorithm;
                SignatureDescription signatureDescription = obj as SignatureDescription;
                return asymmetricAlgorithm != null || signatureDescription != null;
            }
            return algorithm == "http://www.w3.org/2000/09/xmldsig#dsa-sha1" || algorithm == "http://www.w3.org/2000/09/xmldsig#rsa-sha1" || (algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256" || algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p") || algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-1_5";
        }

        internal static bool IsSymmetricAlgorithm(string algorithm)
        {
            object obj;
            try
            {
                obj = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            }
            catch (InvalidOperationException)
            {
                obj = (object) null;
            }
            if (obj != null)
            {
                SymmetricAlgorithm symmetricAlgorithm = obj as SymmetricAlgorithm;
                KeyedHashAlgorithm keyedHashAlgorithm = obj as KeyedHashAlgorithm;
                return symmetricAlgorithm != null || keyedHashAlgorithm != null;
            }
            switch (algorithm)
            {
                case "http://docs.oasis-open.org/ws-sx/ws-secureconversation/200512/dk/p_sha1":
                case "http://schemas.xmlsoap.org/ws/2005/02/sc/dk/p_sha1":
                case "http://www.w3.org/2000/09/xmldsig#hmac-sha1":
                case "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256":
                case "http://www.w3.org/2001/04/xmlenc#aes128-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes192-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes256-cbc":
                case "http://www.w3.org/2001/04/xmlenc#des-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes128":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes192":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes256":
                case "http://www.w3.org/2001/04/xmlenc#kw-tripledes":
                case "http://www.w3.org/2001/04/xmlenc#tripledes-cbc":
                    return true;
                case "http://www.w3.org/2000/09/xmldsig#dsa-sha1":
                case "http://www.w3.org/2000/09/xmldsig#rsa-sha1":
                case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256":
                case "http://www.w3.org/2001/04/xmlenc#rsa-1_5":
                case "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p":
                    return false;
                default:
                    return false;
            }
        }

        internal static bool IsSymmetricSupportedAlgorithm(string algorithm, int keySize)
        {
            bool flag = false;
            object obj = (object) null;
            try
            {
                obj = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            }
            catch (InvalidOperationException)
            {
            }
            if (obj != null)
            {
                SymmetricAlgorithm symmetricAlgorithm = obj as SymmetricAlgorithm;
                KeyedHashAlgorithm keyedHashAlgorithm = obj as KeyedHashAlgorithm;
                if (symmetricAlgorithm != null || keyedHashAlgorithm != null)
                    flag = true;
            }
            switch (algorithm)
            {
                case "http://docs.oasis-open.org/ws-sx/ws-secureconversation/200512/dk/p_sha1":
                case "http://schemas.xmlsoap.org/ws/2005/02/sc/dk/p_sha1":
                case "http://www.w3.org/2000/09/xmldsig#hmac-sha1":
                case "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256":
                    return true;
                case "http://www.w3.org/2000/09/xmldsig#dsa-sha1":
                case "http://www.w3.org/2000/09/xmldsig#rsa-sha1":
                case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256":
                case "http://www.w3.org/2001/04/xmlenc#rsa-1_5":
                case "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p":
                    return false;
                case "http://www.w3.org/2001/04/xmlenc#aes128-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes128":
                    if (keySize >= 128)
                        return keySize <= 256;
                    return false;
                case "http://www.w3.org/2001/04/xmlenc#aes192-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes192":
                    if (keySize >= 192)
                        return keySize <= 256;
                    return false;
                case "http://www.w3.org/2001/04/xmlenc#aes256-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes256":
                    return keySize == 256;
                case "http://www.w3.org/2001/04/xmlenc#kw-tripledes":
                case "http://www.w3.org/2001/04/xmlenc#tripledes-cbc":
                    if (keySize != 128)
                        return keySize == 192;
                    return true;
                default:
                    return flag;
            }
        }

        internal static byte[] UnwrapKey(byte[] wrappingKey, byte[] wrappedKey, string algorithm)
        {
            object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            if (algorithmFromConfig != null)
            {
                SymmetricAlgorithm symmetricAlgorithm = algorithmFromConfig as SymmetricAlgorithm;
                if (symmetricAlgorithm == null)
                    throw new InvalidOperationException("InvalidCustomKeyWrapAlgorithm: "+algorithm);
                using (symmetricAlgorithm)
                {
                    symmetricAlgorithm.Key = wrappingKey;
                    return EncryptedXml.DecryptKey(wrappedKey, symmetricAlgorithm);
                }
            }
            else
            {
                SymmetricAlgorithm symmetricAlgorithm;
                if (algorithm != "http://www.w3.org/2001/04/xmlenc#kw-tripledes")
                {
                    if (algorithm == "http://www.w3.org/2001/04/xmlenc#kw-aes128"
                        || algorithm == "http://www.w3.org/2001/04/xmlenc#kw-aes192"
                        || algorithm == "http://www.w3.org/2001/04/xmlenc#kw-aes256")
                        symmetricAlgorithm = new RijndaelManaged();
                    else
                        throw new InvalidOperationException("UnsupportedKeyWrapAlgorithm: " + algorithm);
                }
                else
                    symmetricAlgorithm = (SymmetricAlgorithm) new TripleDESCryptoServiceProvider();
                using (symmetricAlgorithm)
                {
                    symmetricAlgorithm.Key = wrappingKey;
                    return EncryptedXml.DecryptKey(wrappedKey, symmetricAlgorithm);
                }
            }
        }

        internal static byte[] WrapKey(byte[] wrappingKey, byte[] keyToBeWrapped, string algorithm)
        {
            object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            if (algorithmFromConfig != null)
            {
                SymmetricAlgorithm symmetricAlgorithm = algorithmFromConfig as SymmetricAlgorithm;
                if (symmetricAlgorithm == null)
                    throw new InvalidOperationException("InvalidCustomKeyWrapAlgorithm: "+ algorithm);
                using (symmetricAlgorithm)
                {
                    symmetricAlgorithm.Key = wrappingKey;
                    return EncryptedXml.EncryptKey(keyToBeWrapped, symmetricAlgorithm);
                }
            }
            else
            {
                SymmetricAlgorithm symmetricAlgorithm;
                if (algorithm != "http://www.w3.org/2001/04/xmlenc#kw-tripledes")
                {
                    if (algorithm == "http://www.w3.org/2001/04/xmlenc#kw-aes128"
                        || algorithm == "http://www.w3.org/2001/04/xmlenc#kw-aes192"
                        || algorithm == "http://www.w3.org/2001/04/xmlenc#kw-aes256")
                        symmetricAlgorithm = new RijndaelManaged();
                    else
                        throw new InvalidOperationException("UnsupportedKeyWrapAlgorithm: " + algorithm);
                }
                else
                    symmetricAlgorithm = (SymmetricAlgorithm) new TripleDESCryptoServiceProvider();
                using (symmetricAlgorithm)
                {
                    symmetricAlgorithm.Key = wrappingKey;
                    return EncryptedXml.EncryptKey(keyToBeWrapped, symmetricAlgorithm);
                }
            }
        }

        internal static void ValidateBufferBounds(Array buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof (buffer));
            if (count < 0 || count > buffer.Length) throw new ArgumentOutOfRangeException(nameof(count));
            if (offset < 0 || offset > buffer.Length - count) throw new ArgumentOutOfRangeException(nameof (offset));
        }

        internal static bool IsEqual(byte[] a, byte[] b)
        {
            if (a == b)
                return true;
            if (a == null || b == null || a.Length != b.Length)
                return false;
            for (int index = 0; index < a.Length; ++index)
            {
                if ((int) a[index] != (int) b[index])
                    return false;
            }
            return true;
        }

        private static object GetDefaultAlgorithm(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm)) throw new ArgumentNullException(nameof(algorithm));

            switch (algorithm)
            {
                case "SHA256":
                case "http://www.w3.org/2001/04/xmlenc#sha256":
                    return (object) new SHA256Managed();
                case "http://www.w3.org/2000/09/xmldsig#hmac-sha1":
                    byte[] numArray = new byte[64];
                    new RNGCryptoServiceProvider().GetBytes(numArray);
                    return (object) new HMACSHA1(numArray);
                case "http://www.w3.org/2000/09/xmldsig#sha1":
                    return (object) new SHA1Managed();
                case "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256":
                    return (object)new HMACSHA256();
                case "http://www.w3.org/2001/04/xmlenc#aes128-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes192-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes256-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes128":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes192":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes256":
                    return (object) new RijndaelManaged();
                case "http://www.w3.org/2001/04/xmlenc#des-cbc":
                    return (object) new DESCryptoServiceProvider();
                case "http://www.w3.org/2001/04/xmlenc#kw-tripledes":
                case "http://www.w3.org/2001/04/xmlenc#tripledes-cbc":
                    return (object) new TripleDESCryptoServiceProvider();
                case "http://www.w3.org/2001/04/xmlenc#ripemd160":
                        return (object) new RIPEMD160Managed();
                case "http://www.w3.org/2001/04/xmlenc#sha512":
                    return (object) new SHA512Managed();
                case "http://www.w3.org/2001/10/xml-exc-c14n#":
                    return (object) new XmlDsigExcC14NTransform();
                case "http://www.w3.org/2001/10/xml-exc-c14n#WithComments":
                    return (object) new XmlDsigExcC14NWithCommentsTransform();
                default:
                    return (object) null;
            }
        }

        internal static object GetAlgorithmFromConfig(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm)) throw new ArgumentNullException(nameof (algorithm));

            object obj = (object) null;
            Func<object> func1 = (Func<object>) null;
            if (!CryptoHelper.algorithmDelegateDictionary.TryGetValue(algorithm, out func1))
            {
                lock (CryptoHelper.AlgorithmDictionaryLock)
                {
                    if (!CryptoHelper.algorithmDelegateDictionary.ContainsKey(algorithm))
                    {
                        try
                        {
                            obj = CryptoConfig.CreateFromName(algorithm);
                        }
                        catch (TargetInvocationException ex)
                        {
                            Console.WriteLine(ex);
                            CryptoHelper.algorithmDelegateDictionary[algorithm] = (Func<object>) null;
                        }
                        if (obj == null)
                        {
                            CryptoHelper.algorithmDelegateDictionary[algorithm] = (Func<object>) null;
                        }
                        else
                        {
                            object defaultAlgorithm = CryptoHelper.GetDefaultAlgorithm(algorithm);
                            if (obj is SHA1CryptoServiceProvider || defaultAlgorithm != null && defaultAlgorithm.GetType() == obj.GetType())
                            {
                                CryptoHelper.algorithmDelegateDictionary[algorithm] = (Func<object>) null;
                            }
                            else
                            {
                                CryptoHelper.algorithmDelegateDictionary[algorithm] =
                                    () => Expression.New(obj.GetType()).Constructor.Invoke(new object[0]);
                                return obj;
                            }
                        }
                    }
                }
            }
            else if (func1 != null)
                return func1();
            if (algorithm != "SHA256" && algorithm != "http://www.w3.org/2001/04/xmlenc#sha256")
            {
                if (algorithm != "http://www.w3.org/2000/09/xmldsig#sha1")
                {
                    if (algorithm == "http://www.w3.org/2000/09/xmldsig#hmac-sha1")
                        return (object) new HMACSHA1(CryptoHelper.GenerateRandomBytes(64));
                    return (object) null;
                }
                return (object) new SHA1Managed();
            }
            return (object) new SHA256Managed();
        }

        public static void ResetAllCertificates(X509Certificate2Collection certificates)
        {
            if (certificates == null)
                return;
            for (int index = 0; index < certificates.Count; ++index)
                certificates[index].Reset();
        }

        public static class KeyGenerator
        {
            private static RandomNumberGenerator _random = CryptoHelper.RandomNumberGenerator;
            private const int _maxKeyIterations = 20;

            public static byte[] ComputeCombinedKey(
                byte[] requestorEntropy,
                byte[] issuerEntropy,
                int keySizeInBits)
            {
                if (requestorEntropy == null)
                    throw new ArgumentNullException(nameof (requestorEntropy));
                if (issuerEntropy == null)
                    throw new ArgumentNullException(nameof (issuerEntropy));
                int length = CryptoHelper.KeyGenerator.ValidateKeySizeInBytes(keySizeInBits);
                byte[] numArray1 = new byte[length];
                using (KeyedHashAlgorithm keyedHashAlgorithm = CryptoHelper.NewHmacSha1KeyedHashAlgorithm())
                {
                    keyedHashAlgorithm.Key = requestorEntropy;
                    byte[] buffer1 = issuerEntropy;
                    byte[] buffer2 = new byte[keyedHashAlgorithm.HashSize / 8 + buffer1.Length];
                    byte[] numArray2 = (byte[]) null;
                    try
                    {
                        int num = 0;
                        label_10:
                        while (num < length)
                        {
                            keyedHashAlgorithm.Initialize();
                            buffer1 = keyedHashAlgorithm.ComputeHash(buffer1);
                            buffer1.CopyTo((Array) buffer2, 0);
                            issuerEntropy.CopyTo((Array) buffer2, buffer1.Length);
                            keyedHashAlgorithm.Initialize();
                            numArray2 = keyedHashAlgorithm.ComputeHash(buffer2);
                            int index = 0;
                            while (true)
                            {
                                if (index < numArray2.Length && num < length)
                                {
                                    numArray1[num++] = numArray2[index];
                                    ++index;
                                }
                                else
                                    goto label_10;
                            }
                        }
                    }
                    catch
                    {
                        Array.Clear((Array) numArray1, 0, numArray1.Length);
                        throw;
                    }
                    finally
                    {
                        if (numArray2 != null)
                            Array.Clear((Array) numArray2, 0, numArray2.Length);
                        Array.Clear((Array) buffer2, 0, buffer2.Length);
                        keyedHashAlgorithm.Clear();
                    }
                }
                return numArray1;
            }

            public static byte[] GenerateSymmetricKey(int keySizeInBits)
            {
                byte[] data = new byte[CryptoHelper.KeyGenerator.ValidateKeySizeInBytes(keySizeInBits)];
                CryptoHelper.GenerateRandomBytes(data);
                return data;
            }

            public static byte[] GenerateSymmetricKey(
                int keySizeInBits,
                byte[] senderEntropy,
                out byte[] receiverEntropy)
            {
                if (senderEntropy == null)
                    throw new ArgumentNullException(nameof (senderEntropy));
                int length = CryptoHelper.KeyGenerator.ValidateKeySizeInBytes(keySizeInBits);
                receiverEntropy = new byte[length];
                CryptoHelper.KeyGenerator._random.GetNonZeroBytes(receiverEntropy);
                return CryptoHelper.KeyGenerator.ComputeCombinedKey(senderEntropy, receiverEntropy, keySizeInBits);
            }

            public static byte[] GenerateDESKey(int keySizeInBits)
            {
                byte[] numArray = new byte[CryptoHelper.KeyGenerator.ValidateKeySizeInBytes(keySizeInBits)];
                int num = 0;
                while (num <= 20)
                {
                    CryptoHelper.GenerateRandomBytes(numArray);
                    ++num;
                    if (!TripleDES.IsWeakKey(numArray))
                        return numArray;
                }
                throw new CryptographicException("Failed to generate DES key");
            }

            public static byte[] GenerateDESKey(
                int keySizeInBits,
                byte[] senderEntropy,
                out byte[] receiverEntropy)
            {
                int length = CryptoHelper.KeyGenerator.ValidateKeySizeInBytes(keySizeInBits);
                byte[] numArray = new byte[length];
                int num = 0;
                while (num <= 20)
                {
                    receiverEntropy = new byte[length];
                    CryptoHelper.KeyGenerator._random.GetNonZeroBytes(receiverEntropy);
                    byte[] combinedKey = CryptoHelper.KeyGenerator.ComputeCombinedKey(senderEntropy, receiverEntropy, keySizeInBits);
                    ++num;
                    if (!TripleDES.IsWeakKey(combinedKey))
                        return combinedKey;
                }
                throw new CryptographicException("Failed to generate DES key");
            }

            private static int ValidateKeySizeInBytes(int keySizeInBits)
            {
                int num = keySizeInBits / 8;
                if (keySizeInBits <= 0) throw new ArgumentOutOfRangeException(nameof (keySizeInBits));
                if (num * 8 != keySizeInBits) throw new ArgumentOutOfRangeException(nameof (keySizeInBits));
                return num;
            }

            public static SecurityKeyIdentifier GetSecurityKeyIdentifier(
                byte[] secret,
                EncryptingCredentials wrappingCredentials)
            {
                if (secret == null) throw new ArgumentNullException(nameof (secret));
                if (secret.Length == 0) throw new ArgumentOutOfRangeException(nameof (secret));

                if (wrappingCredentials == null || wrappingCredentials.SecurityKey == null)
                    return new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[1]
                    {
                        (SecurityKeyIdentifierClause) new BinarySecretKeyIdentifierClause(secret)
                    });
                return new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[1]
                {
                    (SecurityKeyIdentifierClause) new EncryptedKeyIdentifierClause(wrappingCredentials.SecurityKey.EncryptKey(wrappingCredentials.Algorithm, secret), wrappingCredentials.Algorithm, wrappingCredentials.SecurityKeyIdentifier)
                });
            }
        }
    }
}