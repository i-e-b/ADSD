using System;
using System.Collections;
using System.Security.Cryptography;
using System.Xml;

namespace ADSD.Crypto
{
    /// <summary>Represents the <see langword="&lt;Signature&gt;" /> element of an XML signature.</summary>
    public class Signature
    {
        private string m_id;
        private SignedInfo m_signedInfo;
        private byte[] m_signatureValue;
        private string m_signatureValueId;
        private KeyInfo m_keyInfo;
        private IList m_embeddedObjects;
        private readonly CanonicalXmlNodeList m_referencedItems;

        internal SignedXml SignedXml { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.Signature" /> class.</summary>
        public Signature()
        {
            this.m_embeddedObjects = (IList) new ArrayList();
            this.m_referencedItems = new CanonicalXmlNodeList();
        }

        /// <summary>Gets or sets the ID of the current <see cref="T:System.Security.Cryptography.Xml.Signature" />.</summary>
        /// <returns>The ID of the current <see cref="T:System.Security.Cryptography.Xml.Signature" />. The default is <see langword="null" />.</returns>
        public string Id
        {
            get
            {
                return this.m_id;
            }
            set
            {
                this.m_id = value;
            }
        }

        /// <summary>Gets or sets the <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> of the current <see cref="T:System.Security.Cryptography.Xml.Signature" />.</summary>
        /// <returns>The <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> of the current <see cref="T:System.Security.Cryptography.Xml.Signature" />.</returns>
        public SignedInfo SignedInfo
        {
            get
            {
                return this.m_signedInfo;
            }
            set
            {
                this.m_signedInfo = value;
                if (this.SignedXml == null || this.m_signedInfo == null)
                    return;
                this.m_signedInfo.SignedXml = this.SignedXml;
            }
        }

        /// <summary>Gets or sets the value of the digital signature.</summary>
        /// <returns>A byte array that contains the value of the digital signature.</returns>
        public byte[] SignatureValue
        {
            get
            {
                return this.m_signatureValue;
            }
            set
            {
                this.m_signatureValue = value;
            }
        }

        /// <summary>Gets or sets the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> of the current <see cref="T:System.Security.Cryptography.Xml.Signature" />.</summary>
        /// <returns>The <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> of the current <see cref="T:System.Security.Cryptography.Xml.Signature" />.</returns>
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

        /// <summary>Gets or sets a list of objects to be signed.</summary>
        /// <returns>A list of objects to be signed.</returns>
        public IList ObjectList
        {
            get
            {
                return this.m_embeddedObjects;
            }
            set
            {
                this.m_embeddedObjects = value;
            }
        }

        internal CanonicalXmlNodeList ReferencedItems
        {
            get
            {
                return this.m_referencedItems;
            }
        }

        /// <summary>Returns the XML representation of the <see cref="T:System.Security.Cryptography.Xml.Signature" />.</summary>
        /// <returns>The XML representation of the <see cref="T:System.Security.Cryptography.Xml.Signature" />.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.Signature.SignedInfo" /> property is <see langword="null" />.-or- The <see cref="P:System.Security.Cryptography.Xml.Signature.SignatureValue" /> property is <see langword="null" />. </exception>
        public XmlElement GetXml()
        {
            return this.GetXml(new XmlDocument()
            {
                PreserveWhitespace = true
            });
        }

        internal XmlElement GetXml(XmlDocument document)
        {
            XmlElement element1 = document.CreateElement(nameof (Signature), "http://www.w3.org/2000/09/xmldsig#");
            if (!string.IsNullOrEmpty(this.m_id))
                element1.SetAttribute("Id", this.m_id);
            if (this.m_signedInfo == null)
                throw new CryptographicException("XML Signed info required");
            element1.AppendChild((XmlNode) this.m_signedInfo.GetXml(document));
            if (this.m_signatureValue == null)
                throw new CryptographicException("XML Signature value required");
            XmlElement element2 = document.CreateElement("SignatureValue", "http://www.w3.org/2000/09/xmldsig#");
            element2.AppendChild((XmlNode) document.CreateTextNode(Convert.ToBase64String(this.m_signatureValue)));
            if (!string.IsNullOrEmpty(this.m_signatureValueId))
                element2.SetAttribute("Id", this.m_signatureValueId);
            element1.AppendChild((XmlNode) element2);
            if (this.KeyInfo.Count > 0)
                element1.AppendChild((XmlNode) this.KeyInfo.GetXml(document));
            foreach (object embeddedObject in (IEnumerable) this.m_embeddedObjects)
            {
                DataObject dataObject = embeddedObject as DataObject;
                if (dataObject != null)
                    element1.AppendChild((XmlNode) dataObject.GetXml(document));
            }
            return element1;
        }

        /// <summary>Loads a <see cref="T:System.Security.Cryptography.Xml.Signature" /> state from an XML element.</summary>
        /// <param name="value">The XML element from which to load the <see cref="T:System.Security.Cryptography.Xml.Signature" /> state. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />. </exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="value" /> parameter does not contain a valid <see cref="P:System.Security.Cryptography.Xml.Signature.SignatureValue" />.-or- The <paramref name="value" /> parameter does not contain a valid <see cref="P:System.Security.Cryptography.Xml.Signature.SignedInfo" />. </exception>
        public void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            XmlElement element1 = value;
            if (!element1.LocalName.Equals(nameof (Signature)))
                throw new CryptographicException("Invalid element: Signature (1)");
            this.m_id = Exml.GetAttribute(element1, "Id", "http://www.w3.org/2000/09/xmldsig#");
            if (!Exml.VerifyAttributes(element1, "Id"))
                throw new CryptographicException("Invalid element: Signature (2)");
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            int num1 = 0;
            XmlNodeList xmlNodeList1 = element1.SelectNodes("ds:SignedInfo", nsmgr);
            if (xmlNodeList1 == null || xmlNodeList1.Count == 0 || xmlNodeList1.Count > 1)
                throw new CryptographicException("Invalid element: SignedInfo");
            XmlElement xmlElement1 = xmlNodeList1[0] as XmlElement;
            int num2 = num1 + xmlNodeList1.Count;
            this.SignedInfo = new SignedInfo();
            this.SignedInfo.LoadXml(xmlElement1);
            XmlNodeList xmlNodeList2 = element1.SelectNodes("ds:SignatureValue", nsmgr);
            if (xmlNodeList2 == null || xmlNodeList2.Count == 0 || xmlNodeList2.Count > 1)
                throw new CryptographicException("Invalid element: SignatureValue (1)");
            XmlElement element2 = xmlNodeList2[0] as XmlElement;
            int num3 = num2 + xmlNodeList2.Count;
            this.m_signatureValue = Convert.FromBase64String(Exml.DiscardWhiteSpaces(element2.InnerText, 0, element2.InnerText.Length));
            this.m_signatureValueId = Exml.GetAttribute(element2, "Id", "http://www.w3.org/2000/09/xmldsig#");
            if (!Exml.VerifyAttributes(element2, "Id"))
                throw new CryptographicException("Invalid element: SignatureValue (2)");
            XmlNodeList xmlNodeList3 = element1.SelectNodes("ds:KeyInfo", nsmgr);
            this.m_keyInfo = new KeyInfo();
            if (xmlNodeList3 != null)
            {
                if (xmlNodeList3.Count > 1)
                    throw new CryptographicException("Invalid element: KeyInfo");
                foreach (XmlNode xmlNode in xmlNodeList3)
                {
                    XmlElement xmlElement2 = xmlNode as XmlElement;
                    if (xmlElement2 != null)
                        this.m_keyInfo.LoadXml(xmlElement2);
                }
                num3 += xmlNodeList3.Count;
            }
            XmlNodeList xmlNodeList4 = element1.SelectNodes("ds:Object", nsmgr);
            this.m_embeddedObjects.Clear();
            if (xmlNodeList4 != null)
            {
                foreach (XmlNode xmlNode in xmlNodeList4)
                {
                    XmlElement xmlElement2 = xmlNode as XmlElement;
                    if (xmlElement2 != null)
                    {
                        DataObject dataObject = new DataObject();
                        dataObject.LoadXml(xmlElement2);
                        this.m_embeddedObjects.Add((object) dataObject);
                    }
                }
                num3 += xmlNodeList4.Count;
            }
            XmlNodeList xmlNodeList5 = element1.SelectNodes("//*[@Id]", nsmgr);
            if (xmlNodeList5 != null)
            {
                foreach (XmlNode xmlNode in xmlNodeList5)
                    this.m_referencedItems.Add((object) xmlNode);
            }
            if (element1.SelectNodes("*").Count != num3)
                throw new CryptographicException("Invalid element: Signature (3)");
        }

        /// <summary>Adds a <see cref="T:System.Security.Cryptography.Xml.DataObject" /> to the list of objects to be signed.</summary>
        /// <param name="dataObject">The <see cref="T:System.Security.Cryptography.Xml.DataObject" /> to be added to the list of objects to be signed. </param>
        public void AddObject(DataObject dataObject)
        {
            this.m_embeddedObjects.Add((object) dataObject);
        }
    }
}