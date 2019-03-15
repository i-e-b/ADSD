using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD
{
    internal class CanonicalXmlEntityReference : XmlEntityReference, ICanonicalizableNode
    {
        private bool m_isInNodeSet;

        public CanonicalXmlEntityReference(
            string name,
            XmlDocument doc,
            bool defaultNodeSetInclusionState)
            : base(name, doc)
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
            CanonicalizationDispatcher.WriteGenericNode((XmlNode) this, strBuilder, docPos, anc);
        }

        public void WriteHash(
            HashAlgorithm hash,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            if (!this.IsInNodeSet)
                return;
            CanonicalizationDispatcher.WriteHashGenericNode((XmlNode) this, hash, docPos, anc);
        }
    }
}