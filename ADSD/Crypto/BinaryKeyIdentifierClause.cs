using System;
using JetBrains.Annotations;

namespace ADSD.Crypto
{
    /// <summary>Represents a base class for key identifier clauses that are based upon binary data.</summary>
    public abstract class BinaryKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        [NotNull]private readonly byte[] identificationData;

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.BinaryKeyIdentifierClause" /> class using the specified key identifier clause type, binary data and a value that indicates whether the binary data must be cloned. </summary>
        /// <param name="clauseType">The key identifier clause type. Sets the value of the <see cref="P:System.IdentityModel.Tokens.SecurityKeyIdentifierClause.ClauseType" /> property.</param>
        /// <param name="identificationData">An array of <see cref="T:System.Byte" /> that contains the binary data that represents the key identifier.</param>
        /// <param name="cloneBuffer">
        /// <see langword="true" /> to clone the array passed into the <paramref name="identificationData" /> parameter; otherwise, <see langword="false" />. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="identificationData" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="identificationData" /> is zero length.</exception>
        protected BinaryKeyIdentifierClause(string clauseType, byte[] identificationData, bool cloneBuffer)
            : this(clauseType, identificationData, cloneBuffer, null, 0)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.BinaryKeyIdentifierClause" /> class using the specified key identifier clause type, binary data, a value that indicates whether the binary data must be cloned, a nonce and the key length.</summary>
        /// <param name="clauseType">The key identifier clause type. Sets the value of the <see cref="P:System.IdentityModel.Tokens.SecurityKeyIdentifierClause.ClauseType" /> property.</param>
        /// <param name="identificationData">An array of <see cref="T:System.Byte" /> that contains the binary data that represents the key identifier. Sets the binary data that is returned by the <see cref="M:System.IdentityModel.Tokens.BinaryKeyIdentifierClause.GetBuffer" /> method.</param>
        /// <param name="cloneBuffer">
        /// <see langword="true" /> to clone the array passed into the <paramref name="identificationData" /> parameter; otherwise, <see langword="false" />. </param>
        /// <param name="derivationNonce">An array of <see cref="T:System.Byte" /> that contains the nonce that was used to create a derived key. Sets the value that is returned by the <see cref="M:System.IdentityModel.Tokens.SecurityKeyIdentifierClause.GetDerivationNonce" /> method.</param>
        /// <param name="derivationLength">The size of the derived key. Sets the value of the <see cref="P:System.IdentityModel.Tokens.SecurityKeyIdentifierClause.DerivationLength" /> property.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="identificationData" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="identificationData" /> is zero length.</exception>
        protected BinaryKeyIdentifierClause(
            string clauseType,
            byte[] identificationData,
            bool cloneBuffer,
            byte[] derivationNonce,
            int derivationLength)
            : base(clauseType, derivationNonce, derivationLength)
        {
            if (identificationData == null) throw new ArgumentNullException(nameof (identificationData));
            if (identificationData.Length == 0) throw new ArgumentOutOfRangeException(nameof (identificationData));

            if (cloneBuffer) this.identificationData = CloneBuffer(identificationData);
            else this.identificationData = identificationData;
        }

        
        [NotNull]internal static byte[] CloneBuffer([NotNull]byte[] buffer)
        {
            return CloneBuffer(buffer, 0, buffer.Length);
        }
        [NotNull]internal static byte[] CloneBuffer([NotNull]byte[] buffer, int offset, int len)
        {
            byte[] numArray = new byte[len];
            Buffer.BlockCopy(buffer, offset, numArray, 0, len);
            return numArray;
        }

        /// <summary>Gets the binary data that represents the key identifier.</summary>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the binary data that represents the key identifier.</returns>
        public byte[] GetBuffer()
        {
            return CloneBuffer(identificationData);
        }

        /// <summary>Gets the binary data that represents the key identifier.</summary>
        /// <returns>An array of <see cref="T:System.Byte" /> that contains the binary data that represents the key identifier.</returns>
        protected byte[] GetRawBuffer()
        {
            return identificationData;
        }

        /// <summary>Returns a value that indicates whether the key identifier for this instance is equivalent to the specified key identifier clause.</summary>
        /// <param name="keyIdentifierClause">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> to compare to.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="keyIdentifierClause" /> is of type <see cref="T:System.IdentityModel.Tokens.BinaryKeyIdentifierClause" /> and the binary data returned by the <see cref="M:System.IdentityModel.Tokens.BinaryKeyIdentifierClause.GetBuffer" /> method is identical for the <paramref name="keyIdentifierClause" /> parameter and the current instance; otherwise, <see langword="false" />.</returns>
        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            BinaryKeyIdentifierClause identifierClause = keyIdentifierClause as BinaryKeyIdentifierClause;
            if (this == identifierClause)
                return true;
            if (identifierClause != null)
                return identifierClause.Matches(identificationData);
            return false;
        }

        /// <summary>Returns a value that indicates whether the binary data for the current instance matches the specified binary data.</summary>
        /// <param name="data">An array of <see cref="T:System.Byte" /> to compare to.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="data " />is equivalent to the binary data returned by the <see cref="M:System.IdentityModel.Tokens.BinaryKeyIdentifierClause.GetBuffer" /> method; otherwise, <see langword="false" />.</returns>
        public bool Matches(byte[] data)
        {
            return Matches(data, 0);
        }
        
        internal static bool MatchesBuffer(byte[] src, byte[] dst)
        {
            return MatchesBuffer(src, 0, dst, 0);
        }
        internal static bool MatchesBuffer(byte[] src, int srcOffset, byte[] dst, int dstOffset)
        {
            if (dstOffset < 0 || srcOffset < 0 || (src == null || srcOffset >= src.Length) || (dst == null || dstOffset >= dst.Length || src.Length - srcOffset != dst.Length - dstOffset))
                return false;
            int index1 = srcOffset;
            int index2 = dstOffset;
            while (index1 < src.Length)
            {
                if (src[index1] != dst[index2])
                    return false;
                ++index1;
                ++index2;
            }
            return true;
        }

        /// <summary>Returns a value that indicates whether the binary data for the current instance is equivalent to the specified binary data at the specified offset.</summary>
        /// <param name="data">An array of <see cref="T:System.Byte" /> to compare to.</param>
        /// <param name="offset">The index in the array at which the comparison starts.</param>
        /// <returns>
        /// <see langword="true" /> if the binary data in the <paramref name="data " />parameter starting at the index specified in the <paramref name="offset" /> parameter is equivalent to the binary data returned by the <see cref="M:System.IdentityModel.Tokens.BinaryKeyIdentifierClause.GetBuffer" /> method (starting at index zero); otherwise, <see langword="false" />.</returns>
        public bool Matches(byte[] data, int offset)
        {
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

            return MatchesBuffer(identificationData, 0, data, offset);
        }

        internal string ToBase64String()
        {
            return Convert.ToBase64String(identificationData);
        }

        internal string ToHexString()
        {
            return new SoapHexBinary(identificationData).ToString();
        }
    }
}