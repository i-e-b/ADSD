using System;
using System.Globalization;

namespace ADSD
{
    /// <summary>
    /// Creates <see cref="T:System.IdentityModel.Tokens.SignatureProvider" />s by specifying a <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> and algorithm.
    /// <para>Supports both <see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" /> and <see cref="T:System.IdentityModel.Tokens.SymmetricSecurityKey" />.</para>
    /// </summary>
    public class SignatureProviderFactory
    {
        /// <summary>
        /// This is the minimum <see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" />.KeySize when creating signatures.
        /// </summary>
        public static readonly int AbsoluteMinimumAsymmetricKeySizeInBitsForSigning = 2048;
        /// <summary>
        /// This is the minimum <see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" />.KeySize when verifying signatures.
        /// </summary>
        public static readonly int AbsoluteMinimumAsymmetricKeySizeInBitsForVerifying = 1024;
        /// <summary>
        /// This is the minimum <see cref="T:System.IdentityModel.Tokens.SymmetricSecurityKey" />.KeySize when creating and verifying signatures.
        /// </summary>
        public static readonly int AbsoluteMinimumSymmetricKeySizeInBits = 128;
        private static int minimumAsymmetricKeySizeInBitsForSigning = SignatureProviderFactory.AbsoluteMinimumAsymmetricKeySizeInBitsForSigning;
        private static int minimumAsymmetricKeySizeInBitsForVerifying = SignatureProviderFactory.AbsoluteMinimumAsymmetricKeySizeInBitsForVerifying;
        private static int minimumSymmetricKeySizeInBits = SignatureProviderFactory.AbsoluteMinimumSymmetricKeySizeInBits;

