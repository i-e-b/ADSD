using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace ADSD
{
    /// <summary>
    /// Provides signing and verifying operations using a <see cref="T:System.IdentityModel.Tokens.SymmetricSecurityKey" /> and specifying an algorithm.
    /// </summary>
    public class SymmetricSignatureProvider : SignatureProvider
    {
        private static byte[] bytesA = new byte[32]
        {
            (byte) 0,
            (byte) 1,
            (byte) 2,
            (byte) 3,
            (byte) 4,
            (byte) 5,
            (byte) 6,
            (byte) 7,
            (byte) 8,
            (byte) 9,
            (byte) 10,
            (byte) 11,
            (byte) 12,
            (byte) 13,
            (byte) 14,
            (byte) 15,
            (byte) 16,
            (byte) 17,
            (byte) 18,
            (byte) 19,
            (byte) 20,
            (byte) 21,
            (byte) 22,
            (byte) 23,
            (byte) 24,
            (byte) 25,
            (byte) 26,
            (byte) 27,
            (byte) 28,
            (byte) 29,
            (byte) 30,
            (byte) 31
        };
        private static byte[] bytesB = new byte[32]
        {
            (byte) 31,
            (byte) 30,
            (byte) 29,
            (byte) 28,
            (byte) 27,
            (byte) 26,
            (byte) 25,
            (byte) 24,
            (byte) 23,
            (byte) 22,
            (byte) 21,
            (byte) 20,
            (byte) 19,
            (byte) 18,
            (byte) 17,
            (byte) 16,
            (byte) 15,
            (byte) 14,
            (byte) 13,
            (byte) 12,
            (byte) 11,
            (byte) 10,
            (byte) 9,
            (byte) 8,
            (byte) 7,
            (byte) 6,
            (byte) 5,
            (byte) 4,
            (byte) 3,
            (byte) 2,
            (byte) 1,
            (byte) 0
        };
        private bool disposed;
        private KeyedHashAlgorithm keyedHash;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.SymmetricSignatureProvider" /> class that uses an <see cref="T:System.IdentityModel.Tokens.SymmetricSecurityKey" /> to create and / or verify signatures over a array of bytes.
        /// </summary>
        /// <param name="key">The <see cref="T:System.IdentityModel.Tokens.SymmetricSecurityKey" /> used for signing.</param>
        /// <param name="algorithm">The signature algorithm to use.</param>
        /// <exception cref="T:System.ArgumentNullException">'key' is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">'algorithm' is null.</exception>
        /// <exception cref="T:System.ArgumentException">'algorithm' contains only whitespace.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">'<see cref="T:System.IdentityModel.Tokens.SymmetricSecurityKey" />.KeySize' is smaller than <see cref="P:System.IdentityModel.Tokens.SignatureProviderFactory.MinimumSymmetricKeySizeInBits" />.</exception>
        /// <exception cref="T:System.InvalidOperationException"><see cref="M:System.IdentityModel.Tokens.SymmetricSecurityKey.GetKeyedHashAlgorithm(System.String)" /> throws.</exception>
        /// <exception cref="T:System.InvalidOperationException"><see cref="M:System.IdentityModel.Tokens.SymmetricSecurityKey.GetKeyedHashAlgorithm(System.String)" /> returns null.</exception>
        /// <exception cref="T:System.InvalidOperationException"><see cref="M:System.IdentityModel.Tokens.SymmetricSecurityKey.GetSymmetricKey" /> throws.</exception>
        public SymmetricSignatureProvider(SymmetricSecurityKey key, string algorithm)
        {
            if (key == null)
                throw new ArgumentNullException(nameof (key));
            if (algorithm == null)
                throw new ArgumentNullException(algorithm);
            if (string.IsNullOrWhiteSpace(algorithm))
                throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10002: The parameter '{0}' cannot be 'null' or a string containing only whitespace.", (object) nameof (algorithm)));
            if (key.KeySize < SignatureProviderFactory.MinimumSymmetricKeySizeInBits)
                throw new ArgumentOutOfRangeException("key.KeySize", (object) key.KeySize, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10603: The '{0}' cannot have less than: '{1}' bits.", (object) key.GetType(), (object) SignatureProviderFactory.MinimumSymmetricKeySizeInBits));
            try
            {
                this.keyedHash = key.GetKeyedHashAlgorithm(algorithm);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10632: SymmetricSecurityKey.GetKeyedHashAlgorithm( '{0}' ) threw an exception.\nSymmetricSecurityKey: '{1}'\nSignatureAlgorithm: '{0}', check to make sure the SignatureAlgorithm is supported.\nException: '{2}'.", (object) algorithm, (object) key, (object) ex), ex);
            }
            if (this.keyedHash == null)
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10633: SymmetricSecurityKey.GetKeyedHashAlgorithm( '{0}' ) returned null.\n\nSymmetricSecurityKey: '{1}'\nSignatureAlgorithm: '{0}', check to make sure the SignatureAlgorithm is supported.", (object) algorithm, (object) key));
            try
            {
                this.keyedHash.Key = key.GetSymmetricKey();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IDX10634: KeyedHashAlgorithm.Key = SymmetricSecurityKey.GetSymmetricKey() threw.\n\nSymmetricSecurityKey: '{1}'\nSignatureAlgorithm: '{0}' check to make sure the SignatureAlgorithm is supported.\nException: '{2}'.", (object) algorithm, (object) key, (object) ex), ex);
            }
        }

        /// <summary>
        /// Produces a signature over the 'input' using the <see cref="T:System.IdentityModel.Tokens.SymmetricSecurityKey" /> and 'algorithm' passed to <see cref="M:System.IdentityModel.Tokens.SymmetricSignatureProvider.#ctor(System.IdentityModel.Tokens.SymmetricSecurityKey,System.String)" />.
        /// </summary>
        /// <param name="input">bytes to sign.</param>
        /// <returns>signed bytes</returns>
        /// <exception cref="T:System.ArgumentNullException">'input' is null. </exception>
        /// <exception cref="T:System.ArgumentException">'input.Length' == 0. </exception>
        /// <exception cref="T:System.ObjectDisposedException"><see cref="M:System.IdentityModel.Tokens.SymmetricSignatureProvider.Dispose(System.Boolean)" /> has been called.</exception>
        /// <exception cref="T:System.InvalidOperationException"><see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> is null. This can occur if a derived type deletes it or does not create it.</exception>
        public override byte[] Sign(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof (input));
            if (input.Length == 0)
                throw new ArgumentException("IDX10624: Cannot sign 'input' byte array has length 0.");
            if (this.disposed)
                throw new ObjectDisposedException(typeof (SymmetricSignatureProvider).ToString());
            if (this.keyedHash == null)
                throw new InvalidOperationException("IDX10623: The KeyedHashAlgorithm is null, cannot sign data.");
            return this.keyedHash.ComputeHash(input);
        }

        /// <summary>
        /// Verifies that a signature created over the 'input' matches the signature. Using <see cref="T:System.IdentityModel.Tokens.SymmetricSecurityKey" /> and 'algorithm' passed to <see cref="M:System.IdentityModel.Tokens.SymmetricSignatureProvider.#ctor(System.IdentityModel.Tokens.SymmetricSecurityKey,System.String)" />.
        /// </summary>
        /// <param name="input">bytes to verify.</param>
        /// <param name="signature">signature to compare against.</param>
        /// <returns>true if computed signature matches the signature parameter, false otherwise.</returns>
        /// <exception cref="T:System.ArgumentNullException">'input' is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">'signature' is null.</exception>
        /// <exception cref="T:System.ArgumentException">'input.Length' == 0.</exception>
        /// <exception cref="T:System.ArgumentException">'signature.Length' == 0. </exception>
        /// <exception cref="T:System.ObjectDisposedException"><see cref="M:System.IdentityModel.Tokens.SymmetricSignatureProvider.Dispose(System.Boolean)" /> has been called.</exception>
        /// <exception cref="T:System.InvalidOperationException">if the internal <see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> is null. This can occur if a derived type deletes it or does not create it.</exception>
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
            if (this.disposed)
                throw new ObjectDisposedException(typeof (SymmetricSignatureProvider).ToString());
            if (this.keyedHash == null)
                throw new InvalidOperationException("IDX10623: The KeyedHashAlgorithm is null, cannot sign data.");
            return SymmetricSignatureProvider.AreEqual(signature, this.keyedHash.ComputeHash(input));
        }

        /// <summary>Disposes of internal components.</summary>
        /// <param name="disposing">true, if called from Dispose(), false, if invoked inside a finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            this.disposed = true;
            if (!disposing || this.keyedHash == null)
                return;
            this.keyedHash.Dispose();
            this.keyedHash = (KeyedHashAlgorithm) null;
        }

        /// <summary>
        /// Compares two byte arrays for equality. Hash size is fixed normally it is 32 bytes.
        /// The attempt here is to take the same time if an attacker shortens the signature OR changes some of the signed contents.
        /// </summary>
        /// <param name="a">One set of bytes to compare.</param>
        /// <param name="b">The other set of bytes to compare with.</param>
        /// <returns>true if the bytes are equal, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool AreEqual(byte[] a, byte[] b)
        {
            int num = 0;
            byte[] numArray1;
            byte[] numArray2;
            if (a == null || b == null || a.Length != b.Length)
            {
                numArray1 = SymmetricSignatureProvider.bytesA;
                numArray2 = SymmetricSignatureProvider.bytesB;
            }
            else
            {
                numArray1 = a;
                numArray2 = b;
            }
            for (int index = 0; index < numArray1.Length; ++index)
                num |= (int) numArray1[index] ^ (int) numArray2[index];
            return num == 0;
        }
    }
}