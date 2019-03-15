using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

namespace ADSD
{
    /// <summary>Represents the enveloped signature transform for an XML digital signature as defined by the W3C.</summary>
    public class XmlDsigEnvelopedSignatureTransform : Transform
    {
        private readonly Type[] _inputTypes = { typeof (Stream), typeof (XmlNodeList), typeof (XmlDocument) };
        private readonly Type[] _outputTypes = { typeof (XmlNodeList), typeof (XmlDocument) };
        private XmlNodeList _inputNodeList;
        private XmlNamespaceManager _nsm;
        private XmlDocument _containingDocument;
        private int _signaturePosition;

        internal int SignaturePosition
        {
            set
            {
                _signaturePosition = value;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> class.</summary>
        public XmlDsigEnvelopedSignatureTransform()
        {
            Algorithm = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> class with comments, if specified.</summary>
        /// <param name="includeComments">
        /// <see langword="true" /> to include comments; otherwise, <see langword="false" />. </param>
        public XmlDsigEnvelopedSignatureTransform(bool includeComments)
        {
            Algorithm = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
        }

        // ReSharper disable once ConvertToAutoProperty
        /// <summary>Gets an array of types that are valid inputs to the <see cref="M:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</summary>
        /// <returns>An array of valid input types for the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object; you can pass only objects of one of these types to the <see cref="M:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</returns>
        public override Type[] InputTypes => _inputTypes;

        // ReSharper disable once ConvertToAutoProperty
        /// <summary>Gets an array of types that are possible outputs from the <see cref="M:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</summary>
        /// <returns>An array of valid output types for the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object; only objects of one of these types are returned from the <see cref="M:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</returns>
        public override Type[] OutputTypes => _outputTypes;

        /// <summary>Parses the specified <see cref="T:System.Xml.XmlNodeList" /> as transform-specific content of a <see langword="&lt;Transform&gt;" /> element and configures the internal state of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object to match the <see langword="&lt;Transform&gt;" /> element.</summary>
        /// <param name="nodeList">An <see cref="T:System.Xml.XmlNodeList" /> to load into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object. </param>
        public override void LoadInnerXml(XmlNodeList nodeList) { }

        /// <summary>Returns an XML representation of the parameters of an <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object that are suitable to be included as subelements of an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</summary>
        /// <returns>A list of the XML nodes that represent the transform-specific content needed to describe the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object in an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</returns>
        protected override XmlNodeList GetInnerXml() { return null; }
        
        internal static XmlReader PreProcessStreamInput( Stream inputStream, XmlResolver xmlResolver, string baseUri)
        {
            var xmlReaderSettings = new XmlReaderSettings
            {
                XmlResolver = xmlResolver,
                DtdProcessing = DtdProcessing.Parse,
                MaxCharactersFromEntities = 10000000L,
                MaxCharactersInDocument = 0
            };
            return XmlReader.Create(inputStream, xmlReaderSettings, baseUri);
        }

        /// <summary>Loads the specified input into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</summary>
        /// <param name="obj">The input to load into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="obj" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The containing XML document is <see langword="null" />.</exception>
        public override void LoadInput(object obj)
        {
            if (obj is Stream stream) LoadStreamInput(stream);
            else if (obj is XmlNodeList list) { LoadXmlNodeListInput(list); }
            else {
                if (!(obj is XmlDocument)) return;
                LoadXmlDocumentInput((XmlDocument) obj);
            }
        }
        internal static XmlDocument GetOwnerDocument(XmlNodeList nodeList)
        {
            foreach (XmlNode node in nodeList)
            {
                if (node.OwnerDocument != null)
                    return node.OwnerDocument;
            }
            return null;
        }

        private void LoadStreamInput(Stream stream)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            XmlResolver xmlResolver = ResolverSet ? m_xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), BaseURI);
            XmlReader reader = PreProcessStreamInput(stream, xmlResolver, BaseURI);
            xmlDocument.Load(reader);
            _containingDocument = xmlDocument;
            if (_containingDocument == null) throw new CryptographicException("Cryptography error: XML enveloped signature requires context");
            _nsm = new XmlNamespaceManager(_containingDocument.NameTable);
            _nsm.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
        }

        private void LoadXmlNodeListInput(XmlNodeList nodeList)
        {
            if (nodeList == null)
                throw new ArgumentNullException(nameof (nodeList));
            _containingDocument = GetOwnerDocument(nodeList);
            if (_containingDocument == null)
                throw new CryptographicException("Cryptography error: XML enveloped signature requires context");
            _nsm = new XmlNamespaceManager(_containingDocument.NameTable);
            _nsm.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
            _inputNodeList = nodeList;
        }

