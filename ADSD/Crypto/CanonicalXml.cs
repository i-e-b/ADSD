using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using JetBrains.Annotations;

namespace ADSD.Crypto
{
    internal class CanonicalXml
    {
        [NotNull]private readonly CanonicalXmlDocument m_c14nDoc;
        [NotNull]private readonly C14NAncestralNamespaceContextManager m_ancMgr;

        internal CanonicalXml(
            Stream inputStream,
            bool includeComments,
            XmlResolver resolver,
            string strBaseUri)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof (inputStream));
            m_c14nDoc = new CanonicalXmlDocument(true, includeComments);
            m_c14nDoc.XmlResolver = resolver;
            m_c14nDoc.Load(PreProcessStreamInput(inputStream, resolver, strBaseUri));
            m_ancMgr = new C14NAncestralNamespaceContextManager();
        }

        internal CanonicalXml(XmlDocument document, XmlResolver resolver)
            : this(document, resolver, false)
        {
        }
        
        [NotNull]internal static XmlReader PreProcessStreamInput(
            [NotNull]Stream inputStream,
            XmlResolver xmlResolver,
            string baseUri)
        {
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
            {
                XmlResolver = xmlResolver,
                DtdProcessing = DtdProcessing.Parse,
                MaxCharactersFromEntities = 10000000L,
                MaxCharactersInDocument = 0
            };
            return XmlReader.Create(inputStream, xmlReaderSettings, baseUri);
        }

        internal CanonicalXml(XmlDocument document, XmlResolver resolver, bool includeComments)
        {
            if (document == null)
                throw new ArgumentNullException(nameof (document));
            m_c14nDoc = new CanonicalXmlDocument(true, includeComments);
            m_c14nDoc.XmlResolver = resolver;
            m_c14nDoc.Load(new XmlNodeReader(document));
            m_ancMgr = new C14NAncestralNamespaceContextManager();
        }

        internal CanonicalXml(XmlNodeList nodeList, XmlResolver resolver, bool includeComments)
        {
            if (nodeList == null)
                throw new ArgumentNullException(nameof (nodeList));
            XmlDocument ownerDocument = Exml.GetOwnerDocument(nodeList);
            if (ownerDocument == null)
                throw new ArgumentException(nameof (nodeList));
            m_c14nDoc = new CanonicalXmlDocument(false, includeComments);
            m_c14nDoc.XmlResolver = resolver;
            m_c14nDoc.Load(new XmlNodeReader(ownerDocument));
            m_ancMgr = new C14NAncestralNamespaceContextManager();
            MarkInclusionStateForNodes(nodeList, ownerDocument, m_c14nDoc);
        }

        private static void MarkNodeAsIncluded(XmlNode node)
        {
            if (!(node is ICanonicalizableNode))
                return;
            ((ICanonicalizableNode) node).IsInNodeSet = true;
        }

        private static void MarkInclusionStateForNodes(
            XmlNodeList nodeList,
            XmlDocument inputRoot,
            XmlDocument root)
        {
            CanonicalXmlNodeList canonicalXmlNodeList1 = new CanonicalXmlNodeList();
            CanonicalXmlNodeList canonicalXmlNodeList2 = new CanonicalXmlNodeList();
            canonicalXmlNodeList1.Add(inputRoot);
            canonicalXmlNodeList2.Add(root);
            int index1 = 0;
            do
            {
                XmlNode xmlNode1 = canonicalXmlNodeList1[index1] ?? throw new InvalidOperationException();
                XmlNode xmlNode2 = canonicalXmlNodeList2[index1] ?? throw new InvalidOperationException();
                XmlNodeList childNodes1 = xmlNode1.ChildNodes;
                XmlNodeList childNodes2 = xmlNode2.ChildNodes;
                for (int index2 = 0; index2 < childNodes1.Count; ++index2)
                {
                    canonicalXmlNodeList1.Add(childNodes1[index2]);
                    canonicalXmlNodeList2.Add(childNodes2[index2]);
                    if (Exml.NodeInList(childNodes1[index2], nodeList))
                        MarkNodeAsIncluded(childNodes2[index2]);
                    XmlAttributeCollection attributes = childNodes1[index2]?.Attributes;
                    if (attributes == null) continue;

                    for (int index3 = 0; index3 < attributes.Count; ++index3)
                    {
                        if (Exml.NodeInList(attributes[index3], nodeList))
                            MarkNodeAsIncluded(childNodes2[index2]?.Attributes?.Item(index3));
                    }
                }
                ++index1;
            }
            while (index1 < canonicalXmlNodeList1.Count);
        }

        internal byte[] GetBytes()
        {
            StringBuilder strBuilder = new StringBuilder();
            m_c14nDoc.Write(strBuilder, DocPosition.BeforeRootElement, m_ancMgr);
            return new UTF8Encoding(false).GetBytes(strBuilder.ToString());
        }

        internal byte[] GetDigestedBytes([NotNull]HashAlgorithm hash)
        {
            m_c14nDoc.WriteHash(hash, DocPosition.BeforeRootElement, m_ancMgr);
            hash.TransformFinalBlock(new byte[0], 0, 0);
            byte[] numArray = (byte[]) hash.Hash.Clone();
            hash.Initialize();
            return numArray;
        }
    }
}