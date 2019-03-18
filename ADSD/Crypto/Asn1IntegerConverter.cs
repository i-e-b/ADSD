using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace ADSD.Crypto
{
    internal static class Asn1IntegerConverter
    {
        [NotNull]private static readonly List<byte[]> powersOfTwo = new List<byte[]>(new[] { new byte[]{ 1 } });
        [NotNull]private static readonly char[] digitMap = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static string Asn1IntegerToDecimalString(byte[] asn1)
        {
            if (asn1 == null) throw new ArgumentNullException(nameof (asn1));
            if (asn1.Length == 0) throw new ArgumentOutOfRangeException(nameof (asn1), "LengthOfArrayToConvertMustGreaterThanZero");
            var byteList1 = new List<byte>(asn1.Length * 8 / 3);
            var n = 0;
            for (int index1 = 0; index1 < asn1.Length - 1; ++index1)
            {
                byte num = asn1[index1];
                for (int index2 = 0; index2 < 8; ++index2)
                {
                    if ((num & 1) == 1) AddSecondDecimalToFirst(byteList1, TwoToThePowerOf(n));
                    ++n;
                    num >>= 1;
                }
            }
            byte num1 = asn1[asn1.Length - 1];
            for (int index = 0; index < 7; ++index)
            {
                if ((num1 & 1) == 1) AddSecondDecimalToFirst(byteList1, TwoToThePowerOf(n));
                ++n;
                num1 >>= 1;
            }
            var stringBuilder = new StringBuilder(byteList1.Count + 1);
            List<byte> byteList2;
            if (num1 == 0)
            {
                byteList2 = byteList1;
            }
            else
            {
                var first = new List<byte>(TwoToThePowerOf(n));
                SubtractSecondDecimalFromFirst(first, byteList1);
                byteList2 = first;
                stringBuilder.Append('-');
            }
            int index3 = byteList2.Count - 1;
            while (index3 >= 0 && byteList2[index3] == 0) --index3;
            if (index3 < 0 && asn1.Length != 0)
            {
                stringBuilder.Append(digitMap[0]);
            }
            else
            {
                while (index3 >= 0) stringBuilder.Append(digitMap[byteList2[index3--]]);
            }
            return stringBuilder.ToString();
        }

        [NotNull]private static byte[] TwoToThePowerOf(int n)
        {
            lock (powersOfTwo)
            {
                if (n >= powersOfTwo.Count)
                {
                    for (int count = powersOfTwo.Count; count <= n; ++count)
                    {
                        List<byte> byteList = new List<byte>(powersOfTwo[count - 1] ?? new byte[0]);
                        byte num1 = 0;
                        for (int index = 0; index < byteList.Count; ++index)
                        {
                            byte num2 = (byte) (((uint) byteList[index] << 1) + num1);
                            byteList[index] = (byte) (num2 % 10U);
                            num1 = (byte) (num2 / 10U);
                        }
                        if (num1 > 0)
                            byteList.Add(num1);
                        powersOfTwo.Add(byteList.ToArray());
                    }
                }
                return powersOfTwo[n] ?? new byte[0];
            }
        }

        private static void AddSecondDecimalToFirst(List<byte> first, byte[] second)
        {
            if (first == null) return;
            if (second == null) return;
            byte num1 = 0;
            for (int index = 0; index < second.Length || index < first.Count; ++index)
            {
                if (index >= first.Count) first.Add(0);
                byte num2 = index >= second.Length ? (byte) (first[index] + (uint) num1) : (byte) (first[index] + (uint) second[index] + num1);
                first[index] = (byte) (num2 % 10U);
                num1 = (byte) (num2 / 10U);
            }
            if (num1 <= 0) return;
            first.Add(num1);
        }

        private static void SubtractSecondDecimalFromFirst(List<byte> first, List<byte> second)
        {
            if (first == null) return;
            if (second == null) return;

            byte num1 = 0;
            for (int index = 0; index < second.Count; ++index)
            {
                int num2 = first[index] - second[index] - num1;
                if (num2 < 0)
                {
                    num1 = 1;
                    first[index] = (byte) (num2 + 10);
                }
                else
                {
                    num1 = 0;
                    first[index] = (byte) num2;
                }
            }
            if (num1 <= 0)
                return;
            for (int count = second.Count; count < first.Count; ++count)
            {
                int num2 = first[count] - num1;
                if (num2 < 0)
                {
                    num1 = 1;
                    first[count] = (byte) (num2 + 10);
                }
                else
                {
                    first[count] = (byte) num2;
                    break;
                }
            }
        }
    }
}