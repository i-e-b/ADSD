using System.Security.Cryptography;

namespace ADSD
{
    /// <summary>Base class for asymmetric keys.</summary>
    public abstract class AsymmetricSecurityKey : SecurityKey
    {
        /// <summary>When overridden in a derived class, gets the specified asymmetric cryptographic algorithm. </summary>
        /// <param name="algorithm">The asymmetric algorithm to create.</param>
        /// <param name="privateKey">
        /// <see langword="true" /> when a private key is required to create the algorithm; otherwise, <see langword="false" />. </param>
        /// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> that represents the specified asymmetric cryptographic algorithm.Typically, <see langword="true" /> is passed into the <paramref name="privateKey" /> parameter, as a private key is typically required for decryption.</returns>
        public abstract AsymmetricAlgorithm GetAsymmetricAlgorithm(
            string algorithm,
            bool privateKey);

        /// <summary>When overridden in a derived class, gets a cryptographic algorithm that generates a hash for a digital signature.</summary>
        /// <param name="algorithm">The hash algorithm.</param>
        /// <returns>A <see cref="T:System.Security.Cryptography.HashAlgorithm" /> that generates hashes for digital signatures.</returns>
        public abstract HashAlgorithm GetHashAlgorithmForSignature(string algorithm);

        /// <summary>When overridden in a derived class, gets the deformatter algorithm for the digital signature.</summary>
        /// <param name="algorithm">The deformatter algorithm for the digital signature.</param>
        /// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricSignatureDeformatter" /> that represents the deformatter algorithm for the digital signature.</returns>
        public abstract AsymmetricSignatureDeformatter GetSignatureDeformatter(
            string algorithm);

        /// <summary>When overridden in a derived class, gets the formatter algorithm for the digital signature.</summary>
        /// <param name="algorithm">The formatter algorithm for the digital signature.</param>
        /// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricSignatureFormatter" /> that represents the formatter algorithm for the digital signature.</returns>
        public abstract AsymmetricSignatureFormatter GetSignatureFormatter(
            string algorithm);

        /// <summary>When overridden in a derived class, gets a value that indicates whether the private key is available.</summary>
        /// <returns>
        /// <see langword="true" /> when the private key is available; otherwise, <see langword="false" />. </returns>
        public abstract bool HasPrivateKey();
    }
}