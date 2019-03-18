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

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.BootstrapContext" /> class from a stream.</summary>
        /// <param name="info">The serialized data.</param>
        /// <param name="context">The context for serialization.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="info" /> is null.</exception>
        protected BootstrapContext(SerializationInfo info, StreamingContext context)
        {
            if (info == null) return;
            switch (info.GetChar("K"))
            {
                case 'B':
                    _tokenBytes = (byte[])info.GetValue("T", typeof(byte[]));
                    break;
                case 'S':
                    _tokenString = info.GetString("T");
                    break;
                case 'T':
                    _tokenString = Encoding.UTF8.GetString(
                        Convert.FromBase64String(info.GetString("T") ?? throw new InvalidOperationException()));
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
            _tokenString = token;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.BootstrapContext" /> class by using the specified array.</summary>
        /// <param name="token">An array that represents the token.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="token" /> is <see langword="null" />.</exception>
        public BootstrapContext(byte[] token)
        {
            _tokenBytes = token ?? throw new ArgumentNullException(nameof (token));
        }

        /// <summary>Populates the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with data needed to serialize the current <see cref="T:System.IdentityModel.Tokens.BootstrapContext" /> object.</summary>
        /// <param name="info">The object to populate with data.</param>
        /// <param name="context">The destination for this serialization. Can be <see langword="null" />.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="info" /> is <see langword="null" />.</exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (_tokenBytes != null)
            {
                info.AddValue("K", 'B');
                info.AddValue("T", _tokenBytes);
            }
            else if (_tokenString != null)
            {
                info.AddValue("K", 'S');
                info.AddValue("T", _tokenString);
            }
        }

        /// <summary>Gets the array that was used to initialize the context.</summary>
        /// <returns>The array that was used to initialize the context or <see langword="null" />.</returns>
        public byte[] TokenBytes
        {
            get
            {
                return _tokenBytes;
            }
        }

        /// <summary>Gets the string that was used to initialize the context.</summary>
        /// <returns>The string that was used to initialize the context or <see langword="null" />.</returns>
        public string Token
        {
            get
            {
                return _tokenString;
            }
        }
    }
}