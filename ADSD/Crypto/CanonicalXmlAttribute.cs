using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD.Crypto
{
    internal class CanonicalXmlAttribute : XmlAttribute, ICanonicalizableNode
    {
        public CanonicalXmlAttribute(
            string prefix,
            string localName,
            string namespaceURI,
            XmlDocument doc,
            bool defaultNodeSetInclusionState)
            : base(prefix, localName, namespaceURI, doc)
        {
            this.IsInNodeSet = defaultNodeSetInclusionState;
        }

        public bool IsInNodeSet { get; set; }

        public void Write(
            StringBuilder strBuilder,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            strBuilder.Append(" " + this.Name + "=\"");
            strBuilder.Append(Exml.EscapeAttributeValue(this.Value));
            strBuilder.Append("\"");
        }

        public void WriteHash(
            HashAlgorithm hash,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            UTF8Encoding utF8Encoding = new UTF8Encoding(false);
            byte[] bytes1 = utF8Encoding.GetBytes(" " + this.Name + "=\"");
            hash.TransformBlock(bytes1, 0, bytes1.Length, bytes1, 0);
            byte[] bytes2 = utF8Encoding.GetBytes(Exml.EscapeAttributeValue(this.Value));
            hash.TransformBlock(bytes2, 0, bytes2.Length, bytes2, 0);
            byte[] bytes3 = utF8Encoding.GetBytes("\"");
            hash.TransformBlock(bytes3, 0, bytes3.Length, bytes3, 0);
        }
    }
}