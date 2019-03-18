using System;
using System.Collections.Generic;

namespace ADSD.Crypto
{
    internal interface IRuntimeEvidenceFactory
    {
        IEvidenceFactory Target { get; }

        IEnumerable<EvidenceBase> GetFactorySuppliedEvidence();

        EvidenceBase GenerateEvidence(Type evidenceType);
    }
}