        /// <summary>
        /// Gets or sets the minimum <see cref="T:System.IdentityModel.Tokens.SymmetricSecurityKey" />.KeySize"/&gt;.
        /// </summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">'value' is smaller than <see cref="F:System.IdentityModel.Tokens.SignatureProviderFactory.AbsoluteMinimumSymmetricKeySizeInBits" />.</exception>
        public static int MinimumSymmetricKeySizeInBits
        {
            get
            {
                return SignatureProviderFactory.minimumSymmetricKeySizeInBits;
            }
            set
            {
                if (value < SignatureProviderFactory.AbsoluteMinimumSymmetricKeySizeInBits)
                    throw new ArgumentOutOfRangeException(nameof (value), (object) value, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10628: Cannot set the MinimumSymmetricKeySizeInBits to less than: '{0}'.", (object) SignatureProviderFactory.AbsoluteMinimumSymmetricKeySizeInBits));
                SignatureProviderFactory.minimumSymmetricKeySizeInBits = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum <see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" />.KeySize for creating signatures.
        /// </summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">'value' is smaller than <see cref="F:System.IdentityModel.Tokens.SignatureProviderFactory.AbsoluteMinimumAsymmetricKeySizeInBitsForSigning" />.</exception>
        public static int MinimumAsymmetricKeySizeInBitsForSigning
        {
            get
            {
                return SignatureProviderFactory.minimumAsymmetricKeySizeInBitsForSigning;
            }
            set
            {
                if (value < SignatureProviderFactory.AbsoluteMinimumAsymmetricKeySizeInBitsForSigning)
                    throw new ArgumentOutOfRangeException(nameof (value), (object) value, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10613: Cannot set the MinimumAsymmetricKeySizeInBitsForSigning to less than: '{0}'.", (object) SignatureProviderFactory.AbsoluteMinimumAsymmetricKeySizeInBitsForSigning));
                SignatureProviderFactory.minimumAsymmetricKeySizeInBitsForSigning = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum <see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" />.KeySize for verifying signatures.
        /// <exception cref="T:System.ArgumentOutOfRangeException">'value' is smaller than <see cref="F:System.IdentityModel.Tokens.SignatureProviderFactory.AbsoluteMinimumAsymmetricKeySizeInBitsForVerifying" />.</exception>
        /// </summary>
        public static int MinimumAsymmetricKeySizeInBitsForVerifying
        {
            get
            {
                return SignatureProviderFactory.minimumAsymmetricKeySizeInBitsForVerifying;
            }
            set
            {
                if (value < SignatureProviderFactory.AbsoluteMinimumAsymmetricKeySizeInBitsForVerifying)
                    throw new ArgumentOutOfRangeException(nameof (value), (object) value, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10627: Cannot set the MinimumAsymmetricKeySizeInBitsForVerifying to less than: '{0}'.", (object) SignatureProviderFactory.AbsoluteMinimumAsymmetricKeySizeInBitsForVerifying));
                SignatureProviderFactory.minimumAsymmetricKeySizeInBitsForVerifying = value;
            }
        }

        /// <summary>
        /// Creates a <see cref="T:System.IdentityModel.Tokens.SignatureProvider" /> that supports the <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> and algorithm.
        /// </summary>
        /// <param name="key">
        /// The <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> to use for signing.
        /// </param>
        /// <param name="algorithm">The algorithm to use for signing.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 'key' is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// 'algorithm' is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 'algorithm' contains only whitespace.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// '<see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" />' is smaller than <see cref="P:System.IdentityModel.Tokens.SignatureProviderFactory.MinimumAsymmetricKeySizeInBitsForSigning" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// '<see cref="T:System.IdentityModel.Tokens.SymmetricSecurityKey" />' is smaller than <see cref="P:System.IdentityModel.Tokens.SignatureProviderFactory.MinimumSymmetricKeySizeInBits" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// '<see cref="T:System.IdentityModel.Tokens.SecurityKey" />' is not a <see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" /> or a <see cref="T:System.IdentityModel.Tokens.SymmetricSecurityKey" />.
        /// </exception>
        /// <remarks>
        /// AsymmetricSignatureProviders require access to a PrivateKey for Signing.
        /// </remarks>
        /// <returns>
        /// The <see cref="T:System.IdentityModel.Tokens.SignatureProvider" />.
        /// </returns>
        public virtual SignatureProvider CreateForSigning(
            SecurityKey key,
            string algorithm)
        {
            return SignatureProviderFactory.CreateProvider(key, algorithm, true);
        }

        /// <summary>
        /// Returns a <see cref="T:System.IdentityModel.Tokens.SignatureProvider" /> instance supports the <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> and algorithm.
        /// </summary>
        /// <param name="key">
        /// The <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> to use for signing.
        /// </param>
        /// <param name="algorithm">The algorithm to use for signing.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 'key' is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// 'algorithm' is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 'algorithm' contains only whitespace.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// '<see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" />' is smaller than <see cref="P:System.IdentityModel.Tokens.SignatureProviderFactory.MinimumAsymmetricKeySizeInBitsForVerifying" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// '<see cref="T:System.IdentityModel.Tokens.SymmetricSecurityKey" />' is smaller than <see cref="P:System.IdentityModel.Tokens.SignatureProviderFactory.MinimumSymmetricKeySizeInBits" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// '<see cref="T:System.IdentityModel.Tokens.SecurityKey" />' is not a <see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" /> or a <see cref="T:System.IdentityModel.Tokens.SymmetricSecurityKey" />.
        /// </exception>
        /// <returns>
        /// The <see cref="T:System.IdentityModel.Tokens.SignatureProvider" />.
        /// </returns>
        public virtual SignatureProvider CreateForVerifying(
            SecurityKey key,
            string algorithm)
        {
            return SignatureProviderFactory.CreateProvider(key, algorithm, false);
        }

        /// <summary>
        /// When finished with a <see cref="T:System.IdentityModel.Tokens.SignatureProvider" /> call this method for cleanup. The default behavior is to call <see cref="M:System.IdentityModel.Tokens.SignatureProvider.Dispose(System.Boolean)" />
        /// </summary>
        /// <param name="signatureProvider"><see cref="T:System.IdentityModel.Tokens.SignatureProvider" /> to be released.</param>
        public virtual void ReleaseProvider(SignatureProvider signatureProvider)
        {
            signatureProvider?.Dispose();
        }

        private static SignatureProvider CreateProvider(
            SecurityKey key,
            string algorithm,
            bool willCreateSignatures)
        {
            if (key == null)
                throw new ArgumentNullException(nameof (key));
            if (algorithm == null)
                throw new ArgumentNullException(nameof (algorithm));
            if (string.IsNullOrWhiteSpace(algorithm))
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10002: The parameter '{0}' cannot be 'null' or a string containing only whitespace.", (object) "algorithm "));
            AsymmetricSecurityKey key1 = key as AsymmetricSecurityKey;
            if (key1 != null)
            {
                if (willCreateSignatures && key1.KeySize < SignatureProviderFactory.MinimumAsymmetricKeySizeInBitsForSigning)
                    throw new ArgumentOutOfRangeException("key.KeySize", (object) key1.KeySize, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10630: The '{0}' for signing cannot be smaller than '{1}' bits.", (object) key.GetType(), (object) SignatureProviderFactory.MinimumAsymmetricKeySizeInBitsForSigning));
                if (key1.KeySize < SignatureProviderFactory.MinimumAsymmetricKeySizeInBitsForVerifying)
                    throw new ArgumentOutOfRangeException("key.KeySize", (object) key1.KeySize, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10631: The '{0}' for verifying cannot be smaller than '{1}' bits.", (object) key.GetType(), (object) SignatureProviderFactory.MinimumAsymmetricKeySizeInBitsForVerifying));
                return (SignatureProvider) new AsymmetricSignatureProvider(key1, algorithm, willCreateSignatures);
            }
            SymmetricSecurityKey key2 = key as SymmetricSecurityKey;
            if (key2 != null)
            {
                if (key2.KeySize < SignatureProviderFactory.MinimumSymmetricKeySizeInBits)
                    throw new ArgumentOutOfRangeException("key.KeySize", (object) key.KeySize, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10603: The '{0}' cannot have less than: '{1}' bits.", (object) key.GetType(), (object) SignatureProviderFactory.MinimumSymmetricKeySizeInBits));
                return (SignatureProvider) new SymmetricSignatureProvider(key2, algorithm);
            }
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10600: '{0}' supports: '{1}' of types: '{2}' or '{3}'. SecurityKey received was of type: '{4}'.", (object) typeof (SignatureProvider).ToString(), (object) typeof (SecurityKey), (object) typeof (AsymmetricSecurityKey), (object) typeof (SymmetricSecurityKey), (object) key.GetType()));
        }
    }
}