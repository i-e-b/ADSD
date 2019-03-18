using System;
using System.Security;
using System.Security.Cryptography;

namespace ADSD.Crypto
{

    /// <summary>Verifies an <see cref="T:System.Security.Cryptography.RSA" /> PKCS #1 version 1.5 signature.</summary>
    public class RSAPKCS1SignatureDeformatter : AsymmetricSignatureDeformatter
    {
        private RSA _rsaKey;
        private string _strOID;
        private bool? _rsaOverridesVerifyHash;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.RSAPKCS1SignatureDeformatter" /> class.</summary>
        public RSAPKCS1SignatureDeformatter()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.RSAPKCS1SignatureDeformatter" /> class with the specified key.</summary>
        /// <param name="key">The instance of <see cref="T:System.Security.Cryptography.RSA" /> that holds the public key. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key " />is <see langword="null" />.</exception>
        public RSAPKCS1SignatureDeformatter(AsymmetricAlgorithm key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            _rsaKey = (RSA)key;
        }

        /// <summary>Sets the public key to use for verifying the signature.</summary>
        /// <param name="key">The instance of <see cref="T:System.Security.Cryptography.RSA" /> that holds the public key. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key " />is <see langword="null" />.</exception>
        public override void SetKey(AsymmetricAlgorithm key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            _rsaKey = (RSA)key;
            _rsaOverridesVerifyHash = new bool?();
        }

        /// <summary>Sets the hash algorithm to use for verifying the signature.</summary>
        /// <param name="strName">The name of the hash algorithm to use for verifying the signature. </param>
        public override void SetHashAlgorithm(string strName)
        {
            _strOID = CryptoConfig.MapNameToOID(strName) ?? "2.16.840.1.101.3.4.2.1";
        }

        internal static int GetAlgIdFromOid(string oid, OidGroup oidGroup)
        {
            if (string.Equals(oid, "2.16.840.1.101.3.4.2.1", StringComparison.Ordinal))
                return 32780;
            if (string.Equals(oid, "2.16.840.1.101.3.4.2.2", StringComparison.Ordinal))
                return 32781;
            if (string.Equals(oid, "2.16.840.1.101.3.4.2.3", StringComparison.Ordinal))
                return 32782;

            // default to 2.1:
            return 32780;
            //return X509Utils.FindOidInfo(OidKeyType.Oid, oid, oidGroup).AlgId;
        }

        

        internal static HashAlgorithmName OidToHashAlgorithmName(string oid)
        {
            if (oid == "1.3.14.3.2.26")
                return HashAlgorithmName.SHA1;
            if (oid == "2.16.840.1.101.3.4.2.1")
                return HashAlgorithmName.SHA256;
            if (oid == "2.16.840.1.101.3.4.2.2")
                return HashAlgorithmName.SHA384;
            if (oid == "2.16.840.1.101.3.4.2.3")
                return HashAlgorithmName.SHA512;
            throw new NotSupportedException();
        }

        
        [SecurityCritical]
        internal static byte[] RsaPkcs1Padding(RSA rsa, byte[] oid, byte[] hash)
        {
            int length = rsa.KeySize / 8;
            byte[] numArray1 = new byte[length];
            byte[] numArray2 = new byte[oid.Length + 8 + hash.Length];
            numArray2[0] = (byte) 48;
            int num1 = numArray2.Length - 2;
            numArray2[1] = (byte) num1;
            numArray2[2] = (byte) 48;
            int num2 = oid.Length + 2;
            numArray2[3] = (byte) num2;
            Buffer.BlockCopy((Array) oid, 0, (Array) numArray2, 4, oid.Length);
            numArray2[4 + oid.Length] = (byte) 5;
            numArray2[4 + oid.Length + 1] = (byte) 0;
            numArray2[4 + oid.Length + 2] = (byte) 4;
            numArray2[4 + oid.Length + 3] = (byte) hash.Length;
            Buffer.BlockCopy((Array) hash, 0, (Array) numArray2, oid.Length + 8, hash.Length);
            int dstOffsetBytes = length - numArray2.Length;
            if (dstOffsetBytes <= 2)
                throw new CryptographicUnexpectedOperationException("Cryptography_InvalidOID");
            numArray1[0] = (byte) 0;
            numArray1[1] = (byte) 1;
            for (int index = 2; index < dstOffsetBytes - 1; ++index)
                numArray1[index] = byte.MaxValue;
            numArray1[dstOffsetBytes - 1] = (byte) 0;
            Buffer.BlockCopy((Array) numArray2, 0, (Array) numArray1, dstOffsetBytes, numArray2.Length);
            return numArray1;
        }

