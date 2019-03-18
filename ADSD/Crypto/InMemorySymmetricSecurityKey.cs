using System;
using System.Security.Cryptography;

namespace ADSD.Crypto
{
    /// <summary>Represents keys that are generated using symmetric algorithms and are only stored in the local computer's random access memory.</summary>
    public class InMemorySymmetricSecurityKey : SymmetricSecurityKey
    {
        private readonly int keySize;
        private readonly byte[] symmetricKey;

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.InMemorySymmetricSecurityKey" /> class using the specified symmetric key. </summary>
        /// <param name="symmetricKey">An array of <see cref="T:System.Byte" /> that contains the symmetric key.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="symmetricKey" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="symmetricKey" /> is zero length.</exception>
        public InMemorySymmetricSecurityKey(byte[] symmetricKey)
            : this(symmetricKey, true)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.InMemorySymmetricSecurityKey" /> class using the specified symmetric key and a value that indicates whether the binary data must be cloned. </summary>
        /// <param name="symmetricKey">An array of <see cref="T:System.Byte" /> that contains the symmetric key.</param>
        /// <param name="cloneBuffer">
        /// <see langword="true" /> to clone the array passed into the <paramref name="symmetricKey" /> parameter; otherwise, <see langword="false" />. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="symmetricKey" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="symmetricKey" /> is zero length.</exception>
        public InMemorySymmetricSecurityKey(byte[] symmetricKey, bool cloneBuffer)
        {
            if (symmetricKey == null) throw  new ArgumentNullException(nameof (symmetricKey));
            if (symmetricKey.Length == 0) throw new ArgumentException("SymmetricKeyLengthTooShort", nameof(symmetricKey));

            this.keySize = symmetricKey.Length * 8;
            if (cloneBuffer)
            {
                this.symmetricKey = new byte[symmetricKey.Length];
                Buffer.BlockCopy((Array) symmetricKey, 0, (Array) this.symmetricKey, 0, symmetricKey.Length);
            }
            else
                this.symmetricKey = symmetricKey;
        }

        /// <summary>Gets the size, in bits, of the key.</summary>
        /// <returns>The size, in bits, of the key.</returns>
        public override int KeySize
        {
            get
            {
                return this.keySize;
            }
        }

        /// <summary>Decrypts the specified encrypted key.</summary>
        /// <param name="algorithm">The cryptographic algorithm that was used to encrypt the key.</param>
        /// <param name="keyData">An array of <see cref="T:System.Byte" /> that contains the encrypted key.</param>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the decrypted key.</returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// <paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.TripleDesKeyWrap" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes128KeyWrap" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes192KeyWrap" />, and <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes256KeyWrap" />.</exception>
        public override byte[] DecryptKey(string algorithm, byte[] keyData)
        {
            return CryptoHelper.UnwrapKey(this.symmetricKey, keyData, algorithm);
        }

        /// <summary>Encrypts the specified key.</summary>
        /// <param name="algorithm">The cryptographic algorithm to encrypt the key with.</param>
        /// <param name="keyData">An array of <see cref="T:System.Byte" /> that contains the key.</param>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the encrypted key.</returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// <paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.TripleDesKeyWrap" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes128KeyWrap" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes192KeyWrap" />, or <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes256KeyWrap" />.</exception>
        public override byte[] EncryptKey(string algorithm, byte[] keyData)
        {
            return CryptoHelper.WrapKey(this.symmetricKey, keyData, algorithm);
        }

        /// <summary>Generates a derived key using the specified cryptographic algorithm and parameters for the current key.</summary>
        /// <param name="algorithm">A URI that represents the cryptographic algorithm to use to generate the derived key.</param>
        /// <param name="label">An array of <see cref="T:System.Byte" /> that contains the label parameter for the cryptographic algorithm.</param>
        /// <param name="nonce">An array of <see cref="T:System.Byte" /> that contains the nonce that is used to create a derived key.</param>
        /// <param name="derivedKeyLength">The size of the derived key.</param>
        /// <param name="offset">The position at which the derived key is located in the byte array that is returned from this method.</param>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the derived key.</returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// <paramref name="algorithm" /> is not supported. The supported algorithms is <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Psha1KeyDerivation" />.</exception>
        public override byte[] GenerateDerivedKey(
            string algorithm,
            byte[] label,
            byte[] nonce,
            int derivedKeyLength,
            int offset)
        {
            return CryptoHelper.GenerateDerivedKey(this.symmetricKey, algorithm, label, nonce, derivedKeyLength, offset);
        }

        /// <summary>Gets a transform that decrypts cipher text using the specified cryptographic algorithm.</summary>
        /// <param name="algorithm">A cryptographic algorithm that decrypts cipher text, such as encrypted XML.</param>
        /// <param name="iv">An array of <see cref="T:System.Byte" /> that contains the initialization vector (<see langword="IV" />) for the specified algorithm.</param>
        /// <returns>An <see cref="T:System.Security.Cryptography.ICryptoTransform" /> that represents the decryption transform.</returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// <paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.TripleDesEncryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes128Encryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes192Encryption" />, and <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes256Encryption" />.</exception>
        public override ICryptoTransform GetDecryptionTransform(
            string algorithm,
            byte[] iv)
        {
            return CryptoHelper.CreateDecryptor(this.symmetricKey, iv, algorithm);
        }

