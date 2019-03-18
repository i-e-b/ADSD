using System.Security.Cryptography;
using System.Text;
using System.Xml;
using JetBrains.Annotations;

namespace ADSD.Crypto
{
    internal class CanonicalXmlAttribute : XmlAttribute, ICanonicalizableNode
    {
        public CanonicalXmlAttribute(
            string prefix,
            string localName,
            string namespaceURI,
            [NotNull]XmlDocument doc,
            bool defaultNodeSetInclusionState)
            : base(prefix, localName ?? "", namespaceURI, doc)
        {
            IsInNodeSet = defaultNodeSetInclusionState;
        }

        public bool IsInNodeSet { get; set; }

        public void Write(
            [NotNull]StringBuilder strBuilder,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            strBuilder.Append(" " + Name + "=\"");
            strBuilder.Append(Exml.EscapeAttributeValue(Value));
            strBuilder.Append("\"");
        }

        public void WriteHash(
            [NotNull]HashAlgorithm hash,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            UTF8Encoding utF8Encoding = new UTF8Encoding(false);
            byte[] bytes1 = utF8Encoding.GetBytes(" " + Name + "=\"");
            hash.TransformBlock(bytes1, 0, bytes1.Length, bytes1, 0);
            byte[] bytes2 = utF8Encoding.GetBytes(Exml.EscapeAttributeValue(Value));
            hash.TransformBlock(bytes2, 0, bytes2.Length, bytes2, 0);
            byte[] bytes3 = utF8Encoding.GetBytes("\"");
            hash.TransformBlock(bytes3, 0, bytes3.Length, bytes3, 0);
        }
    }
}