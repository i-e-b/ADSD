using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD
{
    internal class CanonicalXmlElement : XmlElement, ICanonicalizableNode
    {
        private bool m_isInNodeSet;

        public CanonicalXmlElement(
            string prefix,
            string localName,
            string namespaceURI,
            XmlDocument doc,
            bool defaultNodeSetInclusionState)
            : base(prefix, localName, namespaceURI, doc)
        {
            this.m_isInNodeSet = defaultNodeSetInclusionState;
        }

        public bool IsInNodeSet
        {
            get
            {
                return this.m_isInNodeSet;
            }
            set
            {
                this.m_isInNodeSet = value;
            }
        }

        public void Write(
            StringBuilder strBuilder,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            Hashtable nsLocallyDeclared = new Hashtable();
            SortedList sortedList = new SortedList((IComparer) new NamespaceSortOrder());
            SortedList attrListToRender = new SortedList((IComparer) new AttributeSortOrder());
            XmlAttributeCollection attributes = this.Attributes;
            if (attributes != null)
            {
                foreach (XmlAttribute attr in (XmlNamedNodeMap) attributes)
                {
                    if (((CanonicalXmlAttribute) attr).IsInNodeSet || Exml.IsNamespaceNode((XmlNode) attr) || Exml.IsXmlNamespaceNode((XmlNode) attr))
                    {
                        if (Exml.IsNamespaceNode((XmlNode) attr))
                            anc.TrackNamespaceNode(attr, sortedList, nsLocallyDeclared);
                        else if (Exml.IsXmlNamespaceNode((XmlNode) attr))
                            anc.TrackXmlNamespaceNode(attr, sortedList, attrListToRender, nsLocallyDeclared);
                        else if (this.IsInNodeSet)
                            attrListToRender.Add((object) attr, (object) null);
                    }
                }
            }
            if (!CanonicalXmlNodeList.IsCommittedNamespace((XmlElement) this, this.Prefix, this.NamespaceURI))
            {
                XmlAttribute attribute = this.OwnerDocument.CreateAttribute(this.Prefix.Length > 0 ? "xmlns:" + this.Prefix : "xmlns");
                attribute.Value = this.NamespaceURI;
                anc.TrackNamespaceNode(attribute, sortedList, nsLocallyDeclared);
            }
            if (this.IsInNodeSet)
            {
                anc.GetNamespacesToRender((XmlElement) this, attrListToRender, sortedList, nsLocallyDeclared);
                strBuilder.Append("<" + this.Name);
                foreach (object key in (IEnumerable) sortedList.GetKeyList())
                    (key as CanonicalXmlAttribute).Write(strBuilder, docPos, anc);
                foreach (object key in (IEnumerable) attrListToRender.GetKeyList())
                    (key as CanonicalXmlAttribute).Write(strBuilder, docPos, anc);
                strBuilder.Append(">");
            }
            anc.EnterElementContext();
            anc.LoadUnrenderedNamespaces(nsLocallyDeclared);
            anc.LoadRenderedNamespaces(sortedList);
            foreach (XmlNode childNode in this.ChildNodes)
                CanonicalizationDispatcher.Write(childNode, strBuilder, docPos, anc);
            anc.ExitElementContext();
            if (!this.IsInNodeSet)
                return;
            strBuilder.Append("</" + this.Name + ">");
        }

        public void WriteHash(
            HashAlgorithm hash,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            Hashtable nsLocallyDeclared = new Hashtable();
            SortedList sortedList = new SortedList((IComparer) new NamespaceSortOrder());
            SortedList attrListToRender = new SortedList((IComparer) new AttributeSortOrder());
            UTF8Encoding utF8Encoding = new UTF8Encoding(false);
            XmlAttributeCollection attributes = this.Attributes;
            if (attributes != null)
            {
                foreach (XmlAttribute attr in (XmlNamedNodeMap) attributes)
                {
                    if (((CanonicalXmlAttribute) attr).IsInNodeSet || Exml.IsNamespaceNode((XmlNode) attr) || Exml.IsXmlNamespaceNode((XmlNode) attr))
                    {
                        if (Exml.IsNamespaceNode((XmlNode) attr))
                            anc.TrackNamespaceNode(attr, sortedList, nsLocallyDeclared);
                        else if (Exml.IsXmlNamespaceNode((XmlNode) attr))
                            anc.TrackXmlNamespaceNode(attr, sortedList, attrListToRender, nsLocallyDeclared);
                        else if (this.IsInNodeSet)
                            attrListToRender.Add((object) attr, (object) null);
                    }
                }
            }
            if (!CanonicalXmlNodeList.IsCommittedNamespace((XmlElement) this, this.Prefix, this.NamespaceURI))
            {
                XmlAttribute attribute = this.OwnerDocument.CreateAttribute(this.Prefix.Length > 0 ? "xmlns:" + this.Prefix : "xmlns");
                attribute.Value = this.NamespaceURI;
                anc.TrackNamespaceNode(attribute, sortedList, nsLocallyDeclared);
            }
            if (this.IsInNodeSet)
            {
                anc.GetNamespacesToRender((XmlElement) this, attrListToRender, sortedList, nsLocallyDeclared);
                byte[] bytes1 = utF8Encoding.GetBytes("<" + this.Name);
                hash.TransformBlock(bytes1, 0, bytes1.Length, bytes1, 0);
                foreach (object key in (IEnumerable) sortedList.GetKeyList())
                    (key as CanonicalXmlAttribute).WriteHash(hash, docPos, anc);
                foreach (object key in (IEnumerable) attrListToRender.GetKeyList())
                    (key as CanonicalXmlAttribute).WriteHash(hash, docPos, anc);
                byte[] bytes2 = utF8Encoding.GetBytes(">");
                hash.TransformBlock(bytes2, 0, bytes2.Length, bytes2, 0);
            }
            anc.EnterElementContext();
            anc.LoadUnrenderedNamespaces(nsLocallyDeclared);
            anc.LoadRenderedNamespaces(sortedList);
            foreach (XmlNode childNode in this.ChildNodes)
                CanonicalizationDispatcher.WriteHash(childNode, hash, docPos, anc);
            anc.ExitElementContext();
            if (!this.IsInNodeSet)
                return;
            byte[] bytes = utF8Encoding.GetBytes("</" + this.Name + ">");
            hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
        }
    }
}