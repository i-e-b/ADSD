using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Xml;

namespace ADSD
{
    /// <summary>
    /// XML Reference
    /// </summary>
    public class Reference
    {
        private string m_id;
        private string m_uri;
        private string m_type;
        private TransformChain m_transformChain;
        private string m_digestMethod;
        private byte[] m_digestValue;
        private HashAlgorithm m_hashAlgorithm;
        private object m_refTarget;
        private ReferenceTargetType m_refTargetType;
        private XmlElement m_cachedXml;
        private SignedXml m_signedXml;
        internal CanonicalXmlNodeList m_namespaces;

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.Reference" /> class with default properties.</summary>
        public Reference()
        {
            this.m_transformChain = new TransformChain();
            this.m_refTarget = (object) null;
            this.m_refTargetType = ReferenceTargetType.UriReference;
            this.m_cachedXml = (XmlElement) null;
            this.m_digestMethod = SignedXml.XmlDsigDigestDefault;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.Reference" /> class with a hash value of the specified <see cref="T:System.IO.Stream" />.</summary>
        /// <param name="stream">The <see cref="T:System.IO.Stream" /> with which to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.Reference" />. </param>
        public Reference(Stream stream)
        {
            this.m_transformChain = new TransformChain();
            this.m_refTarget = (object) stream;
            this.m_refTargetType = ReferenceTargetType.Stream;
            this.m_cachedXml = (XmlElement) null;
            this.m_digestMethod = SignedXml.XmlDsigDigestDefault;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.Reference" /> class with the specified <see cref="T:System.Uri" />.</summary>
        /// <param name="uri">The <see cref="T:System.Uri" /> with which to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.Reference" />. </param>
        public Reference(string uri)
        {
            this.m_transformChain = new TransformChain();
            this.m_refTarget = (object) uri;
            this.m_uri = uri;
            this.m_refTargetType = ReferenceTargetType.UriReference;
            this.m_cachedXml = (XmlElement) null;
            this.m_digestMethod = SignedXml.XmlDsigDigestDefault;
        }

        internal Reference(XmlElement element)
        {
            this.m_transformChain = new TransformChain();
            this.m_refTarget = (object) element;
            this.m_refTargetType = ReferenceTargetType.XmlElement;
            this.m_cachedXml = (XmlElement) null;
            this.m_digestMethod = SignedXml.XmlDsigDigestDefault;
        }

        /// <summary>Gets or sets the ID of the current <see cref="T:System.Security.Cryptography.Xml.Reference" />.</summary>
        /// <returns>The ID of the current <see cref="T:System.Security.Cryptography.Xml.Reference" />. The default is <see langword="null" />.</returns>
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

        /// <summary>Gets or sets the <see cref="T:System.Uri" /> of the current <see cref="T:System.Security.Cryptography.Xml.Reference" />.</summary>
        /// <returns>The <see cref="T:System.Uri" /> of the current <see cref="T:System.Security.Cryptography.Xml.Reference" />.</returns>
        public string Uri
        {
            get
            {
                return this.m_uri;
            }
            set
            {
                this.m_uri = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the type of the object being signed.</summary>
        /// <returns>The type of the object being signed.</returns>
        public string Type
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

        /// <summary>Gets or sets the digest method Uniform Resource Identifier (URI) of the current <see cref="T:System.Security.Cryptography.Xml.Reference" />.</summary>
        /// <returns>The digest method URI of the current <see cref="T:System.Security.Cryptography.Xml.Reference" />. The default value is "http://www.w3.org/2000/09/xmldsig#sha1".</returns>
        public string DigestMethod
        {
            get
            {
                return this.m_digestMethod;
            }
            set
            {
                this.m_digestMethod = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets or sets the digest value of the current <see cref="T:System.Security.Cryptography.Xml.Reference" />.</summary>
        /// <returns>The digest value of the current <see cref="T:System.Security.Cryptography.Xml.Reference" />.</returns>
        public byte[] DigestValue
        {
            get
            {
                return this.m_digestValue;
            }
            set
            {
                this.m_digestValue = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        /// <summary>Gets the transform chain of the current <see cref="T:System.Security.Cryptography.Xml.Reference" />.</summary>
        /// <returns>The transform chain of the current <see cref="T:System.Security.Cryptography.Xml.Reference" />.</returns>
        public TransformChain TransformChain
        {
            get
            {
                if (this.m_transformChain == null)
                    this.m_transformChain = new TransformChain();
                return this.m_transformChain;
            }
            set
            {
                this.m_transformChain = value;
                this.m_cachedXml = (XmlElement) null;
            }
        }

        internal bool CacheValid
        {
            get
            {
                return this.m_cachedXml != null;
            }
        }

        internal SignedXml SignedXml
        {
            get
            {
                return this.m_signedXml;
            }
            set
            {
                this.m_signedXml = value;
            }
        }

        internal ReferenceTargetType ReferenceTargetType
        {
            get
            {
                return this.m_refTargetType;
            }
        }

        /// <summary>Returns the XML representation of the <see cref="T:System.Security.Cryptography.Xml.Reference" />.</summary>
        /// <returns>The XML representation of the <see cref="T:System.Security.Cryptography.Xml.Reference" />.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.Reference.DigestMethod" /> property is <see langword="null" />.-or- The <see cref="P:System.Security.Cryptography.Xml.Reference.DigestValue" /> property is <see langword="null" />. </exception>
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
            XmlElement element1 = document.CreateElement(nameof (Reference), "http://www.w3.org/2000/09/xmldsig#");
            if (!string.IsNullOrEmpty(this.m_id))
                element1.SetAttribute("Id", this.m_id);
            if (this.m_uri != null)
                element1.SetAttribute("URI", this.m_uri);
            if (!string.IsNullOrEmpty(this.m_type))
                element1.SetAttribute("Type", this.m_type);
            if (this.TransformChain.Count != 0)
                element1.AppendChild((XmlNode) this.TransformChain.GetXml(document, "http://www.w3.org/2000/09/xmldsig#"));
            if (string.IsNullOrEmpty(this.m_digestMethod))
                throw new CryptographicException("Cryptography exception: Digest method required");
            XmlElement element2 = document.CreateElement("DigestMethod", "http://www.w3.org/2000/09/xmldsig#");
            element2.SetAttribute("Algorithm", this.m_digestMethod);
            element1.AppendChild((XmlNode) element2);
            if (this.DigestValue == null)
            {
                if (this.m_hashAlgorithm?.Hash == null)
                    throw new CryptographicException("Cryptography exception: Digest hash value required");
                this.DigestValue = this.m_hashAlgorithm.Hash;
            }
            XmlElement element3 = document.CreateElement("DigestValue", "http://www.w3.org/2000/09/xmldsig#");
            element3.AppendChild((XmlNode) document.CreateTextNode(Convert.ToBase64String(this.m_digestValue)));
            element1.AppendChild((XmlNode) element3);
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
            foreach (XmlAttribute attribute in (XmlNamedNodeMap) element.Attributes)
            {
                bool flag = attribute.Name == "xmlns" || attribute.Name.StartsWith("xmlns:") || (attribute.Name == "xml:space" || attribute.Name == "xml:lang") || attribute.Name == "xml:base";
                for (int index = 0; !flag && expectedAttrNames != null && index < expectedAttrNames.Length; ++index)
                    flag = attribute.Name == expectedAttrNames[index];
                if (!flag)
                    return false;
            }
            return true;
        }
        
        internal static T CreateFromName<T>(string key) where T : class { return CryptoConfig.CreateFromName(key) as T; }

        /// <summary>Loads a <see cref="T:System.Security.Cryptography.Xml.Reference" /> state from an XML element.</summary>
        /// <param name="value">The XML element from which to load the <see cref="T:System.Security.Cryptography.Xml.Reference" /> state. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />. </exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="value" /> parameter does not contain any transforms.-or- The <paramref name="value" /> parameter contains an unknown transform. </exception>
        public void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof (value));
            this.m_id = GetAttribute(value, "Id", "http://www.w3.org/2000/09/xmldsig#");
            this.m_uri = GetAttribute(value, "URI", "http://www.w3.org/2000/09/xmldsig#");
            this.m_type = GetAttribute(value, "Type", "http://www.w3.org/2000/09/xmldsig#");
            if (!VerifyAttributes(value, "Id", "URI", "Type")) throw new CryptographicException("Cryptography exception: Invalid XML element");

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(value.OwnerDocument.NameTable);
            nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            bool flag = false;
            this.TransformChain = new TransformChain();
            XmlNodeList xmlNodeList1 = value.SelectNodes("ds:Transforms", nsmgr);
            if (xmlNodeList1 != null && xmlNodeList1.Count != 0)
            {
                flag = true;
                XmlElement element1 = xmlNodeList1[0] as XmlElement;
                if (!VerifyAttributes(element1))
                    throw new CryptographicException("Cryptography error: Invalid XML element: Reference/Transforms");

                XmlNodeList xmlNodeList2 = element1.SelectNodes("ds:Transform", nsmgr);
                if (xmlNodeList2 != null)
                {
                    if ((long) xmlNodeList2.Count > 10) throw new CryptographicException("Cryptography error: Invalid XML element: Reference/Transforms, too many transforms");

                    foreach (XmlNode xmlNode1 in xmlNodeList2)
                    {
                        XmlElement element2 = xmlNode1 as XmlElement;
                        string attribute = GetAttribute(element2, "Algorithm", "http://www.w3.org/2000/09/xmldsig#");
                        if (attribute == null || !VerifyAttributes(element2, "Algorithm"))
                            throw new CryptographicException("Cryptography error: Invalid XML element: Unknown transform");
                        Transform fromName = CreateFromName<Transform>(attribute);
                        if (fromName == null)
                            throw new CryptographicException("Cryptography error: Invalid XML element: Unknown transform (name)");

                        this.AddTransform(fromName);
                        fromName.LoadInnerXml(element2.ChildNodes);
                        if (fromName is XmlDsigEnvelopedSignatureTransform)
                        {
                            XmlNode xmlNode2 = element2.SelectSingleNode("ancestor::ds:Signature[1]", nsmgr);
                            XmlNodeList xmlNodeList3 = element2.SelectNodes("//ds:Signature", nsmgr);
                            if (xmlNodeList3 != null)
                            {
                                int num = 0;
                                foreach (XmlNode xmlNode3 in xmlNodeList3)
                                {
                                    ++num;
                                    if (xmlNode3 == xmlNode2)
                                    {
                                        ((XmlDsigEnvelopedSignatureTransform) fromName).SignaturePosition = num;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            XmlNodeList xmlNodeList4 = value.SelectNodes("ds:DigestMethod", nsmgr);
            if (xmlNodeList4 == null || xmlNodeList4.Count == 0) throw new CryptographicException("Cryptography error: XML has invalid element: Reference/DigestMethod (1)");

            XmlElement element3 = xmlNodeList4[0] as XmlElement;
            this.m_digestMethod = GetAttribute(element3, "Algorithm", "http://www.w3.org/2000/09/xmldsig#");
            if (this.m_digestMethod == null || !VerifyAttributes(element3, "Algorithm"))
                throw new CryptographicException("Cryptography error: XML has invalid element: Reference/DigestMethod (2)");
            XmlNodeList xmlNodeList5 = value.SelectNodes("ds:DigestValue", nsmgr);
            if (xmlNodeList5 == null || xmlNodeList5.Count == 0 || xmlNodeList5.Count > 1)
                throw new CryptographicException("Cryptography error: XML has invalid element: Reference/DigestValue (1)");
            XmlElement element4 = xmlNodeList5[0] as XmlElement;
            this.m_digestValue = Convert.FromBase64String(Exml.DiscardWhiteSpaces(element4.InnerText, 0, element4.InnerText.Length));
            if (!VerifyAttributes(element4))
                throw new CryptographicException("Cryptography error: XML has invalid element: Reference/DigestValue (2)");
            int num1 = flag ? 3 : 2;
            if (value.SelectNodes("*").Count != num1)
                throw new CryptographicException("Cryptography error: XML has invalid element: Reference/*");
            this.m_cachedXml = value;
        }
        
        
        internal static XmlDocument PreProcessElementInput(
            XmlElement elem,
            XmlResolver xmlResolver,
            string baseUri)
        {
            if (elem == null)
                throw new ArgumentNullException(nameof (elem));
            Exml exml = new Exml();
            exml.PreserveWhitespace = true;
            using (TextReader input = (TextReader) new StringReader(elem.OuterXml))
            {
                XmlReader reader = XmlReader.Create(input, new XmlReaderSettings()
                {
                    XmlResolver = xmlResolver,
                    DtdProcessing = DtdProcessing.Parse,
                    MaxCharactersFromEntities = 10000000L,
                    MaxCharactersInDocument = 0
                }, baseUri);
                exml.Load(reader);
            }
            return (XmlDocument) exml;
        }
        
        internal static XmlDocument PreProcessDocumentInput(
            XmlDocument document,
            XmlResolver xmlResolver,
            string baseUri)
        {
            if (document == null)
                throw new ArgumentNullException(nameof (document));
            Exml exml = new Exml();
            exml.PreserveWhitespace = document.PreserveWhitespace;
            using (TextReader input = (TextReader) new StringReader(document.OuterXml))
            {
                XmlReader reader = XmlReader.Create(input, new XmlReaderSettings()
                {
                    XmlResolver = xmlResolver,
                    DtdProcessing = DtdProcessing.Parse,
                    MaxCharactersFromEntities = 10000000L,
                    MaxCharactersInDocument = 0
                }, baseUri);
                exml.Load(reader);
            }
            return (XmlDocument) exml;
        }
        
        internal static string GetIdFromLocalUri(string uri, out bool discardComments)
        {
            string str = uri.Substring(1);
            discardComments = true;
            if (str.StartsWith("xpointer(id(", StringComparison.Ordinal))
            {
                int num1 = str.IndexOf("id(", StringComparison.Ordinal);
                int num2 = str.IndexOf(")", StringComparison.Ordinal);
                if (num2 < 0 || num2 < num1 + 3) throw new CryptographicException("Cryptography error; Invalid reference in XML ID");
                str = str.Substring(num1 + 3, num2 - num1 - 3).Replace("'", "").Replace("\"", "");
                discardComments = false;
            }
            return str;
        }

        internal static bool HasAttribute(XmlElement element, string localName, string namespaceURI)
        {
            if (!element.HasAttribute(localName))
                return element.HasAttribute(localName, namespaceURI);
            return true;
        }

        internal static XmlDocument DiscardComments(XmlDocument document)
        {
            XmlNodeList xmlNodeList = document.SelectNodes("//comment()");
            if (xmlNodeList != null)
            {
                foreach (XmlNode oldChild in xmlNodeList)
                    oldChild.ParentNode.RemoveChild(oldChild);
            }
            return document;
        }
        internal static void AddNamespaces(XmlElement elem, CanonicalXmlNodeList namespaces)
        {
            if (namespaces == null)
                return;
            foreach (XmlNode xmlNode in (XmlNodeList) namespaces)
            {
                string name = xmlNode.Prefix.Length > 0 ? xmlNode.Prefix + ":" + xmlNode.LocalName : xmlNode.LocalName;
                if (!elem.HasAttribute(name) && (!name.Equals("xmlns") || elem.Prefix.Length != 0))
                {
                    XmlAttribute attribute = elem.OwnerDocument.CreateAttribute(name);
                    attribute.Value = xmlNode.Value;
                    elem.SetAttributeNode(attribute);
                }
            }
        }

        /// <summary>Adds a <see cref="T:System.Security.Cryptography.Xml.Transform" /> object to the list of transforms to be performed on the data before passing it to the digest algorithm.</summary>
        /// <param name="transform">The transform to be added to the list of transforms. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="transform" /> parameter is <see langword="null" />.</exception>
        public void AddTransform(Transform transform)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof (transform));
            transform.Reference = this;
            this.TransformChain.Add(transform);
        }

        internal void UpdateHashValue(XmlDocument document, CanonicalXmlNodeList refList)
        {
            this.DigestValue = this.CalculateHashValue(document, refList);
        }

        internal byte[] CalculateHashValue(XmlDocument document, CanonicalXmlNodeList refList)
        {
            this.m_hashAlgorithm = CreateFromName<HashAlgorithm>(this.m_digestMethod);
            if (this.m_hashAlgorithm == null)
                throw new CryptographicException("Cryptography_Xml_CreateHashAlgorithmFailed");
            string str = document == null ? Environment.CurrentDirectory + "\\" : document.BaseURI;
            Stream stream = (Stream) null;
            WebResponse webResponse = (WebResponse) null;
            Stream input = (Stream) null;
            XmlResolver xmlResolver1 = (XmlResolver) null;
            try
            {
                switch (this.m_refTargetType)
                {
                    case ReferenceTargetType.Stream:
                        stream = this.TransformChain.TransformToOctetStream((Stream) this.m_refTarget, this.SignedXml.ResolverSet ? this.SignedXml.m_xmlResolver : (XmlResolver) new XmlSecureResolver((XmlResolver) new XmlUrlResolver(), str), str);
                        break;
                    case ReferenceTargetType.XmlElement:
                        XmlResolver xmlResolver2 = this.SignedXml.ResolverSet ? this.SignedXml.m_xmlResolver : (XmlResolver) new XmlSecureResolver((XmlResolver) new XmlUrlResolver(), str);
                        stream = this.TransformChain.TransformToOctetStream(PreProcessElementInput((XmlElement) this.m_refTarget, xmlResolver2, str), xmlResolver2, str);
                        break;
                    case ReferenceTargetType.UriReference:
                        if (this.m_uri == null)
                        {
                            stream = this.TransformChain.TransformToOctetStream((Stream) null, this.SignedXml.ResolverSet ? this.SignedXml.m_xmlResolver : (XmlResolver) new XmlSecureResolver((XmlResolver) new XmlUrlResolver(), str), str);
                            break;
                        }
                        if (this.m_uri.Length == 0)
                        {
                            if (document == null) throw new CryptographicException("Cryptography error: Document is null. XML self-reference requires context (1)");
                            XmlResolver xmlResolver3 = this.SignedXml.ResolverSet ? this.SignedXml.m_xmlResolver : (XmlResolver) new XmlSecureResolver((XmlResolver) new XmlUrlResolver(), str);
                            stream = this.TransformChain.TransformToOctetStream(DiscardComments(PreProcessDocumentInput(document, xmlResolver3, str)), xmlResolver3, str);
                            break;
                        }
                        if (this.m_uri[0] == '#')
                        {
                            bool discardComments = true;
                            string idFromLocalUri = GetIdFromLocalUri(this.m_uri, out discardComments);
                            if (idFromLocalUri == "xpointer(/)")
                            {
                                if (document == null) throw new CryptographicException("Cryptography error: Document is null. XML self-reference requires context (2)");
                                XmlResolver xmlResolver3 = this.SignedXml.ResolverSet ? this.SignedXml.m_xmlResolver : (XmlResolver) new XmlSecureResolver((XmlResolver) new XmlUrlResolver(), str);
                                stream = this.TransformChain.TransformToOctetStream(PreProcessDocumentInput(document, xmlResolver3, str), xmlResolver3, str);
                                break;
                            }
                            XmlElement elem = this.SignedXml.GetIdElement(document, idFromLocalUri);
                            if (elem != null)
                                this.m_namespaces = CanonicalXmlNodeList.GetPropagatedAttributes(elem.ParentNode as XmlElement);
                            if (elem == null && refList != null)
                            {
                                foreach (XmlNode xmlNode in (XmlNodeList) refList)
                                {
                                    XmlElement element = xmlNode as XmlElement;
                                    if (element != null && HasAttribute(element, "Id", "http://www.w3.org/2000/09/xmldsig#") && GetAttribute(element, "Id", "http://www.w3.org/2000/09/xmldsig#").Equals(idFromLocalUri))
                                    {
                                        elem = element;
                                        if (this.m_signedXml.m_context != null)
                                        {
                                            this.m_namespaces = CanonicalXmlNodeList.GetPropagatedAttributes(this.m_signedXml.m_context);
                                            break;
                                        }
                                        break;
                                    }
                                }
                            }
                            if (elem == null) throw new CryptographicException("Cryptography error: XML reference is invalid");
                            XmlDocument document1 = PreProcessElementInput(elem, xmlResolver1, str);
                            AddNamespaces(document1.DocumentElement, this.m_namespaces);
                            XmlResolver resolver = this.SignedXml.ResolverSet ? this.SignedXml.m_xmlResolver : (XmlResolver) new XmlSecureResolver((XmlResolver) new XmlUrlResolver(), str);
                            stream = !discardComments ? this.TransformChain.TransformToOctetStream(document1, resolver, str) : this.TransformChain.TransformToOctetStream(DiscardComments(document1), resolver, str);
                            break;
                        }
                        if (!GlobalSettings.AllowDetachedSignature) throw new CryptographicException($"Cryptography error: XML URL {m_uri} not resolved (1)");
                        System.Uri uri = new System.Uri(this.m_uri, UriKind.RelativeOrAbsolute);
                        if (!uri.IsAbsoluteUri)
                            uri = new System.Uri(new System.Uri(str), uri);
                        WebRequest webRequest = WebRequest.Create(uri);
                        if (webRequest != null)
                        {
                            webResponse = webRequest.GetResponse();
                            if (webResponse != null)
                            {
                                input = webResponse.GetResponseStream();
                                if (input != null)
                                {
                                    XmlResolver resolver = this.SignedXml.ResolverSet ? this.SignedXml.m_xmlResolver : (XmlResolver) new XmlSecureResolver((XmlResolver) new XmlUrlResolver(), str);
                                    stream = this.TransformChain.TransformToOctetStream(input, resolver, this.m_uri);
                                    break;
                                }
                                goto default;
                            }
                            else
                                goto default;
                        }
                        else
                            goto default;
                    default:
                        throw new CryptographicException($"Cryptography error: XML URL {m_uri} not resolved (2)");
                }
                return this.m_hashAlgorithm.ComputeHash(stream);
            }
            finally
            {
                stream?.Close();
                webResponse?.Close();
                input?.Close();
            }
        }
    }
}