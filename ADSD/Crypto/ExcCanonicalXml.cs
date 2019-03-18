using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD.Crypto
{
    internal class ExcCanonicalXml
    {
        private readonly CanonicalXmlDocument m_c14nDoc;
        private readonly ExcAncestralNamespaceContextManager m_ancMgr;

        internal ExcCanonicalXml(
            Stream inputStream,
            bool includeComments,
            string inclusiveNamespacesPrefixList,
            XmlResolver resolver,
            string strBaseUri)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof (inputStream));
            this.m_c14nDoc = new CanonicalXmlDocument(true, includeComments);
            this.m_c14nDoc.XmlResolver = resolver;
            this.m_c14nDoc.Load(Exml.PreProcessStreamInput(inputStream, resolver, strBaseUri));
            this.m_ancMgr = new ExcAncestralNamespaceContextManager(inclusiveNamespacesPrefixList);
        }

        internal ExcCanonicalXml(
            XmlDocument document,
            bool includeComments,
            string inclusiveNamespacesPrefixList,
            XmlResolver resolver)
        {
            if (document == null)
                throw new ArgumentNullException(nameof (document));
            this.m_c14nDoc = new CanonicalXmlDocument(true, includeComments);
            this.m_c14nDoc.XmlResolver = resolver;
            this.m_c14nDoc.Load((XmlReader) new XmlNodeReader((XmlNode) document));
            this.m_ancMgr = new ExcAncestralNamespaceContextManager(inclusiveNamespacesPrefixList);
        }

        internal ExcCanonicalXml(
            XmlNodeList nodeList,
            bool includeComments,
            string inclusiveNamespacesPrefixList,
            XmlResolver resolver)
        {
            if (nodeList == null)
                throw new ArgumentNullException(nameof (nodeList));
            XmlDocument ownerDocument = Exml.GetOwnerDocument(nodeList);
            if (ownerDocument == null)
                throw new ArgumentException(nameof (nodeList));
            this.m_c14nDoc = new CanonicalXmlDocument(false, includeComments);
            this.m_c14nDoc.XmlResolver = resolver;
            this.m_c14nDoc.Load((XmlReader) new XmlNodeReader((XmlNode) ownerDocument));
            this.m_ancMgr = new ExcAncestralNamespaceContextManager(inclusiveNamespacesPrefixList);
            ExcCanonicalXml.MarkInclusionStateForNodes(nodeList, ownerDocument, (XmlDocument) this.m_c14nDoc);
        }

        internal byte[] GetBytes()
        {
            StringBuilder strBuilder = new StringBuilder();
            this.m_c14nDoc.Write(strBuilder, DocPosition.BeforeRootElement, (AncestralNamespaceContextManager) this.m_ancMgr);
            return new UTF8Encoding(false).GetBytes(strBuilder.ToString());
        }

        internal byte[] GetDigestedBytes(HashAlgorithm hash)
        {
            this.m_c14nDoc.WriteHash(hash, DocPosition.BeforeRootElement, (AncestralNamespaceContextManager) this.m_ancMgr);
            hash.TransformFinalBlock(new byte[0], 0, 0);
            byte[] numArray = (byte[]) hash.Hash.Clone();
            hash.Initialize();
            return numArray;
        }

        private static void MarkInclusionStateForNodes(
            XmlNodeList nodeList,
            XmlDocument inputRoot,
            XmlDocument root)
        {
            CanonicalXmlNodeList canonicalXmlNodeList1 = new CanonicalXmlNodeList();
            CanonicalXmlNodeList canonicalXmlNodeList2 = new CanonicalXmlNodeList();
            canonicalXmlNodeList1.Add((object) inputRoot);
            canonicalXmlNodeList2.Add((object) root);
            int index1 = 0;
            do
            {
                XmlNode xmlNode1 = canonicalXmlNodeList1[index1];
                XmlNode xmlNode2 = canonicalXmlNodeList2[index1];
                XmlNodeList childNodes1 = xmlNode1.ChildNodes;
                XmlNodeList childNodes2 = xmlNode2.ChildNodes;
                for (int index2 = 0; index2 < childNodes1.Count; ++index2)
                {
                    canonicalXmlNodeList1.Add((object) childNodes1[index2]);
                    canonicalXmlNodeList2.Add((object) childNodes2[index2]);
                    if (Exml.NodeInList(childNodes1[index2], nodeList))
                        ExcCanonicalXml.MarkNodeAsIncluded(childNodes2[index2]);
                    XmlAttributeCollection attributes = childNodes1[index2].Attributes;
                    if (attributes != null)
                    {
                        for (int index3 = 0; index3 < attributes.Count; ++index3)
                        {
                            if (Exml.NodeInList((XmlNode) attributes[index3], nodeList))
                                ExcCanonicalXml.MarkNodeAsIncluded(childNodes2[index2].Attributes.Item(index3));
                        }
                    }
                }
                ++index1;
            }
            while (index1 < canonicalXmlNodeList1.Count);
        }

        private static void MarkNodeAsIncluded(XmlNode node)
        {
            if (!(node is ICanonicalizableNode))
                return;
            ((ICanonicalizableNode) node).IsInNodeSet = true;
        }
    }
}