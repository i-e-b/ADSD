using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml;

namespace ADSD.Crypto
{
    /// <summary>Contains information about the canonicalization algorithm and signature algorithm used for the XML signature.</summary>
    public class SignedInfo : ICollection
    {
        private string m_id;
        private string m_canonicalizationMethod;
        private string m_signatureMethod;
        private string m_signatureLength;
        private readonly ArrayList m_references;
        private XmlElement m_cachedXml;
        private Transform m_canonicalizationMethodTransform;

        internal SignedXml SignedXml { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> class.</summary>
        public SignedInfo()
        {
            this.m_references = new ArrayList();
        }

        /// <summary>Returns an enumerator that iterates through the collection of references.</summary>
        /// <returns>An enumerator that iterates through the collection of references.</returns>
        /// <exception cref="T:System.NotSupportedException">This method is not supported. </exception>
        public IEnumerator GetEnumerator()
        {
            throw new NotSupportedException();
        }

        /// <summary>Copies the elements of this instance into an <see cref="T:System.Array" /> object, starting at a specified index in the array.</summary>
        /// <param name="array">An <see cref="T:System.Array" /> object that holds the collection's elements. </param>
        /// <param name="index">The beginning index in the array where the elements are copied. </param>
        /// <exception cref="T:System.NotSupportedException">This method is not supported. </exception>
        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>Gets the number of references in the current <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object.</summary>
        /// <returns>The number of references in the current <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object.</returns>
        /// <exception cref="T:System.NotSupportedException">This property is not supported. </exception>
        public int Count
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>Gets a value that indicates whether the collection is read-only.</summary>
        /// <returns>
        /// <see langword="true" /> if the collection is read-only; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.NotSupportedException">This property is not supported. </exception>
        public bool IsReadOnly
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>Gets a value that indicates whether the collection is synchronized.</summary>
        /// <returns>
        /// <see langword="true" /> if the collection is synchronized; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.NotSupportedException">This property is not supported. </exception>
        public bool IsSynchronized
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>Gets an object to use for synchronization.</summary>
        /// <returns>An object to use for synchronization.</returns>
        /// <exception cref="T:System.NotSupportedException">This property is not supported. </exception>
        public object SyncRoot
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>Gets or sets the ID of the current <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object.</summary>
        /// <returns>The ID of the current <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object.</returns>
        public string Id
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

        /// <summary>Gets or sets the canonicalization algorithm that is used before signing for the current <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object.</summary>
        /// <returns>The canonicalization algorithm used before signing for the current <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object.</returns>
        public string CanonicalizationMethod
        {
            get
            {
                if (this.m_canonicalizationMethod == null)
                    return "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
                return this.m_canonicalizationMethod;
            }
            set
            {
                this.m_canonicalizationMethod = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets a <see cref="T:System.Security.Cryptography.Xml.Transform" /> object used for canonicalization.</summary>
        /// <returns>A <see cref="T:System.Security.Cryptography.Xml.Transform" /> object used for canonicalization.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">
        /// <see cref="T:System.Security.Cryptography.Xml.Transform" /> is <see langword="null" />.</exception>
        [ComVisible(false)]
        public Transform CanonicalizationMethodObject
        {
            get
            {
                if (this.m_canonicalizationMethodTransform == null)
                {
                    this.m_canonicalizationMethodTransform = CryptoConfig.CreateFromName(CanonicalizationMethod) as Transform;
                    if (this.m_canonicalizationMethodTransform == null) throw new CryptographicException("Failed to create transform for method = " + CanonicalizationMethod);
                    this.m_canonicalizationMethodTransform.SignedXml = this.SignedXml;
                    this.m_canonicalizationMethodTransform.Reference = (Reference) null;
                }
                return this.m_canonicalizationMethodTransform;
            }
        }

        /// <summary>Gets or sets the name of the algorithm used for signature generation and validation for the current <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object.</summary>
        /// <returns>The name of the algorithm used for signature generation and validation for the current <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object.</returns>
        public string SignatureMethod
        {
            get
            {
                return this.m_signatureMethod;
            }
            set
            {
                this.m_signatureMethod = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the length of the signature for the current <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object.</summary>
        /// <returns>The length of the signature for the current <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object.</returns>
        public string SignatureLength
        {
            get
            {
                return this.m_signatureLength;
            }
            set
            {
                this.m_signatureLength = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets a list of the <see cref="T:System.Security.Cryptography.Xml.Reference" /> objects of the current <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object.</summary>
        /// <returns>A list of the <see cref="T:System.Security.Cryptography.Xml.Reference" /> elements of the current <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object.</returns>
        public ArrayList References
        {
            get
            {
                return this.m_references;
            }
        }

        internal bool CacheValid
        {
            get
            {
                if (this.m_cachedXml == null)
                    return false;
                foreach (Reference reference in this.References)
                {
                    if (!reference.CacheValid)
                        return false;
                }
                return true;
            }
        }

        /// <summary>Returns the XML representation of the <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object.</summary>
        /// <returns>The XML representation of the <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> instance.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.SignedInfo.SignatureMethod" /> property is <see langword="null" />.-or- The <see cref="P:System.Security.Cryptography.Xml.SignedInfo.References" /> property is empty. </exception>
        public XmlElement GetXml()
        {
            if (this.CacheValid)
                return this.m_cachedXml;
            return this.GetXml(new XmlDocument()
            {
                PreserveWhitespace = true
            });
        }

        internal XmlElement GetXml(XmlDocument document)
        {
            XmlElement element1 = document.CreateElement(nameof (SignedInfo), "http://www.w3.org/2000/09/xmldsig#");
            if (!string.IsNullOrEmpty(this.m_id))
                element1.SetAttribute("Id", this.m_id);
            XmlElement xml = this.CanonicalizationMethodObject.GetXml(document, "CanonicalizationMethod");
            element1.AppendChild((XmlNode) xml);
            if (string.IsNullOrEmpty(this.m_signatureMethod))
                throw new CryptographicException("Signature method cequired");
            XmlElement element2 = document.CreateElement("SignatureMethod", "http://www.w3.org/2000/09/xmldsig#");
            element2.SetAttribute("Algorithm", this.m_signatureMethod);
            if (this.m_signatureLength != null)
            {
                XmlElement element3 = document.CreateElement((string) null, "HMACOutputLength", "http://www.w3.org/2000/09/xmldsig#");
                XmlText textNode = document.CreateTextNode(this.m_signatureLength);
                element3.AppendChild((XmlNode) textNode);
                element2.AppendChild((XmlNode) element3);
            }
            element1.AppendChild((XmlNode) element2);
            if (this.m_references.Count == 0)
                throw new CryptographicException("Reference element required");
            for (int index = 0; index < this.m_references.Count; ++index)
            {
                Reference reference = (Reference) this.m_references[index];
                element1.AppendChild((XmlNode) reference.GetXml(document));
            }
            return element1;
        }
        
        internal static string GetAttribute(XmlElement element, string localName, string namespaceURI)
        {
            string str = element.HasAttribute(localName) ? element.GetAttribute(localName) : (string) null;
            if (str == null && element.HasAttribute(localName, namespaceURI))
                str = element.GetAttribute(localName, namespaceURI);
            return str;
        }
        
        internal static bool VerifyAttributes(XmlElement element, params string[] expectedAttrNames)
        {
            if (GlobalSettings.SkipSignatureAttributeChecks) return true;

            foreach (XmlAttribute attribute in element.Attributes)
            {
                bool flag = attribute.Name == "xmlns" || attribute.Name.StartsWith("xmlns:") || (attribute.Name == "xml:space" || attribute.Name == "xml:lang") || attribute.Name == "xml:base";
                for (int index = 0; !flag && expectedAttrNames != null && index < expectedAttrNames.Length; ++index)
                    flag = attribute.Name == expectedAttrNames[index];
                if (!flag)
                    return false;
            }
            return true;
        }

        /// <summary>Loads a <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> state from an XML element.</summary>
        /// <param name="value">The XML element from which to load the <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> state. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />. </exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="value" /> parameter is not a valid <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> element.-or- The <paramref name="value" /> parameter does not contain a valid <see cref="P:System.Security.Cryptography.Xml.SignedInfo.CanonicalizationMethod" /> property.-or- The <paramref name="value" /> parameter does not contain a valid <see cref="P:System.Security.Cryptography.Xml.SignedInfo.SignatureMethod" /> property.</exception>
        public void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            XmlElement element1 = value;
            if (!element1.LocalName.Equals(nameof (SignedInfo)))
                throw new CryptographicException("XML invalid element: 'SignedInfo' (1)");
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            int num1 = 0;
            this.m_id = GetAttribute(element1, "Id", "http://www.w3.org/2000/09/xmldsig#");
            if (!VerifyAttributes(element1, "Id"))
                throw new CryptographicException("XML invalid element: 'SignedInfo' (2)");
            XmlNodeList xmlNodeList1 = element1.SelectNodes("ds:CanonicalizationMethod", nsmgr);
            if (xmlNodeList1 == null || xmlNodeList1.Count == 0 || xmlNodeList1.Count > 1)
                throw new CryptographicException("XML invalid element: SignedInfo/CanonicalizationMethod (1)");
            XmlElement element2 = xmlNodeList1.Item(0) as XmlElement;
            int num2 = num1 + xmlNodeList1.Count;
            this.m_canonicalizationMethod = GetAttribute(element2, "Algorithm", "http://www.w3.org/2000/09/xmldsig#");
            if (this.m_canonicalizationMethod == null && !GlobalSettings.SkipSignatureAttributeChecks || !VerifyAttributes(element2, "Algorithm"))
                throw new CryptographicException("XML invalid element: SignedInfo/CanonicalizationMethod (2)");
            this.m_canonicalizationMethodTransform = (Transform) null;
            if (element2.ChildNodes.Count > 0)
                this.CanonicalizationMethodObject.LoadInnerXml(element2.ChildNodes);
            XmlNodeList xmlNodeList2 = element1.SelectNodes("ds:SignatureMethod", nsmgr);
            if (xmlNodeList2 == null || xmlNodeList2.Count == 0 || xmlNodeList2.Count > 1)
                throw new CryptographicException("XML invalid element: SignedInfo/SignatureMethod (1)");
            XmlElement element3 = xmlNodeList2.Item(0) as XmlElement;
            int num3 = num2 + xmlNodeList2.Count;
            this.m_signatureMethod = GetAttribute(element3, "Algorithm", "http://www.w3.org/2000/09/xmldsig#");
            if (this.m_signatureMethod == null && !GlobalSettings.SkipSignatureAttributeChecks || !VerifyAttributes(element3, "Algorithm"))
                throw new CryptographicException("XML invalid element: SignedInfo/SignatureMethod (2)");
            XmlElement xmlElement1 = element3.SelectSingleNode("ds:HMACOutputLength", nsmgr) as XmlElement;
            if (xmlElement1 != null)
                this.m_signatureLength = xmlElement1.InnerXml;
            this.m_references.Clear();
            XmlNodeList xmlNodeList3 = element1.SelectNodes("ds:Reference", nsmgr);
            if (xmlNodeList3 != null)
            {
                if ((long) xmlNodeList3.Count > 10)
                    throw new CryptographicException("XML invalid element: SignedInfo/Reference");
                foreach (XmlNode xmlNode in xmlNodeList3)
                {
                    XmlElement xmlElement2 = xmlNode as XmlElement;
                    Reference reference = new Reference();
                    this.AddReference(reference);
                    reference.LoadXml(xmlElement2);
                }
                num3 += xmlNodeList3.Count;
            }
            if (element1.SelectNodes("*").Count != num3) throw new CryptographicException("XML invalid element: 'SignedInfo' (3)");
            this.m_cachedXml = element1;
        }

        /// <summary>Adds a <see cref="T:System.Security.Cryptography.Xml.Reference" /> object to the list of references to digest and sign.</summary>
        /// <param name="reference">The reference to add to the list of references. </param>
        /// <exception cref="T:System.ArgumentNullException">The reference parameter is <see langword="null" />.</exception>
        public void AddReference(Reference reference)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof (reference));
            reference.SignedXml = this.SignedXml;
            this.m_references.Add((object) reference);
        }
    }
}