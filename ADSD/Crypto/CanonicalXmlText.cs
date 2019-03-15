using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD
{
    internal class CanonicalXmlText : XmlText, ICanonicalizableNode
    {
        private bool m_isInNodeSet;

        public CanonicalXmlText(string strData, XmlDocument doc, bool defaultNodeSetInclusionState)
            : base(strData, doc)
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
            strBuilder.Append(Exml.EscapeTextData(this.Value));
        }

        public void WriteHash(
            HashAlgorithm hash,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            if (!this.IsInNodeSet)
                return;
            byte[] bytes = new UTF8Encoding(false).GetBytes(Exml.EscapeTextData(this.Value));
            hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
        }
    }
}