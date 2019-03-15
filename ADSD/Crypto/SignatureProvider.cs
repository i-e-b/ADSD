using System;

namespace ADSD
{
    /// <summary>
    /// This class defines the object model for types that provide signature services.
    /// </summary>
    public abstract class SignatureProvider : IDisposable
    {
        /// <summary>
        /// Gets or sets a user context for a <see cref="T:System.IdentityModel.Tokens.SignatureProvider" />.
        /// </summary>
        public string Context { get; set; }

        /// <summary>Produces a signature over the 'input'</summary>
        /// <param name="input">bytes to sign.</param>
        /// <returns>signed bytes</returns>
        public abstract byte[] Sign(byte[] input);

        /// <summary>
        /// Verifies that a signature created over the 'input' matches the signature.
        /// </summary>
        /// <param name="input">bytes to verify.</param>
        /// <param name="signature">signature to compare against.</param>
        /// <returns>true if the computed signature matches the signature parameter, false otherwise.</returns>
        public abstract bool Verify(byte[] input, byte[] signature);

        /// <summary>
        /// Calls <see cref="M:System.IdentityModel.Tokens.SignatureProvider.Dispose(System.Boolean)" /> and <see cref="M:System.GC.SuppressFinalize(System.Object)" />
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object) this);
        }

        /// <summary>
        /// Can be over written in descendants to dispose of internal components.
        /// </summary>
        /// <param name="disposing">true, if called from Dispose(), false, if invoked inside a finalizer</param>
        protected abstract void Dispose(bool disposing);
    }
}