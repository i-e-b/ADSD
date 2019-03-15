using System;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace ADSD
{
    [Serializable]
    internal sealed class LegacyEvidenceWrapper : EvidenceBase, ILegacyEvidenceAdapter
    {
        private object m_legacyEvidence;

        internal LegacyEvidenceWrapper(object legacyEvidence)
        {
            this.m_legacyEvidence = legacyEvidence;
        }

        public object EvidenceObject
        {
            get
            {
                return this.m_legacyEvidence;
            }
        }

        public Type EvidenceType
        {
            get
            {
                return this.m_legacyEvidence.GetType();
            }
        }

        public override bool Equals(object obj)
        {
            return this.m_legacyEvidence.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.m_legacyEvidence.GetHashCode();
        }

        [SecuritySafeCritical]
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override EvidenceBase Clone()
        {
            return base.Clone();
        }
    }
}