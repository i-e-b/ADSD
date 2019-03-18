using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD.Crypto
{
    internal class CanonicalXmlComment : XmlComment, ICanonicalizableNode
    {
        public CanonicalXmlComment(
            string comment,
            XmlDocument doc,
            bool defaultNodeSetInclusionState,
            bool includeComments)
            : base(comment, doc)
        {
            this.IsInNodeSet = defaultNodeSetInclusionState;
            this.IncludeComments = includeComments;
        }

        public bool IsInNodeSet { get; set; }

        public bool IncludeComments { get; }

        public void Write(
            StringBuilder strBuilder,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            if (!this.IsInNodeSet || !this.IncludeComments)
                return;
            if (docPos == DocPosition.AfterRootElement)
                strBuilder.Append('\n');
            strBuilder.Append("<!--");
            strBuilder.Append(this.Value);
            strBuilder.Append("-->");
            if (docPos != DocPosition.BeforeRootElement)
                return;
            strBuilder.Append('\n');
        }

        public void WriteHash(
            HashAlgorithm hash,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            if (!this.IsInNodeSet || !this.IncludeComments)
                return;
            UTF8Encoding utF8Encoding = new UTF8Encoding(false);
            byte[] bytes1 = utF8Encoding.GetBytes("(char) 10");
            if (docPos == DocPosition.AfterRootElement)
                hash.TransformBlock(bytes1, 0, bytes1.Length, bytes1, 0);
            byte[] bytes2 = utF8Encoding.GetBytes("<!--");
            hash.TransformBlock(bytes2, 0, bytes2.Length, bytes2, 0);
            byte[] bytes3 = utF8Encoding.GetBytes(this.Value);
            hash.TransformBlock(bytes3, 0, bytes3.Length, bytes3, 0);
            byte[] bytes4 = utF8Encoding.GetBytes("-->");
            hash.TransformBlock(bytes4, 0, bytes4.Length, bytes4, 0);
            if (docPos != DocPosition.BeforeRootElement)
                return;
            byte[] bytes5 = utF8Encoding.GetBytes("(char) 10");
            hash.TransformBlock(bytes5, 0, bytes5.Length, bytes5, 0);
        }
    }
}