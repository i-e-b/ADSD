using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Xml;

namespace ADSD
{
    /// <summary>Represents a collection of security token handlers.</summary>
    public class SecurityTokenHandlerCollection : Collection<SecurityTokenHandler>
    {
        internal static int defaultHandlerCollectionCount = 8;
        private Dictionary<string, SecurityTokenHandler> handlersByIdentifier = new Dictionary<string, SecurityTokenHandler>();
        private Dictionary<Type, SecurityTokenHandler> handlersByType = new Dictionary<Type, SecurityTokenHandler>();
        private SecurityTokenHandlerConfiguration configuration;
        private System.IdentityModel.Tokens.KeyInfoSerializer keyInfoSerializer;

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.SecurityTokenHandlerCollection" /> class.</summary>
        public SecurityTokenHandlerCollection()
            : this(new SecurityTokenHandlerConfiguration())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.SecurityTokenHandlerCollection" /> class with the specified configuration.</summary>
        /// <param name="configuration">The base configuration to associate with the collection.</param>
        public SecurityTokenHandlerCollection(SecurityTokenHandlerConfiguration configuration)
        {
            if (configuration == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (configuration));
            this.configuration = configuration;
            this.keyInfoSerializer = new System.IdentityModel.Tokens.KeyInfoSerializer(true);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.SecurityTokenHandlerCollection" /> class with the specified token handlers.</summary>
        /// <param name="handlers">The token handlers with which to initialize the new instance.</param>
        public SecurityTokenHandlerCollection(IEnumerable<SecurityTokenHandler> handlers)
            : this(handlers, new SecurityTokenHandlerConfiguration())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.SecurityTokenHandlerCollection" /> class with the specified token handlers and configuration.</summary>
        /// <param name="handlers">The token handlers with which to initialize the new instance.</param>
        /// <param name="configuration">The base configuration to associate with the collection.</param>
        public SecurityTokenHandlerCollection(
            IEnumerable<SecurityTokenHandler> handlers,
            SecurityTokenHandlerConfiguration configuration)
            : this(configuration)
        {
            if (handlers == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (handlers));
            foreach (SecurityTokenHandler handler in handlers)
                this.Add(handler);
        }

        /// <summary>Gets and sets the base configuration for this security token handler collection.</summary>
        /// <returns>The configuration for the token handler collection.</returns>
        public SecurityTokenHandlerConfiguration Configuration
        {
            get
            {
                return this.configuration;
            }
        }

        /// <summary>Gets a list of the types of the tokens handled by handlers in this collection.</summary>
        /// <returns>The list of types.</returns>
        public IEnumerable<Type> TokenTypes
        {
            get
            {
                return (IEnumerable<Type>) this.handlersByType.Keys;
            }
        }

        /// <summary>Gets a list of the type identifiers of the tokens handled by the handlers in this collection.</summary>
        /// <returns>The list of type identifier URIs.</returns>
        public IEnumerable<string> TokenTypeIdentifiers
        {
            get
            {
                return (IEnumerable<string>) this.handlersByIdentifier.Keys;
            }
        }

        /// <summary>Gets a token handler from this collection that can handle the specified type identifier.</summary>
        /// <param name="tokenTypeIdentifier">A URI that identifies the token type.</param>
        /// <returns>A token handler that can handle tokens that correspond to the specified type identifier.</returns>
        public SecurityTokenHandler this[string tokenTypeIdentifier]
        {
            get
            {
                if (string.IsNullOrEmpty(tokenTypeIdentifier))
                    return (SecurityTokenHandler) null;
                SecurityTokenHandler securityTokenHandler;
                this.handlersByIdentifier.TryGetValue(tokenTypeIdentifier, out securityTokenHandler);
                return securityTokenHandler;
            }
        }

        /// <summary>Gets a token handler from this collection that can handle the specified security token.</summary>
        /// <param name="token">The token for which the handler should be returned.</param>
        /// <returns>A token handler that can handle the specified token.</returns>
        public SecurityTokenHandler this[SecurityToken token]
        {
            get
            {
                if (token == null)
                    return (SecurityTokenHandler) null;
                return this[token.GetType()];
            }
        }

        /// <summary>Gets the handler from this collection that can handle the specified token type.</summary>
        /// <param name="tokenType">The type of the token to be handled.</param>
        /// <returns>A token handler that can handle tokens of the specified type.</returns>
        public SecurityTokenHandler this[Type tokenType]
        {
            get
            {
                SecurityTokenHandler securityTokenHandler = (SecurityTokenHandler) null;
                if (tokenType != (Type) null)
                    this.handlersByType.TryGetValue(tokenType, out securityTokenHandler);
                return securityTokenHandler;
            }
        }

        /// <summary>Creates a system default collection of basic security token handlers, each of which has the system default configuration. The token handlers in this collection must be configured with service specific data before they can be used.</summary>
        /// <returns>A security token handler collection that contains the default, basic security token handlers.</returns>
        public static SecurityTokenHandlerCollection CreateDefaultSecurityTokenHandlerCollection()
        {
            return SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection(new SecurityTokenHandlerConfiguration());
        }

        /// <summary>Creates a system default collection of basic security token handlers and associates the specified configuration with the new collection. Each of the handlers has the system default configuration. The token handlers in this collection must be configured with service specific data before they can be used.</summary>
        /// <param name="configuration">The configuration to associate with the handler collection.</param>
        /// <returns>A security token handler collection that contains the default, basic security token handlers.</returns>
        public static SecurityTokenHandlerCollection CreateDefaultSecurityTokenHandlerCollection(
            SecurityTokenHandlerConfiguration configuration)
        {
            SecurityTokenHandlerCollection handlerCollection = new SecurityTokenHandlerCollection((IEnumerable<SecurityTokenHandler>) new SecurityTokenHandler[8]
            {
                (SecurityTokenHandler) new KerberosSecurityTokenHandler(),
                (SecurityTokenHandler) new RsaSecurityTokenHandler(),
                (SecurityTokenHandler) new SamlSecurityTokenHandler(),
                (SecurityTokenHandler) new Saml2SecurityTokenHandler(),
                (SecurityTokenHandler) new WindowsUserNameSecurityTokenHandler(),
                (SecurityTokenHandler) new X509SecurityTokenHandler(),
                (SecurityTokenHandler) new EncryptedSecurityTokenHandler(),
                (SecurityTokenHandler) new SessionSecurityTokenHandler()
            }, configuration);
            SecurityTokenHandlerCollection.defaultHandlerCollectionCount = handlerCollection.Count;
            return handlerCollection;
        }

        internal SecurityTokenSerializer KeyInfoSerializer
        {
            get
            {
                return (SecurityTokenSerializer) this.keyInfoSerializer;
            }
        }

        /// <summary>Adds the specified token handler to this collection. If a handler with the same token type identifier as the specified handler already exists in the collection, it is replaced with the specified handler.</summary>
        /// <param name="handler">The token handler to add to the collection.</param>
        public void AddOrReplace(SecurityTokenHandler handler)
        {
            if (handler == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (handler));
            Type tokenType = handler.TokenType;
            if (tokenType != (Type) null && this.handlersByType.ContainsKey(tokenType))
            {
                this.Remove(this[tokenType]);
            }
            else
            {
                string[] tokenTypeIdentifiers = handler.GetTokenTypeIdentifiers();
                if (tokenTypeIdentifiers != null)
                {
                    foreach (string key in tokenTypeIdentifiers)
                    {
                        if (key != null && this.handlersByIdentifier.ContainsKey(key))
                        {
                            this.Remove(this[key]);
                            break;
                        }
                    }
                }
            }
            this.Add(handler);
        }

        /// <summary>Returns a value that indicates whether the specified token can be read by one of the handlers in this collection.</summary>
        /// <param name="reader">An XML reader positioned at the start element. The reader should not be advanced.</param>
        /// <returns>
        /// <see langword="true" /> if the token can be read; otherwise <see langword="false" />.</returns>
        public bool CanReadToken(XmlReader reader)
        {
            if (reader == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (reader));
            foreach (SecurityTokenHandler securityTokenHandler in (Collection<SecurityTokenHandler>) this)
            {
                if (securityTokenHandler != null && securityTokenHandler.CanReadToken(reader))
                    return true;
            }
            return false;
        }

        /// <summary>Returns a value that indicates whether the specified string representation of a token can be read by one of the handlers in this collection.</summary>
        /// <param name="tokenString">The token represented as a string.</param>
        /// <returns>
        /// <see langword="true" /> if the collection contains a token handler that can read the specified token; otherwise <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="tokenString" /> is <see langword="null" /> or an empty string.</exception>
        public bool CanReadToken(string tokenString)
        {
            if (string.IsNullOrEmpty(tokenString))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNullOrEmptyString(nameof (tokenString));
            foreach (SecurityTokenHandler securityTokenHandler in (Collection<SecurityTokenHandler>) this)
            {
                if (securityTokenHandler != null && securityTokenHandler.CanReadToken(tokenString))
                    return true;
            }
            return false;
        }

        /// <summary>Returns a value that indicates whether the specified token can be serialized by one of the handlers in this collection.</summary>
        /// <param name="token">The security token to be serialized.</param>
        /// <returns>
        /// <see langword="true" /> if the token can be serialized by one of the handlers; otherwise <see langword="false" />.</returns>
        public bool CanWriteToken(SecurityToken token)
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (token));
            SecurityTokenHandler securityTokenHandler = this[token];
            return securityTokenHandler != null && securityTokenHandler.CanWriteToken;
        }

