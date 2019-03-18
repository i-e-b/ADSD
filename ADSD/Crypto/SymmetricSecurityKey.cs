using System.Security.Cryptography;

namespace ADSD.Crypto
{
    /// <summary>Represents the abstract base class for all keys that are generated using symmetric algorithms.</summary>
    public abstract class SymmetricSecurityKey : SecurityKey
    {
        /// <summary>When overridden in a derived class, generates a derived key using the specified cryptographic algorithm and parameters for the current key. </summary>
        /// <param name="algorithm">A URI that represents the cryptographic algorithm to use to generate the derived key.</param>
        /// <param name="label">An array of <see cref="T:System.Byte" /> that contains the label parameter for the cryptographic algorithm.</param>
        /// <param name="nonce">An array of <see cref="T:System.Byte" /> that contains the nonce that is used to create a derived key.</param>
        /// <param name="derivedKeyLength">The size of the derived key.</param>
        /// <param name="offset">The position at which the derived key is located in the byte array that is returned from this method.</param>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the derived key.</returns>
        public abstract byte[] GenerateDerivedKey(
            string algorithm,
            byte[] label,
            byte[] nonce,
            int derivedKeyLength,
            int offset);

        /// <summary>When overridden in a derived class, gets a transform that decrypts cipher text using the specified cryptographic algorithm. </summary>
        /// <param name="algorithm">A cryptographic algorithm that decrypts cipher text, such as encrypted XML.</param>
        /// <param name="iv">An array of <see cref="T:System.Byte" /> that contains the initialization vector (<see langword="IV" />) for the specified algorithm.</param>
        /// <returns>An <see cref="T:System.Security.Cryptography.ICryptoTransform" /> that represents the decryption transform.</returns>
        public abstract ICryptoTransform GetDecryptionTransform(
            string algorithm,
            byte[] iv);

        /// <summary>When overridden in a derived class, gets a transform that encrypts XML using the specified cryptographic algorithm. </summary>
        /// <param name="algorithm">A cryptographic algorithm that encrypts XML.</param>
        /// <param name="iv">An array of <see cref="T:System.Byte" /> that contains the initialization vector (<see langword="IV" />) for the specified algorithm.</param>
        /// <returns>An <see cref="T:System.Security.Cryptography.ICryptoTransform" /> that represents the encryption transform.</returns>
        public abstract ICryptoTransform GetEncryptionTransform(
            string algorithm,
            byte[] iv);

        /// <summary>When overridden in a derived class, gets the size, in bits, of the initialization vector (<see langword="IV" />) that is required for the specified cryptographic algorithm. </summary>
        /// <param name="algorithm">The cryptographic algorithm to get the size of the initialization vector (<see langword="IV" />).</param>
        /// <returns>The size, in bits, of the initialization vector (<see langword="IV" />) that is required for the cryptographic algorithm specified in the <paramref name="algorithm" /> parameter.</returns>
        public abstract int GetIVSize(string algorithm);

        /// <summary>When overridden in a derived class, gets an instance of the specified keyed hash algorithm.</summary>
        /// <param name="algorithm">The keyed hash algorithm to get an instance of.</param>
        /// <returns>A <see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> that represents the keyed hash algorithm.</returns>
        public abstract KeyedHashAlgorithm GetKeyedHashAlgorithm(string algorithm);

        /// <summary>When overridden in a derived class, gets an instance of the specified symmetric algorithm.</summary>
        /// <param name="algorithm">The symmetric algorithm to get an instance of.</param>
        /// <returns>A <see cref="T:System.Security.Cryptography.SymmetricAlgorithm" /> that represents the symmetric algorithm.</returns>
        public abstract SymmetricAlgorithm GetSymmetricAlgorithm(string algorithm);

        /// <summary>When overridden in a derived class, gets the bytes that represent the symmetric key.</summary>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the symmetric key.</returns>
        public abstract byte[] GetSymmetricKey();
    }
}