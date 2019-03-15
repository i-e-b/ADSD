using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD
{
    internal class CanonicalXmlWhitespace : XmlWhitespace, ICanonicalizableNode
    {
        private bool m_isInNodeSet;

        public CanonicalXmlWhitespace(
            string strData,
            XmlDocument doc,
            bool defaultNodeSetInclusionState)
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
            if (!this.IsInNodeSet || docPos != DocPosition.InRootElement)
                return;
            strBuilder.Append(Exml.EscapeWhitespaceData(this.Value));
        }

        public void WriteHash(
            HashAlgorithm hash,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            if (!this.IsInNodeSet || docPos != DocPosition.InRootElement)
                return;
            byte[] bytes = new UTF8Encoding(false).GetBytes(Exml.EscapeWhitespaceData(this.Value));
            hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
        }
    }
}