        /// <summary>Gets a transform that encrypts XML using the specified cryptographic algorithm.</summary>
        /// <param name="algorithm">A cryptographic algorithm that encrypts XML.</param>
        /// <param name="iv">An array of <see cref="T:System.Byte" /> that contains the initialization vector (<see langword="IV" />) for the specified algorithm.</param>
        /// <returns>An <see cref="T:System.Security.Cryptography.ICryptoTransform" /> that represents the encryption transform.</returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// <paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.TripleDesEncryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes128Encryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes192Encryption" />, and <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes256Encryption" />.</exception>
        public override ICryptoTransform GetEncryptionTransform(
            string algorithm,
            byte[] iv)
        {
            return CryptoHelper.CreateEncryptor(this.symmetricKey, iv, algorithm);
        }

        /// <summary>Gets the size, in bits, of the initialization vector (<see langword="IV" />) that is required for the specified cryptographic algorithm.</summary>
        /// <param name="algorithm">The cryptographic algorithm to get the size of the initialization vector (<see langword="IV" />).</param>
        /// <returns>The size, in bits, of the initialization vector (<see langword="IV" />) that is required for the cryptographic algorithm specified in the <paramref name="algorithm" /> parameter.</returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// <paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.TripleDesEncryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes128Encryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes192Encryption" />, and <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes256Encryption" />.</exception>
        public override int GetIVSize(string algorithm)
        {
            return CryptoHelper.GetIVSize(algorithm);
        }

        /// <summary>Gets an instance of the specified keyed hash algorithm.</summary>
        /// <param name="algorithm">The keyed hash algorithm to get an instance of.</param>
        /// <returns>A <see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> that represents the keyed hash algorithm.</returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// <paramref name="algorithm" /> is not supported. The supported algorithms is <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.HmacSha1Signature" />.</exception>
        public override KeyedHashAlgorithm GetKeyedHashAlgorithm(string algorithm)
        {
            return CryptoHelper.CreateKeyedHashAlgorithm(this.symmetricKey, algorithm);
        }

        /// <summary>Gets an instance of the specified symmetric algorithm.</summary>
        /// <param name="algorithm">The symmetric algorithm to get an instance of.</param>
        /// <returns>A <see cref="T:System.Security.Cryptography.SymmetricAlgorithm" /> that represents the symmetric algorithm.</returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// <paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.TripleDesEncryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes128Encryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes192Encryption" />,  <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes256Encryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.TripleDesKeyWrap" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes128KeyWrap" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes192KeyWrap" />, and <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes256KeyWrap" /></exception>
        public override SymmetricAlgorithm GetSymmetricAlgorithm(string algorithm)
        {
            return CryptoHelper.GetSymmetricAlgorithm(this.symmetricKey, algorithm);
        }

        /// <summary>Gets the bytes that represent the symmetric key.</summary>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the symmetric key.</returns>
        public override byte[] GetSymmetricKey()
        {
            byte[] numArray = new byte[this.symmetricKey.Length];
            Buffer.BlockCopy((Array) this.symmetricKey, 0, (Array) numArray, 0, this.symmetricKey.Length);
            return numArray;
        }

        /// <summary>Gets a value that indicates whether the specified algorithm uses asymmetric keys.</summary>
        /// <param name="algorithm">The cryptographic algorithm.</param>
        /// <returns>
        /// <see langword="true" /> when the specified algorithm uses asymmetric keys; otherwise, <see langword="false" />.</returns>
        public override bool IsAsymmetricAlgorithm(string algorithm)
        {
            return CryptoHelper.IsAsymmetricAlgorithm(algorithm);
        }

        /// <summary>Gets a value that indicates whether the specified algorithm is supported by this class. </summary>
        /// <param name="algorithm">The cryptographic algorithm.</param>
        /// <returns>
        /// <see langword="true" /> when the specified algorithm is supported by this class; otherwise, <see langword="false" />.</returns>
        public override bool IsSupportedAlgorithm(string algorithm)
        {
            return CryptoHelper.IsSymmetricSupportedAlgorithm(algorithm, this.KeySize);
        }

        /// <summary>Gets a value that indicates whether the specified algorithm uses symmetric keys.</summary>
        /// <param name="algorithm">The cryptographic algorithm.</param>
        /// <returns>
        /// <see langword="true" /> when the specified algorithm uses symmetric keys; otherwise, <see langword="false" />.</returns>
        public override bool IsSymmetricAlgorithm(string algorithm)
        {
            return CryptoHelper.IsSymmetricAlgorithm(algorithm);
        }
    }
}