using System.Security.Cryptography;
using System.Xml;

namespace ADSD
{
    /// <summary>Wraps the <see cref="T:System.Security.Cryptography.Xml.EncryptedKey" /> class, it to be placed as a subelement of the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> class.</summary>
    public class KeyInfoEncryptedKey : KeyInfoClause
    {
        private EncryptedKey m_encryptedKey;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoEncryptedKey" /> class. </summary>
        public KeyInfoEncryptedKey()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoEncryptedKey" /> class using an <see cref="T:System.Security.Cryptography.Xml.EncryptedKey" /> object.</summary>
        /// <param name="encryptedKey">An <see cref="T:System.Security.Cryptography.Xml.EncryptedKey" />  object that encapsulates an encrypted key.</param>
        public KeyInfoEncryptedKey(EncryptedKey encryptedKey)
        {
            this.m_encryptedKey = encryptedKey;
        }

        /// <summary>Gets or sets an <see cref="T:System.Security.Cryptography.Xml.EncryptedKey" /> object that encapsulates an encrypted key.</summary>
        /// <returns>An <see cref="T:System.Security.Cryptography.Xml.EncryptedKey" /> object that encapsulates an encrypted key.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.KeyInfoEncryptedKey.EncryptedKey" /> property is <see langword="null" />.</exception>
        public EncryptedKey EncryptedKey
        {
            get
            {
                return this.m_encryptedKey;
            }
            set
            {
                this.m_encryptedKey = value;
            }
        }

        /// <summary>Returns an XML representation of a <see cref="T:System.Security.Cryptography.Xml.KeyInfoEncryptedKey" /> object.</summary>
        /// <returns>An XML representation of a <see cref="T:System.Security.Cryptography.Xml.KeyInfoEncryptedKey" /> object. </returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The encrypted key is <see langword="null" />.</exception>
        public override XmlElement GetXml()
        {
            if (this.m_encryptedKey == null)
                throw new CryptographicException("Cryptography_Xml_InvalidElement", nameof (KeyInfoEncryptedKey));
            return this.m_encryptedKey.GetXml();
        }

        internal override XmlElement GetXml(XmlDocument xmlDocument)
        {
            if (this.m_encryptedKey == null)
                throw new CryptographicException("Cryptography_Xml_InvalidElement", nameof (KeyInfoEncryptedKey));
            return this.m_encryptedKey.GetXml(xmlDocument);
        }

        /// <summary>Parses the input <see cref="T:System.Xml.XmlElement" /> object and configures the internal state of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoEncryptedKey" /> object to match.</summary>
        /// <param name="value">The <see cref="T:System.Xml.XmlElement" /> object that specifies the state of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoEncryptedKey" /> object.</param>
        public override void LoadXml(XmlElement value)
        {
            this.m_encryptedKey = new EncryptedKey();
            this.m_encryptedKey.LoadXml(value);
        }
    }
}