﻿namespace ADSD.Crypto
{
    /// <summary>Gets an object's <see cref="T:System.Security.Policy.Evidence" />.</summary>
    public interface IEvidenceFactory
    {
        /// <summary>Gets <see cref="T:System.Security.Policy.Evidence" /> that verifies the current object's identity.</summary>
        /// <returns>
        /// <see cref="T:System.Security.Policy.Evidence" /> of the current object's identity.</returns>
        Evidence Evidence { get; }
    }
}