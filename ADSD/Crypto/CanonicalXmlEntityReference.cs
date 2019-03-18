using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD.Crypto
{
    internal class CanonicalXmlEntityReference : XmlEntityReference, ICanonicalizableNode
    {
        public CanonicalXmlEntityReference(
            string name,
            XmlDocument doc,
            bool defaultNodeSetInclusionState)
            : base(name, doc)
        {
            this.IsInNodeSet = defaultNodeSetInclusionState;
        }

        public bool IsInNodeSet { get; set; }

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