        /// <summary>Verifies the <see cref="T:System.Security.Cryptography.RSA" /> PKCS#1 signature for the specified data.</summary>
        /// <param name="rgbHash">The data signed with <paramref name="rgbSignature" />. </param>
        /// <param name="rgbSignature">The signature to be verified for <paramref name="rgbHash" />. </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="rgbSignature" /> matches the signature computed using the specified hash algorithm and key on <paramref name="rgbHash" />; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicUnexpectedOperationException">The key is <see langword="null" />.-or- The hash algorithm is <see langword="null" />. </exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="rgbHash" /> parameter is <see langword="null" />.-or- The <paramref name="rgbSignature" /> parameter is <see langword="null" />. </exception>
        [SecuritySafeCritical]
        public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
        {
            if (rgbHash == null)
                throw new ArgumentNullException(nameof(rgbHash));
            if (rgbSignature == null)
                throw new ArgumentNullException(nameof(rgbSignature));
            if (_strOID == null)
                throw new CryptographicUnexpectedOperationException("Cryptography_MissingOID");
            if (_rsaKey == null)
                throw new CryptographicUnexpectedOperationException("Cryptography_MissingKey");
            if (_rsaKey is RSACryptoServiceProvider)
            {
                throw new Exception("Unexpected path");
                //int algIdFromOid = GetAlgIdFromOid(_strOID, OidGroup.HashAlgorithm);
                //return ((RSACryptoServiceProvider)_rsaKey).VerifyHash(rgbHash, algIdFromOid, rgbSignature);
            }
            if (OverridesVerifyHash)
            {
                HashAlgorithmName hashAlgorithmName = OidToHashAlgorithmName(_strOID);
                return _rsaKey.VerifyHash(rgbHash, rgbSignature, hashAlgorithmName, RSASignaturePadding.Pkcs1);
            }
            byte[] rhs = RsaPkcs1Padding(_rsaKey, CryptoConfig.EncodeOID(_strOID), rgbHash);
            return CompareBigIntArrays(_rsaKey.EncryptValue(rgbSignature), rhs);
        }

        
        internal static bool CompareBigIntArrays(byte[] lhs, byte[] rhs)
        {
            if (lhs == null)
                return rhs == null;
            int index1 = 0;
            int index2 = 0;
            while (index1 < lhs.Length && lhs[index1] == (byte) 0)
                ++index1;
            while (index2 < rhs.Length && rhs[index2] == (byte) 0)
                ++index2;
            int num = lhs.Length - index1;
            if (rhs.Length - index2 != num)
                return false;
            for (int index3 = 0; index3 < num; ++index3)
            {
                if ((int) lhs[index1 + index3] != (int) rhs[index2 + index3])
                    return false;
            }
            return true;
        }


        private bool OverridesVerifyHash
        {
            get
            {
                if (!_rsaOverridesVerifyHash.HasValue)
                    _rsaOverridesVerifyHash = new bool?(DoesRsaKeyOverride(_rsaKey, "VerifyHash", new Type[4]
                    {
            typeof (byte[]),
            typeof (byte[]),
            typeof (HashAlgorithmName),
            typeof (RSASignaturePadding)
                    }));
                return _rsaOverridesVerifyHash.Value;
            }
        }

        private bool DoesRsaKeyOverride(RSA rsaKey, string verifyhash, Type[] types)
        {
            return true;
        }
    }
}
