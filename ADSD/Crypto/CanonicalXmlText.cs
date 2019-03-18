using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD.Crypto
{
    internal class CanonicalXmlText : XmlText, ICanonicalizableNode
    {
        public CanonicalXmlText(string strData, XmlDocument doc, bool defaultNodeSetInclusionState)
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