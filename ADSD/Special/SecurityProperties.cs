

// ReSharper disable once CheckNamespace
namespace System.Security.Policy
{
    internal interface ILegacyEvidenceAdapter
    {
        object EvidenceObject { get; }

        Type EvidenceType { get; }
    }
    
    internal interface IDelayEvaluatedEvidence
    {
        bool IsVerified { [SecurityCritical] get; }

        bool WasUsed { get; }

        void MarkUsed();
    }
}