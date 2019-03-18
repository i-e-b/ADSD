using System;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace ADSD.Crypto
{
    [Serializable]
    internal sealed class LegacyEvidenceWrapper : EvidenceBase, ILegacyEvidenceAdapter
    {
        internal LegacyEvidenceWrapper(object legacyEvidence)
        {
            EvidenceObject = legacyEvidence;
        }

        public object EvidenceObject { get; }

        public Type EvidenceType
        {
            get
            {
                return EvidenceObject.GetType();
            }
        }

        public override bool Equals(object obj)
        {
            return EvidenceObject.Equals(obj);
        }

        public override int GetHashCode()
        {
            return EvidenceObject.GetHashCode();
        }

        [SecuritySafeCritical]
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override EvidenceBase Clone()
        {
            return base.Clone();
        }
    }
}