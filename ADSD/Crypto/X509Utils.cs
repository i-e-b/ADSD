using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ADSD.Crypto
{
    internal class X509Utils
    {
        private static readonly char[] hexValues = new char[16]
        {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            'A',
            'B',
            'C',
            'D',
            'E',
            'F'
        };

        private X509Utils()
        {
        }

        internal static uint MapRevocationFlags(
            X509RevocationMode revocationMode,
            X509RevocationFlag revocationFlag)
        {
            uint num1 = 0;
            switch (revocationMode)
            {
                case X509RevocationMode.NoCheck:
                    return num1;
                case X509RevocationMode.Offline:
                    num1 |= 2147483648U;
                    break;
            }
            uint num2;
            switch (revocationFlag)
            {
                case X509RevocationFlag.EndCertificateOnly:
                    num2 = num1 | 268435456U;
                    break;
                case X509RevocationFlag.EntireChain:
                    num2 = num1 | 536870912U;
                    break;
                default:
                    num2 = num1 | 1073741824U;
                    break;
            }
            return num2;
        }

        internal static string EncodeHexString(byte[] sArray)
        {
            return X509Utils.EncodeHexString(sArray, 0U, (uint) sArray.Length);
        }

        internal static string EncodeHexString(byte[] sArray, uint start, uint end)
        {
            string str = (string) null;
            if (sArray != null)
            {
                char[] chArray1 = new char[((int) end - (int) start) * 2];
                uint num1 = start;
                uint num2 = 0;
                for (; num1 < end; ++num1)
                {
                    uint num3 = (uint) (((int) sArray[(int) num1] & 240) >> 4);
                    char[] chArray2 = chArray1;
                    int index1 = (int) num2;
                    uint num4 = (uint) (index1 + 1);
                    int hexValue1 = (int) X509Utils.hexValues[(int) num3];
                    chArray2[index1] = (char) hexValue1;
                    uint num5 = (uint) sArray[(int) num1] & 15U;
                    char[] chArray3 = chArray1;
                    int index2 = (int) num4;
                    num2 = (uint) (index2 + 1);
                    int hexValue2 = (int) X509Utils.hexValues[(int) num5];
                    chArray3[index2] = (char) hexValue2;
                }
                str = new string(chArray1);
            }
            return str;
        }

        internal static string EncodeHexStringFromInt(byte[] sArray)
        {
            return X509Utils.EncodeHexStringFromInt(sArray, 0U, (uint) sArray.Length);
        }

        internal static string EncodeHexStringFromInt(byte[] sArray, uint start, uint end)
        {
            string str = (string) null;
            if (sArray != null)
            {
                char[] chArray1 = new char[((int) end - (int) start) * 2];
                uint num1 = end;
                uint num2 = 0;
                while (num1-- > start)
                {
                    uint num3 = ((uint) sArray[(int) num1] & 240U) >> 4;
                    char[] chArray2 = chArray1;
                    int index1 = (int) num2;
                    uint num4 = (uint) (index1 + 1);
                    int hexValue1 = (int) X509Utils.hexValues[(int) num3];
                    chArray2[index1] = (char) hexValue1;
                    uint num5 = (uint) sArray[(int) num1] & 15U;
                    char[] chArray3 = chArray1;
                    int index2 = (int) num4;
                    num2 = (uint) (index2 + 1);
                    int hexValue2 = (int) X509Utils.hexValues[(int) num5];
                    chArray3[index2] = (char) hexValue2;
                }
                str = new string(chArray1);
            }
            return str;
        }

        internal static byte HexToByte(char val)
        {
            if (val <= '9' && val >= '0')
                return (byte) ((uint) val - 48U);
            if (val >= 'a' && val <= 'f')
                return (byte) ((int) val - 97 + 10);
            if (val >= 'A' && val <= 'F')
                return (byte) ((int) val - 65 + 10);
            return byte.MaxValue;
        }

        internal static string DiscardWhiteSpaces(string inputBuffer)
        {
            return DiscardWhiteSpaces(inputBuffer, 0, inputBuffer.Length);
        }

        internal static string DiscardWhiteSpaces(string inputBuffer, int inputOffset, int inputCount)
        {
            int num1 = 0;
            for (int index = 0; index < inputCount; ++index)
            {
                if (char.IsWhiteSpace(inputBuffer[inputOffset + index]))
                    ++num1;
            }
            char[] chArray = new char[inputCount - num1];
            int num2 = 0;
            for (int index = 0; index < inputCount; ++index)
            {
                if (!char.IsWhiteSpace(inputBuffer[inputOffset + index]))
                    chArray[num2++] = inputBuffer[inputOffset + index];
            }
            return new string(chArray);
        }

        internal static byte[] DecodeHexString(string s)
        {
            string str = DiscardWhiteSpaces(s);
            uint num = (uint) str.Length / 2U;
            byte[] numArray = new byte[(int) num];
            int index1 = 0;
            for (int index2 = 0; (long) index2 < (long) num; ++index2)
            {
                numArray[index2] = (byte) ((uint) X509Utils.HexToByte(str[index1]) << 4 | (uint) X509Utils.HexToByte(str[index1 + 1]));
                index1 += 2;
            }
            return numArray;
        }

        [SecurityCritical]
        internal static unsafe bool MemEqual(byte* pbBuf1, uint cbBuf1, byte* pbBuf2, uint cbBuf2)
        {
            if ((int) cbBuf1 != (int) cbBuf2)
                return false;
            while (cbBuf1-- > 0U)
            {
                if ((int) *pbBuf1++ != (int) *pbBuf2++)
                    return false;
            }
            return true;
        }

        internal static bool IsSelfSigned(X509Chain chain)
        {
            X509ChainElementCollection chainElements = chain.ChainElements;
            if (chainElements.Count != 1)
                return false;
            X509Certificate2 certificate = chainElements[0].Certificate;
            return string.Compare(certificate.SubjectName.Name, certificate.IssuerName.Name, StringComparison.OrdinalIgnoreCase) == 0;
        }


        /*
        [SecurityCritical]
        internal static X509Certificate2Collection GetCertificates(
            System.Security.Cryptography.SafeCertStoreHandle safeCertStoreHandle)
        {
            X509Certificate2Collection certificate2Collection = new X509Certificate2Collection();
            for (IntPtr index = CAPI.CertEnumCertificatesInStore(safeCertStoreHandle, IntPtr.Zero); index != IntPtr.Zero; index = CAPI.CertEnumCertificatesInStore(safeCertStoreHandle, index))
            {
                X509Certificate2 certificate = new X509Certificate2(index);
                certificate2Collection.Add(certificate);
            }
            return certificate2Collection;
        }

        [SecurityCritical]
        internal static unsafe int BuildChain(
            IntPtr hChainEngine,
            System.Security.Cryptography.SafeCertContextHandle pCertContext,
            X509Certificate2Collection extraStore,
            OidCollection applicationPolicy,
            OidCollection certificatePolicy,
            X509RevocationMode revocationMode,
            X509RevocationFlag revocationFlag,
            DateTime verificationTime,
            TimeSpan timeout,
            ref SafeCertChainHandle ppChainContext)
        {
            if (pCertContext == null || pCertContext.IsInvalid)
                throw new ArgumentException(SecurityResources.GetResourceString("Cryptography_InvalidContextHandle"), nameof (pCertContext));
            System.Security.Cryptography.SafeCertStoreHandle hAdditionalStore = System.Security.Cryptography.SafeCertStoreHandle.InvalidHandle;
            if (extraStore != null && extraStore.Count > 0)
                hAdditionalStore = X509Utils.ExportToMemoryStore(extraStore);
            CAPI.CERT_CHAIN_PARA pChainPara = new CAPI.CERT_CHAIN_PARA();
            pChainPara.cbSize = (uint) Marshal.SizeOf((object) pChainPara);
            SafeLocalAllocHandle localAllocHandle1 = SafeLocalAllocHandle.InvalidHandle;
            if (applicationPolicy != null && applicationPolicy.Count > 0)
            {
                pChainPara.RequestedUsage.dwType = 0U;
                pChainPara.RequestedUsage.Usage.cUsageIdentifier = (uint) applicationPolicy.Count;
                localAllocHandle1 = X509Utils.CopyOidsToUnmanagedMemory(applicationPolicy);
                pChainPara.RequestedUsage.Usage.rgpszUsageIdentifier = localAllocHandle1.DangerousGetHandle();
            }
            SafeLocalAllocHandle localAllocHandle2 = SafeLocalAllocHandle.InvalidHandle;
            if (certificatePolicy != null && certificatePolicy.Count > 0)
            {
                pChainPara.RequestedIssuancePolicy.dwType = 0U;
                pChainPara.RequestedIssuancePolicy.Usage.cUsageIdentifier = (uint) certificatePolicy.Count;
                localAllocHandle2 = X509Utils.CopyOidsToUnmanagedMemory(certificatePolicy);
                pChainPara.RequestedIssuancePolicy.Usage.rgpszUsageIdentifier = localAllocHandle2.DangerousGetHandle();
            }
            pChainPara.dwUrlRetrievalTimeout = (uint) timeout.Milliseconds;
            System.Runtime.InteropServices.ComTypes.FILETIME pTime = new System.Runtime.InteropServices.ComTypes.FILETIME();
            *(long*) &pTime = verificationTime.ToFileTime();
            uint dwFlags = X509Utils.MapRevocationFlags(revocationMode, revocationFlag);
            if (!CAPI.CAPISafe.CertGetCertificateChain(hChainEngine, pCertContext, ref pTime, hAdditionalStore, ref pChainPara, dwFlags, IntPtr.Zero, ref ppChainContext))
                return Marshal.GetHRForLastWin32Error();
            localAllocHandle1.Dispose();
            localAllocHandle2.Dispose();
            return 0;
        }*/

        [SecurityCritical]
        internal static unsafe int VerifyCertificate(
            object/*System.Security.Cryptography.SafeCertContextHandle*/ pCertContext,
            OidCollection applicationPolicy,
            OidCollection certificatePolicy,
            X509RevocationMode revocationMode,
            X509RevocationFlag revocationFlag,
            DateTime verificationTime,
            TimeSpan timeout,
            X509Certificate2Collection extraStore,
            IntPtr pszPolicy,
            IntPtr pdwErrorStatus)
        {/*
            if (pCertContext == null || pCertContext.IsInvalid)
                throw new ArgumentException(nameof (pCertContext));
            CAPI.CERT_CHAIN_POLICY_PARA pPolicyPara = new CAPI.CERT_CHAIN_POLICY_PARA(Marshal.SizeOf(typeof (CAPI.CERT_CHAIN_POLICY_PARA)));
            CAPI.CERT_CHAIN_POLICY_STATUS pPolicyStatus = new CAPI.CERT_CHAIN_POLICY_STATUS(Marshal.SizeOf(typeof (CAPI.CERT_CHAIN_POLICY_STATUS)));
            SafeCertChainHandle invalidHandle = SafeCertChainHandle.InvalidHandle;
            int num = X509Utils.BuildChain(new IntPtr(0L), pCertContext, extraStore, applicationPolicy, certificatePolicy, revocationMode, revocationFlag, verificationTime, timeout, ref invalidHandle);
            if (num != 0)
                return num;
            if (!CAPI.CAPISafe.CertVerifyCertificateChainPolicy(pszPolicy, invalidHandle, ref pPolicyPara, ref pPolicyStatus))
                return Marshal.GetHRForLastWin32Error();
            if (pdwErrorStatus != IntPtr.Zero)
                *(int*) (void*) pdwErrorStatus = (int) pPolicyStatus.dwError;
            return pPolicyStatus.dwError != 0U ? 1 : 0;*/
            return 0;
        }
    }
}