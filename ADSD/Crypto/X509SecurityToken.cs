using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ADSD
{
    /// <summary>Represents a security token that is based upon an X.509 certificate.</summary>
    public class X509SecurityToken : SecurityToken, IDisposable
    {
        private static readonly DateTime MaxUtcDateTime = new DateTime(2299, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime MinUtcDateTime = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime effectiveTime = new DateTime(2299, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime expirationTime = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private string id;
        private X509Certificate2 certificate;
        private List<SecurityKey> securityKeys;
        private bool disposed;
        private bool disposable;

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509SecurityToken" /> class using the specified X.509 certificate. </summary>
        /// <param name="certificate">An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that contains the X.509 certificate. Sets the <see cref="P:System.IdentityModel.Tokens.X509SecurityToken.Certificate" /> property.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="certificate" /> is <see langword="null" />.</exception>
        public X509SecurityToken(X509Certificate2 certificate)
          : this(certificate, SecurityUniqueId.Create().Value)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509SecurityToken" /> class using the specified X.509 certificate and unique identifier. </summary>
        /// <param name="certificate">An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that contains the X.509 certificate. Sets the <see cref="P:System.IdentityModel.Tokens.X509SecurityToken.Certificate" /> property.</param>
        /// <param name="id">A unique identifier of the security token. Sets the <see cref="P:System.IdentityModel.Tokens.X509SecurityToken.Id" /> property.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="certificate" /> is <see langword="null" />.-or-
        /// <paramref name="id" /> is null.</exception>
        public X509SecurityToken(X509Certificate2 certificate, string id)
          : this(certificate, id, true)
        {
        }

        internal X509SecurityToken(X509Certificate2 certificate, bool clone)
          : this(certificate, SecurityUniqueId.Create().Value, clone)
        {
        }

        internal X509SecurityToken(X509Certificate2 certificate, bool clone, bool disposable)
          : this(certificate, SecurityUniqueId.Create().Value, clone, disposable)
        {
        }

        internal X509SecurityToken(X509Certificate2 certificate, string id, bool clone)
          : this(certificate, id, clone, true)
        {
        }

        internal X509SecurityToken(
          X509Certificate2 certificate,
          string id,
          bool clone,
          bool disposable)
        {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));
            if (id == null) throw new ArgumentNullException(nameof(id));
            this.id = id;
            this.certificate = clone ? new X509Certificate2((X509Certificate)certificate) : certificate;
            this.disposable = clone | disposable;
        }

        /// <summary>Gets a unique identifier of the security token.</summary>
        /// <returns>A unique identifier of the security token.</returns>
        public override string Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>Gets the cryptographic keys associated with the security token.</summary>
        /// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of type <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that contains the set of keys associated with the security token.</returns>
        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                this.ThrowIfDisposed();
                if (this.securityKeys == null) this.securityKeys = new List<SecurityKey>(1) { (SecurityKey) new X509AsymmetricSecurityKey(this.certificate) };
                return this.securityKeys.AsReadOnly();
            }
        }

        /// <summary>Gets the first instant in time at which this security token is valid.</summary>
        /// <returns>A <see cref="T:System.DateTime" /> that represents the instant in time at which this security token is first valid.</returns>
        public override DateTime ValidFrom
        {
            get
            {
                this.ThrowIfDisposed();
                if (this.effectiveTime == MaxUtcDateTime)
                    this.effectiveTime = this.certificate.NotBefore.ToUniversalTime();
                return this.effectiveTime;
            }
        }

        /// <summary>Gets the last instant in time at which this security token is valid.</summary>
        /// <returns>A <see cref="T:System.DateTime" /> that represents the last instant in time at which this security token is valid.</returns>
        public override DateTime ValidTo
        {
            get
            {
                this.ThrowIfDisposed();
                if (this.expirationTime == MinUtcDateTime)
                    this.expirationTime = this.certificate.NotAfter.ToUniversalTime();
                return this.expirationTime;
            }
        }

        /// <summary>Gets the X.509 certificate associated with the security token.</summary>
        /// <returns>An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that contains the X.509 certificate.</returns>
        public X509Certificate2 Certificate
        {
            get
            {
                this.ThrowIfDisposed();
                return this.certificate;
            }
        }

        /// <summary>Gets a value indicating whether this security token is capable of creating the specified key identifier.</summary>
        public override bool CanCreateKeyIdentifierClause<T>()
        {
            this.ThrowIfDisposed();
            if (typeof(T) == typeof(X509SubjectKeyIdentifierClause))
                return X509SubjectKeyIdentifierClause.CanCreateFrom(this.certificate);
            if (!(typeof(T) == typeof(X509ThumbprintKeyIdentifierClause)) && !(typeof(T) == typeof(X509IssuerSerialKeyIdentifierClause)) && !(typeof(T) == typeof(X509RawDataKeyIdentifierClause)))
                return base.CanCreateKeyIdentifierClause<T>();
            return true;
        }

        /// <summary>Creates the specified key identifier clause.</summary>
        /// <typeparam name="T">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> that specifies the key identifier to create.</typeparam>
        /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> that is a key identifier clause for the security token.</returns>
        public override T CreateKeyIdentifierClause<T>()
        {
            this.ThrowIfDisposed();
            if (typeof(T) == typeof(X509SubjectKeyIdentifierClause))
            {
                X509SubjectKeyIdentifierClause keyIdentifierClause;
                if (X509SubjectKeyIdentifierClause.TryCreateFrom(this.certificate, out keyIdentifierClause))
                    return keyIdentifierClause as T;
            }
            else
            {
                if (typeof(T) == typeof(X509ThumbprintKeyIdentifierClause))
                    return new X509ThumbprintKeyIdentifierClause(this.certificate) as T;
                if (typeof(T) == typeof(X509IssuerSerialKeyIdentifierClause))
                    return new X509IssuerSerialKeyIdentifierClause(this.certificate) as T;
                if (typeof(T) == typeof(X509RawDataKeyIdentifierClause))
                    return new X509RawDataKeyIdentifierClause(this.certificate) as T;
            }
            return base.CreateKeyIdentifierClause<T>();
        }

        /// <summary>Returns a value indicating whether the key identifier for this instance is equal to the specified key identifier.</summary>
        /// <param name="keyIdentifierClause">An <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> to compare to this instance.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="keyIdentifierClause" /> is one of the <see cref="T:System.IdentityModel.Tokens.X509SubjectKeyIdentifierClause" />, <see cref="T:System.IdentityModel.Tokens.X509ThumbprintKeyIdentifierClause" />, <see cref="T:System.IdentityModel.Tokens.X509IssuerSerialKeyIdentifierClause" />, or <see cref="T:System.IdentityModel.Tokens.X509RawDataKeyIdentifierClause" /> types and the key identifier clauses match; otherwise, <see langword="false" />.</returns>
        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            this.ThrowIfDisposed();
            X509SubjectKeyIdentifierClause identifierClause1 = keyIdentifierClause as X509SubjectKeyIdentifierClause;
            if (identifierClause1 != null)
                return identifierClause1.Matches(this.certificate);
            X509ThumbprintKeyIdentifierClause identifierClause2 = keyIdentifierClause as X509ThumbprintKeyIdentifierClause;
            if (identifierClause2 != null)
                return identifierClause2.Matches(this.certificate);
            X509IssuerSerialKeyIdentifierClause identifierClause3 = keyIdentifierClause as X509IssuerSerialKeyIdentifierClause;
            if (identifierClause3 != null)
                return identifierClause3.Matches(this.certificate);
            X509RawDataKeyIdentifierClause identifierClause4 = keyIdentifierClause as X509RawDataKeyIdentifierClause;
            if (identifierClause4 != null)
                return identifierClause4.Matches(this.certificate);
            return base.MatchesKeyIdentifierClause(keyIdentifierClause);
        }

        /// <summary>Releases all resources used by the <see cref="T:System.IdentityModel.Tokens.X509SecurityToken" />. </summary>
        public virtual void Dispose()
        {
            if (!this.disposable || this.disposed)
                return;
            this.disposed = true;
            this.certificate.Reset();
        }

        /// <summary>Throws an exception if the <see cref="M:System.IdentityModel.Tokens.X509SecurityToken.ThrowIfDisposed" /> method has been called for this instance.</summary>
        /// <exception cref="T:System.ObjectDisposedException">the <see cref="M:System.IdentityModel.Tokens.X509SecurityToken.ThrowIfDisposed" />  method has been called for this instance.</exception>
        protected void ThrowIfDisposed()
        {
            if (this.disposed) throw new ObjectDisposedException(this.GetType().FullName);
        }
    }
}
