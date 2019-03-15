using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD
{
    
    internal class CanonicalXmlCDataSection : XmlCDataSection, ICanonicalizableNode
    {
        private bool m_isInNodeSet;

        public CanonicalXmlCDataSection(
            string data,
            XmlDocument doc,
            bool defaultNodeSetInclusionState)
            : base(data, doc)
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
            if (!this.IsInNodeSet)
                return;
            strBuilder.Append(Exml.EscapeTextData(this.Data));
        }

        public void WriteHash(
            HashAlgorithm hash,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            if (!this.IsInNodeSet)
                return;
            byte[] bytes = new UTF8Encoding(false).GetBytes(Exml.EscapeTextData(this.Data));
            hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
        }
    }

    internal class CanonicalXmlDocument : XmlDocument, ICanonicalizableNode
    {
        private bool m_defaultNodeSetInclusionState;
        private bool m_includeComments;
        private bool m_isInNodeSet;

        public CanonicalXmlDocument(bool defaultNodeSetInclusionState, bool includeComments)
        {
            this.PreserveWhitespace = true;
            this.m_includeComments = includeComments;
            this.m_isInNodeSet = this.m_defaultNodeSetInclusionState = defaultNodeSetInclusionState;
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
            docPos = DocPosition.BeforeRootElement;
            foreach (XmlNode childNode in this.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    CanonicalizationDispatcher.Write(childNode, strBuilder, DocPosition.InRootElement, anc);
                    docPos = DocPosition.AfterRootElement;
                }
                else
                    CanonicalizationDispatcher.Write(childNode, strBuilder, docPos, anc);
            }
        }

        public void WriteHash(
            HashAlgorithm hash,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            docPos = DocPosition.BeforeRootElement;
            foreach (XmlNode childNode in this.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    CanonicalizationDispatcher.WriteHash(childNode, hash, DocPosition.InRootElement, anc);
                    docPos = DocPosition.AfterRootElement;
                }
                else
                    CanonicalizationDispatcher.WriteHash(childNode, hash, docPos, anc);
            }
        }

        public override XmlElement CreateElement(
            string prefix,
            string localName,
            string namespaceURI)
        {
            return (XmlElement) new CanonicalXmlElement(prefix, localName, namespaceURI, (XmlDocument) this, this.m_defaultNodeSetInclusionState);
        }

        public override XmlAttribute CreateAttribute(
            string prefix,
            string localName,
            string namespaceURI)
        {
            return (XmlAttribute) new CanonicalXmlAttribute(prefix, localName, namespaceURI, (XmlDocument) this, this.m_defaultNodeSetInclusionState);
        }

        protected override XmlAttribute CreateDefaultAttribute(
            string prefix,
            string localName,
            string namespaceURI)
        {
            return (XmlAttribute) new CanonicalXmlAttribute(prefix, localName, namespaceURI, (XmlDocument) this, this.m_defaultNodeSetInclusionState);
        }

        public override XmlText CreateTextNode(string text)
        {
            return (XmlText) new CanonicalXmlText(text, (XmlDocument) this, this.m_defaultNodeSetInclusionState);
        }

        public override XmlWhitespace CreateWhitespace(string prefix)
        {
            return (XmlWhitespace) new CanonicalXmlWhitespace(prefix, (XmlDocument) this, this.m_defaultNodeSetInclusionState);
        }

        public override XmlSignificantWhitespace CreateSignificantWhitespace(
            string text)
        {
            return (XmlSignificantWhitespace) new CanonicalXmlSignificantWhitespace(text, (XmlDocument) this, this.m_defaultNodeSetInclusionState);
        }

        public override XmlProcessingInstruction CreateProcessingInstruction(
            string target,
            string data)
        {
            return (XmlProcessingInstruction) new CanonicalXmlProcessingInstruction(target, data, (XmlDocument) this, this.m_defaultNodeSetInclusionState);
        }

        public override XmlComment CreateComment(string data)
        {
            return (XmlComment) new CanonicalXmlComment(data, (XmlDocument) this, this.m_defaultNodeSetInclusionState, this.m_includeComments);
        }

        public override XmlEntityReference CreateEntityReference(string name)
        {
            return (XmlEntityReference) new CanonicalXmlEntityReference(name, (XmlDocument) this, this.m_defaultNodeSetInclusionState);
        }

        public override XmlCDataSection CreateCDataSection(string data)
        {
            return (XmlCDataSection) new CanonicalXmlCDataSection(data, (XmlDocument) this, this.m_defaultNodeSetInclusionState);
        }
    }
}