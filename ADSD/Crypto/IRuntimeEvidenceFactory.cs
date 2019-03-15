using System;
using System.Collections.Generic;

namespace ADSD
{
    internal interface IRuntimeEvidenceFactory
    {
        IEvidenceFactory Target { get; }

        IEnumerable<EvidenceBase> GetFactorySuppliedEvidence();

        EvidenceBase GenerateEvidence(Type evidenceType);
    }
}