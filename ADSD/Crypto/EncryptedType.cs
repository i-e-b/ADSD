using System;
using System.Xml;

namespace ADSD.Crypto
{
    /// <summary>Represents the abstract base class from which the classes <see cref="T:System.Security.Cryptography.Xml.EncryptedData" /> and <see cref="T:System.Security.Cryptography.Xml.EncryptedKey" /> derive.</summary>
    public abstract class EncryptedType
    {
        private string m_id;
        private string m_type;
        private string m_mimeType;
        private string m_encoding;
        private EncryptionMethod m_encryptionMethod;
        private CipherData m_cipherData;
        private EncryptionPropertyCollection m_props;
        private KeyInfo m_keyInfo;
        internal XmlElement m_cachedXml;

        internal bool CacheValid
        {
            get
            {
                return this.m_cachedXml != null;
            }
        }

        /// <summary>Gets or sets the <see langword="Id" /> attribute of an <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> instance in XML encryption.</summary>
        /// <returns>A string of the <see langword="Id" /> attribute of the <see langword="&lt;EncryptedType&gt;" /> element.</returns>
        public virtual string Id
        {
            get
            {
                return this.m_id;
            }
            set
            {
                this.m_id = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the <see langword="Type" /> attribute of an <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> instance in XML encryption.</summary>
        /// <returns>A string that describes the text form of the encrypted data.</returns>
        public virtual string Type
        {
            get
            {
                return this.m_type;
            }
            set
            {
                this.m_type = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the <see langword="MimeType" /> attribute of an <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> instance in XML encryption.</summary>
        /// <returns>A string that describes the media type of the encrypted data.</returns>
        public virtual string MimeType
        {
            get
            {
                return this.m_mimeType;
            }
            set
            {
                this.m_mimeType = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the <see langword="Encoding" /> attribute of an <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> instance in XML encryption.</summary>
        /// <returns>A string that describes the encoding of the encrypted data.</returns>
        public virtual string Encoding
        {
            get
            {
                return this.m_encoding;
            }
            set
            {
                this.m_encoding = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets of sets the <see langword="&lt;KeyInfo&gt;" /> element in XML encryption.</summary>
        /// <returns>A <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object.</returns>
        public KeyInfo KeyInfo
        {
            get
            {
                if (this.m_keyInfo == null)
                    this.m_keyInfo = new KeyInfo();
                return this.m_keyInfo;
            }
            set
            {
                this.m_keyInfo = value;
            }
        }

        /// <summary>Gets or sets the <see langword="&lt;EncryptionMethod&gt;" /> element for XML encryption.</summary>
        /// <returns>An <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> object that represents the <see langword="&lt;EncryptionMethod&gt;" /> element.</returns>
        public virtual EncryptionMethod EncryptionMethod
        {
            get
            {
                return this.m_encryptionMethod;
            }
            set
            {
                this.m_encryptionMethod = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the <see langword="&lt;EncryptionProperties&gt;" /> element in XML encryption.</summary>
        /// <returns>An <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</returns>
        public virtual EncryptionPropertyCollection EncryptionProperties
        {
            get
            {
                if (this.m_props == null)
                    this.m_props = new EncryptionPropertyCollection();
                return this.m_props;
            }
        }

        /// <summary>Adds an <see langword="&lt;EncryptionProperty&gt;" /> child element to the <see langword="&lt;EncryptedProperties&gt;" /> element in the current <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> object in XML encryption.</summary>
        /// <param name="ep">An <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object.</param>
        public void AddProperty(EncryptionProperty ep)
        {
            this.EncryptionProperties.Add(ep);
        }

        /// <summary>Gets or sets the <see cref="T:System.Security.Cryptography.Xml.CipherData" /> value for an instance of an <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> class.</summary>
        /// <returns>A <see cref="T:System.Security.Cryptography.Xml.CipherData" /> object.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Security.Cryptography.Xml.EncryptedType.CipherData" /> property was set to <see langword="null" />.</exception>
        public virtual CipherData CipherData
        {
            get
            {
                if (this.m_cipherData == null)
                    this.m_cipherData = new CipherData();
                return this.m_cipherData;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof (value));
                this.m_cipherData = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Loads XML information into the <see langword="&lt;EncryptedType&gt;" /> element in XML encryption.</summary>
        /// <param name="value">An <see cref="T:System.Xml.XmlElement" /> object representing an XML element to use in the <see langword="&lt;EncryptedType&gt;" /> element.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> provided is <see langword="null" />.</exception>
        public abstract void LoadXml(XmlElement value);

        /// <summary>Returns the XML representation of the <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> object.</summary>
        /// <returns>An <see cref="T:System.Xml.XmlElement" /> object that represents the <see langword="&lt;EncryptedType&gt;" /> element in XML encryption.</returns>
        public abstract XmlElement GetXml();
    }
}