        private void LoadXmlDocumentInput(XmlDocument doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof (doc));
            _containingDocument = doc;
            _nsm = new XmlNamespaceManager(_containingDocument.NameTable);
            _nsm.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
        }
        internal static bool IsXmlNamespaceNode(XmlNode n)
        {
            if (n.NodeType == XmlNodeType.Attribute)
                return n.Prefix.Equals("xml");
            return false;
        }
        internal static bool IsNamespaceNode(XmlNode n)
        {
            if (n.NodeType != XmlNodeType.Attribute)
                return false;
            if (n.Prefix.Equals("xmlns"))
                return true;
            if (n.Prefix.Length == 0)
                return n.LocalName.Equals("xmlns");
            return false;
        }
        /// <summary>Returns the output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</summary>
        /// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</returns>
        /// <exception cref="T:System.Security.Cryptography.CryptographicException">The containing XML document is <see langword="null" />.</exception>
        public override object GetOutput()
        {
            if (_containingDocument == null)
                throw new CryptographicException("Cryptography error: XML enveloped signature requires context");
            if (_inputNodeList != null)
            {
                if (_signaturePosition == 0)
                    return (object) _inputNodeList;
                XmlNodeList xmlNodeList = _containingDocument.SelectNodes("//dsig:Signature", _nsm);
                if (xmlNodeList == null)
                    return (object) _inputNodeList;
                CanonicalXmlNodeList canonicalXmlNodeList = new CanonicalXmlNodeList();
                foreach (XmlNode inputNode in _inputNodeList)
                {
                    if (inputNode != null)
                    {
                        if (!IsXmlNamespaceNode(inputNode))
                        {
                            if (!IsNamespaceNode(inputNode))
                            {
                                try
                                {
                                    XmlNode xmlNode1 = inputNode.SelectSingleNode("ancestor-or-self::dsig:Signature[1]", _nsm);
                                    int num = 0;
                                    foreach (XmlNode xmlNode2 in xmlNodeList)
                                    {
                                        ++num;
                                        if (xmlNode2 == xmlNode1)
                                            break;
                                    }
                                    if (xmlNode1 != null)
                                    {
                                        if (xmlNode1 != null)
                                        {
                                            if (num == _signaturePosition)
                                                continue;
                                        }
                                        else
                                            continue;
                                    }
                                    canonicalXmlNodeList.Add((object) inputNode);
                                    continue;
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                        canonicalXmlNodeList.Add((object) inputNode);
                    }
                }
                return (object) canonicalXmlNodeList;
            }
            XmlNodeList xmlNodeList1 = _containingDocument.SelectNodes("//dsig:Signature", _nsm);
            if (xmlNodeList1 == null)
                return (object) _containingDocument;
            if (xmlNodeList1.Count < _signaturePosition || _signaturePosition <= 0)
                return (object) _containingDocument;
            xmlNodeList1[_signaturePosition - 1].ParentNode.RemoveChild(xmlNodeList1[_signaturePosition - 1]);
            return (object) _containingDocument;
        }
        
        internal static XmlNodeList AllDescendantNodes(XmlNode node, bool includeComments)
        {
            CanonicalXmlNodeList canonicalXmlNodeList1 = new CanonicalXmlNodeList();
            CanonicalXmlNodeList canonicalXmlNodeList2 = new CanonicalXmlNodeList();
            CanonicalXmlNodeList canonicalXmlNodeList3 = new CanonicalXmlNodeList();
            CanonicalXmlNodeList canonicalXmlNodeList4 = new CanonicalXmlNodeList();
            int index = 0;
            canonicalXmlNodeList2.Add((object) node);
            do
            {
                XmlNode xmlNode1 = canonicalXmlNodeList2[index];
                XmlNodeList childNodes = xmlNode1.ChildNodes;
                if (childNodes != null)
                {
                    foreach (XmlNode xmlNode2 in childNodes)
                    {
                        if (includeComments || !(xmlNode2 is XmlComment))
                            canonicalXmlNodeList2.Add((object) xmlNode2);
                    }
                }
                if (xmlNode1.Attributes != null)
                {
                    foreach (XmlNode attribute in (XmlNamedNodeMap) xmlNode1.Attributes)
                    {
                        if (attribute.LocalName == "xmlns" || attribute.Prefix == "xmlns")
                            canonicalXmlNodeList4.Add((object) attribute);
                        else
                            canonicalXmlNodeList3.Add((object) attribute);
                    }
                }
                ++index;
            }
            while (index < canonicalXmlNodeList2.Count);
            foreach (XmlNode xmlNode in (XmlNodeList) canonicalXmlNodeList2)
                canonicalXmlNodeList1.Add((object) xmlNode);
            foreach (XmlNode xmlNode in (XmlNodeList) canonicalXmlNodeList3)
                canonicalXmlNodeList1.Add((object) xmlNode);
            foreach (XmlNode xmlNode in (XmlNodeList) canonicalXmlNodeList4)
                canonicalXmlNodeList1.Add((object) xmlNode);
            return (XmlNodeList) canonicalXmlNodeList1;
        }

        /// <summary>Returns the output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object of type <see cref="T:System.Xml.XmlNodeList" />.</summary>
        /// <param name="type">The type of the output to return. <see cref="T:System.Xml.XmlNodeList" /> is the only valid type for this parameter. </param>
        /// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object of type <see cref="T:System.Xml.XmlNodeList" />.</returns>
        /// <exception cref="T:System.ArgumentException">The <paramref name="type" /> parameter is not an <see cref="T:System.Xml.XmlNodeList" /> object.</exception>
        public override object GetOutput(Type type)
        {
            if (type == typeof (XmlNodeList) || type.IsSubclassOf(typeof (XmlNodeList)))
            {
                if (_inputNodeList == null)
                    _inputNodeList = AllDescendantNodes((XmlNode) _containingDocument, true);
                return (object) (XmlNodeList) GetOutput();
            }
            if (!(type == typeof (XmlDocument)) && !type.IsSubclassOf(typeof (XmlDocument)))
                throw new ArgumentException("Cryptography error: XML Transform has an incorrect input type (1)");
            if (_inputNodeList != null)
                throw new ArgumentException("Cryptography error: XML Transform has an incorrect input type (1)");
            return (object) (XmlDocument) GetOutput();
        }
    }
}