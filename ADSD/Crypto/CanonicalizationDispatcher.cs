using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace ADSD.Crypto
{
    internal class CanonicalizationDispatcher
    {
        private CanonicalizationDispatcher() { }

        public static void Write(
            XmlNode node,
            StringBuilder strBuilder,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            if (node is ICanonicalizableNode canonicalizableNode) canonicalizableNode.Write(strBuilder, docPos, anc);
            else WriteGenericNode(node, strBuilder, docPos, anc);
        }

        public static void WriteGenericNode(
            XmlNode node,
            StringBuilder strBuilder,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            if (node == null) throw new ArgumentNullException(nameof (node));
            foreach (XmlNode childNode in node.ChildNodes)
                Write(childNode, strBuilder, docPos, anc);
        }

        public static void WriteHash(
            XmlNode node,
            HashAlgorithm hash,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            if (node is ICanonicalizableNode)
                ((ICanonicalizableNode) node).WriteHash(hash, docPos, anc);
            else
                WriteHashGenericNode(node, hash, docPos, anc);
        }

        public static void WriteHashGenericNode(
            XmlNode node,
            HashAlgorithm hash,
            DocPosition docPos,
            AncestralNamespaceContextManager anc)
        {
            if (node == null)
                throw new ArgumentNullException(nameof (node));
            foreach (XmlNode childNode in node.ChildNodes)
                WriteHash(childNode, hash, docPos, anc);
        }
    }
}