using System;
using System.Globalization;
using System.Security.Cryptography;

namespace ADSD.Crypto
{
    /// <summary>
    /// Provides signing and verifying operations when working with an <see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" />
    /// </summary>
    public class AsymmetricSignatureProvider : SignatureProvider
    {
        private bool disposed;
        private HashAlgorithm hash;
        private readonly AsymmetricSignatureFormatter formatter;
        private readonly AsymmetricSignatureDeformatter deformatter;
        private readonly AsymmetricSecurityKey key;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.AsymmetricSignatureProvider" /> class used to create and verify signatures.
        /// </summary>
        /// <param name="key">
        /// The <see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" /> that will be used for cryptographic operations.
        /// </param>
        /// <param name="algorithm">The signature algorithm to apply.</param>
        /// <param name="willCreateSignatures">
        /// If this <see cref="T:System.IdentityModel.Tokens.AsymmetricSignatureProvider" /> is required to create signatures then set this to true.
        /// <para>
        /// Creating signatures requires that the <see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" /> has access to a private key.
        /// Verifying signatures (the default), does not require access to the private key.
        /// </para>
        /// </param>
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
        /// willCreateSignatures is true and <see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" />.KeySize is less than <see cref="P:System.IdentityModel.Tokens.SignatureProviderFactory.MinimumAsymmetricKeySizeInBitsForSigning" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" />.KeySize is less than <see cref="P:System.IdentityModel.Tokens.SignatureProviderFactory.MinimumAsymmetricKeySizeInBitsForVerifying" />. Note: this is always checked.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// Is thrown if the <see cref="M:System.IdentityModel.Tokens.AsymmetricSecurityKey.GetHashAlgorithmForSignature(System.String)" /> throws.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// Is thrown if the <see cref="M:System.IdentityModel.Tokens.AsymmetricSecurityKey.GetHashAlgorithmForSignature(System.String)" /> returns null.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// Is thrown if the <see cref="M:System.IdentityModel.Tokens.AsymmetricSecurityKey.GetSignatureFormatter(System.String)" /> throws.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// Is thrown if the <see cref="M:System.IdentityModel.Tokens.AsymmetricSecurityKey.GetSignatureFormatter(System.String)" /> returns null.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// Is thrown if the <see cref="M:System.IdentityModel.Tokens.AsymmetricSecurityKey.GetSignatureDeformatter(System.String)" /> throws.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// Is thrown if the <see cref="M:System.IdentityModel.Tokens.AsymmetricSecurityKey.GetSignatureDeformatter(System.String)" /> returns null.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// Is thrown if the <see cref="M:System.Security.Cryptography.AsymmetricSignatureFormatter.SetHashAlgorithm(System.String)" /> throws.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// Is thrown if the <see cref="M:System.Security.Cryptography.AsymmetricSignatureDeformatter.SetHashAlgorithm(System.String)" /> throws.
        /// </exception>
        public AsymmetricSignatureProvider(
            AsymmetricSecurityKey key,
            string algorithm,
            bool willCreateSignatures = false)
        {
            if (key == null)
                throw new ArgumentNullException(nameof (key));
            if (algorithm == null)
                throw new ArgumentNullException(nameof (algorithm));
            if (string.IsNullOrWhiteSpace(algorithm))
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10002: The parameter '{0}' cannot be 'null' or a string containing only whitespace.", (object) nameof (algorithm)));
            if (willCreateSignatures && key.KeySize < SignatureProviderFactory.MinimumAsymmetricKeySizeInBitsForSigning)
                throw new ArgumentOutOfRangeException("key.KeySize", (object) key.KeySize, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10631: The '{0}' for verifying cannot be smaller than '{1}' bits.", (object) key.GetType(), (object) SignatureProviderFactory.MinimumAsymmetricKeySizeInBitsForSigning));
            if (key.KeySize < SignatureProviderFactory.MinimumAsymmetricKeySizeInBitsForVerifying)
                throw new ArgumentOutOfRangeException("key.KeySize", (object) key.KeySize, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10630: The '{0}' for signing cannot be smaller than '{1}' bits.", (object) key.GetType(), (object) SignatureProviderFactory.MinimumAsymmetricKeySizeInBitsForVerifying));
            this.key = key;
            try
            {
                hash = this.key.GetHashAlgorithmForSignature(algorithm);
            }
            catch (Exception ex)
            {
                    throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10618: AsymmetricSecurityKey.GetHashAlgorithmForSignature( '{0}' ) threw an exception.\nAsymmetricSecurityKey: '{1}'\nSignatureAlgorithm: '{0}', check to make sure the SignatureAlgorithm is supported.\nException: '{2}'.", (object) algorithm, (object) this.key.ToString(), (object) ex), ex);
            }
            if (hash == null)
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10611: AsymmetricSecurityKey.GetHashAlgorithmForSignature( '{0}' ) returned null.\nKey: '{1}'\nSignatureAlgorithm: '{0}'", (object) algorithm, (object) this.key.ToString()));
            if (willCreateSignatures)
            {
                try
                {
                    formatter = this.key.GetSignatureFormatter(algorithm);
                    formatter.SetHashAlgorithm(hash.GetType().ToString());
                }
                catch (Exception ex)
                {
                        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10614: AsymmetricSecurityKey.GetSignatureFormater( '{0}' ) threw an exception.\nKey: '{1}'\nSignatureAlgorithm: '{0}', check to make sure the SignatureAlgorithm is supported.\nException:'{2}'.\nIf you only need to verify signatures the parameter 'willBeUseForSigning' should be false if the private key is not be available.", (object) algorithm, (object) this.key.ToString(), (object) ex), ex);
                }
                if (formatter == null)
                    throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10615: AsymmetricSecurityKey.GetSignatureFormater( '{0}' ) returned null.\nKey: '{1}'\nSignatureAlgorithm: '{0}', check to make sure the SignatureAlgorithm is supported.", (object) algorithm, (object) this.key.ToString()));
            }
            try
            {
                deformatter = this.key.GetSignatureDeformatter(algorithm);
                deformatter.SetHashAlgorithm(hash.GetType().ToString());
            }
            catch (Exception ex)
            {
                    throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10616: AsymmetricSecurityKey.GetSignatureDeformatter( '{0}' ) threw an exception.\nKey: '{1}'\nSignatureAlgorithm: '{0}, check to make sure the SignatureAlgorithm is supported.'\nException:'{2}'.", (object) algorithm, (object) this.key.ToString(), (object) ex), ex);
            }
            if (deformatter == null)
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10617: AsymmetricSecurityKey.GetSignatureDeFormater( '{0}' ) returned null.\nKey: '{1}'\nSignatureAlgorithm: '{0}', check to make sure the SignatureAlgorithm is supported.", (object) algorithm, (object) this.key.ToString()));
        }

        /// <summary>
        /// Produces a signature over the 'input' using the <see cref="T:System.IdentityModel.Tokens.AsymmetricSecurityKey" /> and algorithm passed to <see cref="M:System.IdentityModel.Tokens.AsymmetricSignatureProvider.#ctor(System.IdentityModel.Tokens.AsymmetricSecurityKey,System.String,System.Boolean)" />.
        /// </summary>
        /// <param name="input">bytes to be signed.</param>
        /// <returns>a signature over the input.</returns>
        /// <exception cref="T:System.ArgumentNullException">'input' is null. </exception>
        /// <exception cref="T:System.ArgumentException">'input.Length' == 0. </exception>
        /// <exception cref="T:System.ObjectDisposedException">if <see cref="M:System.IdentityModel.Tokens.AsymmetricSignatureProvider.Dispose(System.Boolean)" /> has been called. </exception>
        /// <exception cref="T:System.InvalidOperationException">if the internal <see cref="T:System.Security.Cryptography.AsymmetricSignatureFormatter" /> is null. This can occur if the constructor parameter 'willBeUsedforSigning' was not 'true'.</exception>
        /// <exception cref="T:System.InvalidOperationException">if the internal <see cref="T:System.Security.Cryptography.HashAlgorithm" /> is null. This can occur if a derived type deletes it or does not create it.</exception>
        public override byte[] Sign(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof (input));
            if (input.Length == 0)
                throw new ArgumentException("IDX10624: Cannot sign 'input' byte array has length 0.");
            if (disposed)
                throw new ObjectDisposedException(GetType().ToString());
            if (formatter == null)
                throw new InvalidOperationException("IDX10620: The AsymmetricSignatureFormatter is null, cannot sign data.  Was this AsymmetricSignatureProvider constructor called specifying setting parameter: 'willCreateSignatures' == 'true'?.");
            if (hash == null)
                throw new InvalidOperationException("IDX10621: This AsymmetricSignatureProvider has a minimum key size requirement of: '{0}', the AsymmetricSecurityKey in has a KeySize of: '{1}'.");
            return formatter.CreateSignature(hash.ComputeHash(input));
        }

        /// <summary>
        /// Verifies that a signature over the' input' matches the signature.
        /// </summary>
        /// <param name="input">the bytes to generate the signature over.</param>
        /// <param name="signature">the value to verify against.</param>
        /// <returns>true if signature matches, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException">'input' is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">'signature' is null.</exception>
        /// <exception cref="T:System.ArgumentException">'input.Length' == 0.</exception>
        /// <exception cref="T:System.ArgumentException">'signature.Length' == 0.</exception>
        /// <exception cref="T:System.ObjectDisposedException">if <see cref="M:System.IdentityModel.Tokens.AsymmetricSignatureProvider.Dispose(System.Boolean)" /> has been called. </exception>
        /// <exception cref="T:System.InvalidOperationException">if the internal <see cref="T:System.Security.Cryptography.AsymmetricSignatureDeformatter" /> is null. This can occur if a derived type does not call the base constructor.</exception>
        /// <exception cref="T:System.InvalidOperationException">if the internal <see cref="T:System.Security.Cryptography.HashAlgorithm" /> is null. This can occur if a derived type deletes it or does not create it.</exception>
        public override bool Verify(byte[] input, byte[] signature)
        {
            if (input == null)
                throw new ArgumentNullException(nameof (input));
            if (signature == null)
                throw new ArgumentNullException(nameof (signature));
            if (input.Length == 0)
                throw new ArgumentException("IDX10625: Cannot verify signature 'input' byte array has length 0.");
            if (signature.Length == 0)
                throw new ArgumentException("IDX10626: Cannot verify signature 'signature' byte array has length 0.");
            if (disposed)
                throw new ObjectDisposedException(GetType().ToString());
            if (deformatter == null)
                throw new InvalidOperationException("IDX10629: The AsymmetricSignatureDeformatter is null, cannot sign data. If a derived AsymmetricSignatureProvider is being used, make sure to call the base constructor.");
            if (hash == null)
                throw new InvalidOperationException("IDX10621: This AsymmetricSignatureProvider has a minimum key size requirement of: '{0}', the AsymmetricSecurityKey in has a KeySize of: '{1}'.");

            return deformatter.VerifySignature(hash.ComputeHash(input), signature);
        }

        /// <summary>
        /// Calls <see cref="M:System.Security.Cryptography.HashAlgorithm.Dispose" /> to release this managed resources.
        /// </summary>
        /// <param name="disposing">true, if called from Dispose(), false, if invoked inside a finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposed || !disposing)
                return;
            disposed = true;
            if (hash == null)
                return;
            hash.Dispose();
            hash = (HashAlgorithm) null;
        }
    }
}