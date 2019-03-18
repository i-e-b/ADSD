using System;
using System.Security.Cryptography;
using System.Xml;

namespace ADSD.Crypto
{
    /// <summary>Represents the <see langword="&lt;EncryptedKey&gt;" /> element in XML encryption. This class cannot be inherited.</summary>
    public sealed class EncryptedKey : EncryptedType
    {
        private string m_recipient;
        private string m_carriedKeyName;
        private ReferenceList m_referenceList;

        /// <summary>Gets or sets the optional <see langword="Recipient" /> attribute in XML encryption.</summary>
        /// <returns>A string representing the value of the <see langword="Recipient" /> attribute.</returns>
        public string Recipient
        {
            get
            {
                if (m_recipient == null)
                    m_recipient = string.Empty;
                return m_recipient;
            }
            set
            {
                m_recipient = value;
                m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the optional <see langword="&lt;CarriedKeyName&gt;" /> element in XML encryption.</summary>
        /// <returns>A string that represents a name for the key value.</returns>
        public string CarriedKeyName
        {
            get
            {
                return m_carriedKeyName;
            }
            set
            {
                m_carriedKeyName = value;
                m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the <see langword="&lt;ReferenceList&gt;" /> element in XML encryption.</summary>
        /// <returns>A <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> object.</returns>
        public ReferenceList ReferenceList
        {
            get
            {
                if (m_referenceList == null)
                    m_referenceList = new ReferenceList();
                return m_referenceList;
            }
        }

        /// <summary>Adds a <see langword="&lt;DataReference&gt; " />element to the <see langword="&lt;ReferenceList&gt;" /> element.</summary>
        /// <param name="dataReference">A <see cref="T:System.Security.Cryptography.Xml.DataReference" /> object to add to the <see cref="P:System.Security.Cryptography.Xml.EncryptedKey.ReferenceList" /> property.</param>
        public void AddReference(DataReference dataReference)
        {
            ReferenceList.Add((object) dataReference);
        }

        /// <summary>Adds a <see langword="&lt;KeyReference&gt; " />element to the <see langword="&lt;ReferenceList&gt;" /> element.</summary>
        /// <param name="keyReference">A <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object to add to the <see cref="P:System.Security.Cryptography.Xml.EncryptedKey.ReferenceList" /> property.</param>
        public void AddReference(KeyReference keyReference)
        {
            ReferenceList.Add((object) keyReference);
        }

        /// <summary>Loads the specified XML information into the <see langword="&lt;EncryptedKey&gt;" /> element in XML encryption.</summary>
        /// <param name="value">An <see cref="T:System.Xml.XmlElement" /> representing an XML element to use for the <see langword="&lt;EncryptedKey&gt;" /> element.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="value" /> parameter does not contain a <see cref="T:System.Security.Cryptography.Xml.CipherData" />  element.</exception>
        public override void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsmgr.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
            nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            Id = Exml.GetAttribute(value, "Id", "http://www.w3.org/2001/04/xmlenc#");
            Type = Exml.GetAttribute(value, "Type", "http://www.w3.org/2001/04/xmlenc#");
            MimeType = Exml.GetAttribute(value, "MimeType", "http://www.w3.org/2001/04/xmlenc#");
            Encoding = Exml.GetAttribute(value, "Encoding", "http://www.w3.org/2001/04/xmlenc#");
            Recipient = Exml.GetAttribute(value, "Recipient", "http://www.w3.org/2001/04/xmlenc#");
            XmlNode xmlNode1 = value.SelectSingleNode("enc:EncryptionMethod", nsmgr);
            EncryptionMethod = new EncryptionMethod();
            if (xmlNode1 != null)
                EncryptionMethod.LoadXml(xmlNode1 as XmlElement);
            KeyInfo = new KeyInfo();
            XmlNode xmlNode2 = value.SelectSingleNode("ds:KeyInfo", nsmgr);
            if (xmlNode2 != null)
                KeyInfo.LoadXml(xmlNode2 as XmlElement);
            XmlNode xmlNode3 = value.SelectSingleNode("enc:CipherData", nsmgr);
            if (xmlNode3 == null)
                throw new CryptographicException("Cryptography_Xml_MissingCipherData");
            CipherData = new CipherData();
            CipherData.LoadXml(xmlNode3 as XmlElement);
            XmlNode xmlNode4 = value.SelectSingleNode("enc:EncryptionProperties", nsmgr);
            if (xmlNode4 != null)
            {
                XmlNodeList xmlNodeList = xmlNode4.SelectNodes("enc:EncryptionProperty", nsmgr);
                if (xmlNodeList != null)
                {
                    foreach (XmlNode xmlNode5 in xmlNodeList)
                    {
                        EncryptionProperty encryptionProperty = new EncryptionProperty();
                        encryptionProperty.LoadXml(xmlNode5 as XmlElement);
                        EncryptionProperties.Add(encryptionProperty);
                    }
                }
            }
            XmlNode xmlNode6 = value.SelectSingleNode("enc:CarriedKeyName", nsmgr);
            if (xmlNode6 != null)
                CarriedKeyName = xmlNode6.InnerText;
            XmlNode xmlNode7 = value.SelectSingleNode("enc:ReferenceList", nsmgr);
            if (xmlNode7 != null)
            {
                XmlNodeList xmlNodeList1 = xmlNode7.SelectNodes("enc:DataReference", nsmgr);
                if (xmlNodeList1 != null)
                {
                    foreach (XmlNode xmlNode5 in xmlNodeList1)
                    {
                        DataReference dataReference = new DataReference();
                        dataReference.LoadXml(xmlNode5 as XmlElement);
                        ReferenceList.Add((object) dataReference);
                    }
                }
                XmlNodeList xmlNodeList2 = xmlNode7.SelectNodes("enc:KeyReference", nsmgr);
                if (xmlNodeList2 != null)
                {
                    foreach (XmlNode xmlNode5 in xmlNodeList2)
                    {
                        KeyReference keyReference = new KeyReference();
                        keyReference.LoadXml(xmlNode5 as XmlElement);
                        ReferenceList.Add((object) keyReference);
                    }
                }
            }
            m_cachedXml = value;
        }

        /// <summary>Returns the XML representation of the <see cref="T:System.Security.Cryptography.Xml.EncryptedKey" /> object.</summary>
        /// <returns>An <see cref="T:System.Xml.XmlElement" /> that represents the <see langword="&lt;EncryptedKey&gt;" /> element in XML encryption.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="T:System.Security.Cryptography.Xml.EncryptedKey" /> value is <see langword="null" />.</exception>
        public override XmlElement GetXml()
        {
            if (CacheValid)
                return m_cachedXml;
            return GetXml(new XmlDocument()
            {
                PreserveWhitespace = true
            });
        }

        internal XmlElement GetXml(XmlDocument document)
        {
            XmlElement element1 = document.CreateElement(nameof (EncryptedKey), "http://www.w3.org/2001/04/xmlenc#");
            if (!string.IsNullOrEmpty(Id))
                element1.SetAttribute("Id", Id);
            if (!string.IsNullOrEmpty(Type))
                element1.SetAttribute("Type", Type);
            if (!string.IsNullOrEmpty(MimeType))
                element1.SetAttribute("MimeType", MimeType);
            if (!string.IsNullOrEmpty(Encoding))
                element1.SetAttribute("Encoding", Encoding);
            if (!string.IsNullOrEmpty(Recipient))
                element1.SetAttribute("Recipient", Recipient);
            if (EncryptionMethod != null)
                element1.AppendChild((XmlNode) EncryptionMethod.GetXml(document));
            if (KeyInfo.Count > 0)
                element1.AppendChild((XmlNode) KeyInfo.GetXml(document));
            if (CipherData == null)
                throw new CryptographicException("Cryptography_Xml_MissingCipherData");
            element1.AppendChild((XmlNode) CipherData.GetXml(document));
            if (EncryptionProperties.Count > 0)
            {
                XmlElement element2 = document.CreateElement("EncryptionProperties", "http://www.w3.org/2001/04/xmlenc#");
                for (int index = 0; index < EncryptionProperties.Count; ++index)
                {
                    EncryptionProperty encryptionProperty = EncryptionProperties[index];
                    element2.AppendChild((XmlNode) encryptionProperty.GetXml(document));
                }
                element1.AppendChild((XmlNode) element2);
            }
            if (ReferenceList.Count > 0)
            {
                XmlElement element2 = document.CreateElement("ReferenceList", "http://www.w3.org/2001/04/xmlenc#");
                for (int index = 0; index < ReferenceList.Count; ++index)
                    element2.AppendChild((XmlNode) ReferenceList[index].GetXml(document));
                element1.AppendChild((XmlNode) element2);
            }
            if (CarriedKeyName != null)
            {
                XmlElement element2 = document.CreateElement("CarriedKeyName", "http://www.w3.org/2001/04/xmlenc#");
                XmlText textNode = document.CreateTextNode(CarriedKeyName);
                element2.AppendChild((XmlNode) textNode);
                element1.AppendChild((XmlNode) element2);
            }
            return element1;
        }
    }
}