namespace ADSD.Crypto
{
    /// <summary>Base class for security keys.</summary>
    public abstract class SecurityKey
    {
        /// <summary>When overridden in a derived class, gets the size, in bits, of the key.</summary>
        /// <returns>The size, in bits, of the key.</returns>
        public abstract int KeySize { get; }

        /// <summary>When overridden in a derived class, decrypts the specified encrypted key.</summary>
        /// <param name="algorithm">The cryptographic algorithm that was used to encrypt the key.</param>
        /// <param name="keyData">An array of <see cref="T:System.Byte" /> that contains the encrypted key.</param>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the decrypted key.</returns>
        public abstract byte[] DecryptKey(string algorithm, byte[] keyData);

        /// <summary>When overridden in a derived class, encrypts the specified key.</summary>
        /// <param name="algorithm">The cryptographic algorithm to encrypt the key with.</param>
        /// <param name="keyData">An array of <see cref="T:System.Byte" /> that contains the key.</param>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the encrypted key.</returns>
        public abstract byte[] EncryptKey(string algorithm, byte[] keyData);

        /// <summary>When overridden in a derived class, gets a value that indicates whether the specified algorithm uses asymmetric keys. </summary>
        /// <param name="algorithm">The cryptographic algorithm.</param>
        /// <returns>
        /// <see langword="true" /> when the specified algorithm uses asymmetric keys; otherwise, <see langword="false" />. </returns>
        public abstract bool IsAsymmetricAlgorithm(string algorithm);

        /// <summary>When overridden in a derived class, gets a value that indicates whether the specified algorithm is supported by this class. </summary>
        /// <param name="algorithm">The cryptographic algorithm.</param>
        /// <returns>
        /// <see langword="true" /> when the specified algorithm is supported by this class; otherwise, <see langword="false" />.</returns>
        public abstract bool IsSupportedAlgorithm(string algorithm);

        /// <summary>When overridden in a derived class, gets a value that indicates whether the specified algorithm uses symmetric keys.</summary>
        /// <param name="algorithm">The cryptographic algorithm.</param>
        /// <returns>
        /// <see langword="true" /> when the specified algorithm uses symmetric keys; otherwise, <see langword="false" />.</returns>
        public abstract bool IsSymmetricAlgorithm(string algorithm);
    }
}