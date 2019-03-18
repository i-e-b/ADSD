using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD.Crypto
{
    internal class CanonicalXmlSignificantWhitespace : XmlSignificantWhitespace, ICanonicalizableNode
    {
        public CanonicalXmlSignificantWhitespace(
            string strData,
            XmlDocument doc,
            bool defaultNodeSetInclusionState)
            : base(strData, doc)
        {
            this.IsInNodeSet = defaultNodeSetInclusionState;
        }

        public bool IsInNodeSet { get; set; }

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