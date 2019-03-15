using System.Security.Cryptography;
using System.Text;

namespace ADSD
{
    internal interface ICanonicalizableNode
    {
        bool IsInNodeSet { get; set; }

        void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc);

        void WriteHash(HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc);
    }
}