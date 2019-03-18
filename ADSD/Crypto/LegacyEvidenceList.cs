using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace ADSD.Crypto
{
    [Serializable]
    internal sealed class LegacyEvidenceList : EvidenceBase, IEnumerable<EvidenceBase>, IEnumerable, ILegacyEvidenceAdapter
    {
        private readonly List<EvidenceBase> m_legacyEvidenceList = new List<EvidenceBase>();

        public object EvidenceObject
        {
            get
            {
                if (m_legacyEvidenceList.Count <= 0)
                    return (object) null;
                return (object) m_legacyEvidenceList[0];
            }
        }

        public Type EvidenceType
        {
            get
            {
                ILegacyEvidenceAdapter legacyEvidence = m_legacyEvidenceList[0] as ILegacyEvidenceAdapter;
                if (legacyEvidence != null)
                    return legacyEvidence.EvidenceType;
                return m_legacyEvidenceList[0].GetType();
            }
        }

        public void Add(EvidenceBase evidence)
        {
            m_legacyEvidenceList.Add(evidence);
        }

        public IEnumerator<EvidenceBase> GetEnumerator()
        {
            return (IEnumerator<EvidenceBase>) m_legacyEvidenceList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) m_legacyEvidenceList.GetEnumerator();
        }

        [SecuritySafeCritical]
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override EvidenceBase Clone()
        {
            return base.Clone();
        }
    }
}