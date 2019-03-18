using System;
using System.Runtime.Serialization;
using System.Text;

namespace ADSD.Crypto
{
    /// <summary>Contains a serialized version of the original token that was used at sign-in time.</summary>
    [Serializable]
    public class BootstrapContext : ISerializable
    {
        private readonly string _tokenString;
        private readonly byte[] _tokenBytes;
        private const string _tokenTypeKey = "K";
        private const string _tokenKey = "T";
        private const char _securityTokenType = 'T';
        private const char _stringTokenType = 'S';
        private const char _byteTokenType = 'B';

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.BootstrapContext" /> class from a stream.</summary>
        /// <param name="info">The serialized data.</param>
        /// <param name="context">The context for serialization.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="info" /> is null.</exception>
        protected BootstrapContext(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                return;
            switch (info.GetChar("K"))
            {
                case 'B':
                    this._tokenBytes = (byte[]) info.GetValue("T", typeof (byte[]));
                    break;
                case 'S':
                    this._tokenString = info.GetString("T");
                    break;
                case 'T':
                        this._tokenString = Encoding.UTF8.GetString(Convert.FromBase64String(info.GetString("T")));
                        break;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.BootstrapContext" /> class by using the specified string.</summary>
        /// <param name="token">A string that represents the token.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="token" /> is <see langword="null" />.</exception>
        public BootstrapContext(string token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof (token));
            this._tokenString = token;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.BootstrapContext" /> class by using the specified array.</summary>
        /// <param name="token">An array that represents the token.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="token" /> is <see langword="null" />.</exception>
        public BootstrapContext(byte[] token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof (token));
            this._tokenBytes = token;
        }

        /// <summary>Populates the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with data needed to serialize the current <see cref="T:System.IdentityModel.Tokens.BootstrapContext" /> object.</summary>
        /// <param name="info">The object to populate with data.</param>
        /// <param name="context">The destination for this serialization. Can be <see langword="null" />.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="info" /> is <see langword="null" />.</exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (this._tokenBytes != null)
            {
                info.AddValue("K", 'B');
                info.AddValue("T", (object) this._tokenBytes);
            }
            else if (this._tokenString != null)
            {
                info.AddValue("K", 'S');
                info.AddValue("T", (object) this._tokenString);
            }
            else
            {
                return;
            }
        }

        /// <summary>Gets the array that was used to initialize the context.</summary>
        /// <returns>The array that was used to initialize the context or <see langword="null" />.</returns>
        public byte[] TokenBytes
        {
            get
            {
                return this._tokenBytes;
            }
        }

        /// <summary>Gets the string that was used to initialize the context.</summary>
        /// <returns>The string that was used to initialize the context or <see langword="null" />.</returns>
        public string Token
        {
            get
            {
                return this._tokenString;
            }
        }
    }
}