        /// <summary>Creates a token from the specified token descriptor.</summary>
        /// <param name="tokenDescriptor">The token descriptor from which the token is to be created. Properties of the token descriptor are set before this method is called.</param>
        /// <returns>A security token that matches the properties of the token descriptor.</returns>
        public SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor)
        {
            if (tokenDescriptor == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (tokenDescriptor));
            SecurityTokenHandler securityTokenHandler = this[tokenDescriptor.TokenType];
            if (securityTokenHandler == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(System.IdentityModel.SR.GetString("ID4020", (object) tokenDescriptor.TokenType)));
            return securityTokenHandler.CreateToken(tokenDescriptor);
        }

        /// <summary>Validates the specified security token.</summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>The identities that are contained in the token.</returns>
        public ReadOnlyCollection<ClaimsIdentity> ValidateToken(
            SecurityToken token)
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (token));
            SecurityTokenHandler securityTokenHandler = this[token];
            if (securityTokenHandler == null || !securityTokenHandler.CanValidateToken)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(System.IdentityModel.SR.GetString("ID4011", (object) token.GetType())));
            return securityTokenHandler.ValidateToken(token);
        }

        /// <summary>Deserializes a security token from the specified XML reader.</summary>
        /// <param name="reader">An XML reader positioned at the start element of the token.</param>
        /// <returns>The security token deserialized from the XML.</returns>
        public SecurityToken ReadToken(XmlReader reader)
        {
            if (reader == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (reader));
            foreach (SecurityTokenHandler securityTokenHandler in (Collection<SecurityTokenHandler>) this)
            {
                if (securityTokenHandler != null && securityTokenHandler.CanReadToken(reader))
                    return securityTokenHandler.ReadToken(reader);
            }
            return (SecurityToken) null;
        }

        /// <summary>Deserializes a security token from the specified string.</summary>
        /// <param name="tokenString">The string from which to deserialize the token.</param>
        /// <returns>The token that was deserialized from the specified string.</returns>
        public SecurityToken ReadToken(string tokenString)
        {
            if (string.IsNullOrEmpty(tokenString))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNullOrEmptyString(nameof (tokenString));
            foreach (SecurityTokenHandler securityTokenHandler in (Collection<SecurityTokenHandler>) this)
            {
                if (securityTokenHandler != null && securityTokenHandler.CanReadToken(tokenString))
                    return securityTokenHandler.ReadToken(tokenString);
            }
            return (SecurityToken) null;
        }

        /// <summary>Serializes the specified security token to XML.</summary>
        /// <param name="writer">The XML writer.</param>
        /// <param name="token">The token to serialize.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="token" /> is <see langword="null" />.-or-
        /// <paramref name="writer" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The collection does not contain a handler capable of serializing the specified token.</exception>
        public void WriteToken(XmlWriter writer, SecurityToken token)
        {
            if (writer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (writer));
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (token));
            SecurityTokenHandler securityTokenHandler = this[token];
            if (securityTokenHandler == null || !securityTokenHandler.CanWriteToken)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(System.IdentityModel.SR.GetString("ID4010", (object) token.GetType())));
            securityTokenHandler.WriteToken(writer, token);
        }

        /// <summary>Serializes the specified security token to a string.</summary>
        /// <param name="token">The token to serialize.</param>
        /// <returns>The string serialized from the token.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="token" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The collection does not contain a handler capable of serializing the specified token.</exception>
        public string WriteToken(SecurityToken token)
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (token));
            SecurityTokenHandler securityTokenHandler = this[token];
            if (securityTokenHandler == null || !securityTokenHandler.CanWriteToken)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(System.IdentityModel.SR.GetString("ID4010", (object) token.GetType())));
            return securityTokenHandler.WriteToken(token);
        }

        /// <summary>Clears all of the handlers from this collection. (Override of the base class method.)</summary>
        protected override void ClearItems()
        {
            base.ClearItems();
            this.handlersByIdentifier.Clear();
            this.handlersByType.Clear();
        }

        /// <summary>Inserts the specified token handler in this collection at the specified index. (Override of the base class method.)</summary>
        /// <param name="index">The zero-based index at which the handler should be inserted.</param>
        /// <param name="item">The token handler to insert.</param>
        protected override void InsertItem(int index, SecurityTokenHandler item)
        {
            base.InsertItem(index, item);
            try
            {
                this.AddToDictionaries(item);
            }
            catch
            {
                base.RemoveItem(index);
                throw;
            }
        }

        /// <summary>Removes the handler at the specified index from this collection. (Override of the base class method.)</summary>
        /// <param name="index">The zero-based index of the handler to remove.</param>
        protected override void RemoveItem(int index)
        {
            SecurityTokenHandler handler = this.Items[index];
            base.RemoveItem(index);
            this.RemoveFromDictionaries(handler);
        }

        /// <summary>Replaces the token handler at the specified index in the collection with the specified handler. </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new token handler for the element at the specified index.</param>
        protected override void SetItem(int index, SecurityTokenHandler item)
        {
            SecurityTokenHandler handler = this.Items[index];
            base.SetItem(index, item);
            this.RemoveFromDictionaries(handler);
            try
            {
                this.AddToDictionaries(item);
            }
            catch
            {
                base.SetItem(index, handler);
                this.AddToDictionaries(handler);
                throw;
            }
        }

        /// <summary>Returns a value that indicates whether the specified key identifier clause can be read by one of the handlers in this collection or by the base <see cref="T:System.ServiceModel.Security.WSSecurityTokenSerializer" />.</summary>
        /// <param name="reader">An XML reader positioned at the start element. The reader should not be advanced.</param>
        /// <returns>
        /// <see langword="true" /> if the key identifier clause can be read; otherwise <see langword="false" />.</returns>
        public bool CanReadKeyIdentifierClause(XmlReader reader)
        {
            if (reader == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (reader));
            return this.CanReadKeyIdentifierClauseCore(reader);
        }

        /// <summary>Returns a value that indicates whether the specified key identifier clause can be read by one of the handlers in the collection or by the base <see cref="T:System.ServiceModel.Security.WSSecurityTokenSerializer" />.</summary>
        /// <param name="reader">An XML reader positioned at the start element. The reader should not be advanced.</param>
        /// <returns>
        /// <see langword="true" /> if the key identifier clause can be read; otherwise <see langword="false" />.</returns>
        protected virtual bool CanReadKeyIdentifierClauseCore(XmlReader reader)
        {
            if (reader == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (reader));
            foreach (SecurityTokenHandler securityTokenHandler in (Collection<SecurityTokenHandler>) this)
            {
                if (securityTokenHandler.CanReadKeyIdentifierClause(reader))
                    return true;
            }
            return false;
        }

        /// <summary>Deserializes a key identifier clause from the specified XML reader.</summary>
        /// <param name="reader">An XML reader positioned at the start element of the XML to be deserialized into the key identifier clause.</param>
        /// <returns>The key identifier clause deserialized from the XML.</returns>
        public SecurityKeyIdentifierClause ReadKeyIdentifierClause(
            XmlReader reader)
        {
            if (reader == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (reader));
            return this.ReadKeyIdentifierClauseCore(reader);
        }

        /// <summary>Deserializes a key identifier clause from the specified XML reader.</summary>
        /// <param name="reader">An XML reader positioned at the start element of the XML to be deserialized into the key identifier clause.</param>
        /// <returns>The key identifier clause deserialized from the XML.</returns>
        protected virtual SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(
            XmlReader reader)
        {
            if (reader == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (reader));
            foreach (SecurityTokenHandler securityTokenHandler in (Collection<SecurityTokenHandler>) this)
            {
                if (securityTokenHandler.CanReadKeyIdentifierClause(reader))
                    return securityTokenHandler.ReadKeyIdentifierClause(reader);
            }
            return this.keyInfoSerializer.ReadKeyIdentifierClause(reader);
        }

        /// <summary>Serializes the specified key identifier clause to XML.</summary>
        /// <param name="writer">The XML writer.</param>
        /// <param name="keyIdentifierClause">The key identifier clause to serialize.</param>
        public void WriteKeyIdentifierClause(
            XmlWriter writer,
            SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (writer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (writer));
            if (keyIdentifierClause == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (keyIdentifierClause));
            this.WriteKeyIdentifierClauseCore(writer, keyIdentifierClause);
        }

        /// <summary>Serializes the specified key identifier clause to XML.</summary>
        /// <param name="writer">The XML writer.</param>
        /// <param name="keyIdentifierClause">The key identifier clause to serialize.</param>
        protected virtual void WriteKeyIdentifierClauseCore(
            XmlWriter writer,
            SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (writer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (writer));
            if (keyIdentifierClause == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (keyIdentifierClause));
            foreach (SecurityTokenHandler securityTokenHandler in (Collection<SecurityTokenHandler>) this)
            {
                if (securityTokenHandler.CanWriteKeyIdentifierClause(keyIdentifierClause))
                {
                    securityTokenHandler.WriteKeyIdentifierClause(writer, keyIdentifierClause);
                    return;
                }
            }
            this.keyInfoSerializer.WriteKeyIdentifierClause(writer, keyIdentifierClause);
        }

        private void AddToDictionaries(SecurityTokenHandler handler)
        {
            if (handler == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof (handler));
            bool flag = false;
            string[] tokenTypeIdentifiers = handler.GetTokenTypeIdentifiers();
            if (tokenTypeIdentifiers != null)
            {
                foreach (string key in tokenTypeIdentifiers)
                {
                    if (key != null)
                    {
                        this.handlersByIdentifier.Add(key, handler);
                        flag = true;
                    }
                }
            }
            Type tokenType = handler.TokenType;
            if (handler.TokenType != (Type) null)
            {
                try
                {
                    this.handlersByType.Add(tokenType, handler);
                }
                catch
                {
                    if (flag)
                        this.RemoveFromDictionaries(handler);
                    throw;
                }
            }
            handler.ContainingCollection = this;
            if (handler.Configuration != null)
                return;
            handler.Configuration = this.configuration;
        }

        private void RemoveFromDictionaries(SecurityTokenHandler handler)
        {
            string[] tokenTypeIdentifiers = handler.GetTokenTypeIdentifiers();
            if (tokenTypeIdentifiers != null)
            {
                foreach (string key in tokenTypeIdentifiers)
                {
                    if (key != null)
                        this.handlersByIdentifier.Remove(key);
                }
            }
            Type tokenType = handler.TokenType;
            if (tokenType != (Type) null && this.handlersByType.ContainsKey(tokenType))
                this.handlersByType.Remove(tokenType);
            handler.ContainingCollection = (SecurityTokenHandlerCollection) null;
            handler.Configuration = (SecurityTokenHandlerConfiguration) null;
        }
    }
}