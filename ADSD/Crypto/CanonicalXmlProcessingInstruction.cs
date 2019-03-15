using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD
{
    internal class CanonicalXmlProcessingInstruction : XmlProcessingInstruction, ICanonicalizableNode
    {
        private bool m_isInNodeSet;

        public CanonicalXmlProcessingInstruction(
            string target,
            string data,
            XmlDocument doc,
            bool defaultNodeSetInclusionState)
            : base(target, data, doc)
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
            if (docPos == DocPosition.AfterRootElement)
                strBuilder.Append('\n');
            strBuilder.Append("<?");
            strBuilder.Append(this.Name);
            if (this.Value != null && this.Value.Length > 0)
                strBuilder.Append(" " + this.Value);
            strBuilder.Append("?>");
            if (docPos != DocPosition.BeforeRootElement)
                return;
            strBuilder.Append('\n');
        }

        public void WriteHash(
            HashAlgorithm hash,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            if (!this.IsInNodeSet)
                return;
            UTF8Encoding utF8Encoding = new UTF8Encoding(false);
            if (docPos == DocPosition.AfterRootElement)
            {
                byte[] bytes = utF8Encoding.GetBytes("(char) 10");
                hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
            }
            byte[] bytes1 = utF8Encoding.GetBytes("<?");
            hash.TransformBlock(bytes1, 0, bytes1.Length, bytes1, 0);
            byte[] bytes2 = utF8Encoding.GetBytes(this.Name);
            hash.TransformBlock(bytes2, 0, bytes2.Length, bytes2, 0);
            if (this.Value != null && this.Value.Length > 0)
            {
                byte[] bytes3 = utF8Encoding.GetBytes(" " + this.Value);
                hash.TransformBlock(bytes3, 0, bytes3.Length, bytes3, 0);
            }
            byte[] bytes4 = utF8Encoding.GetBytes("?>");
            hash.TransformBlock(bytes4, 0, bytes4.Length, bytes4, 0);
            if (docPos != DocPosition.BeforeRootElement)
                return;
            byte[] bytes5 = utF8Encoding.GetBytes("(char) 10");
            hash.TransformBlock(bytes5, 0, bytes5.Length, bytes5, 0);
        }
    }
}