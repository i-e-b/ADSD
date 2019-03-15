using System;
using System.Collections.Generic;
using System.Text;

namespace ADSD
{
    internal static class Asn1IntegerConverter
    {
        private static List<byte[]> powersOfTwo = new List<byte[]>((IEnumerable<byte[]>) new byte[1][]
        {
            new byte[1]{ (byte) 1 }
        });
        private static readonly char[] digitMap = new char[10]
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
            '9'
        };

        public static string Asn1IntegerToDecimalString(byte[] asn1)
        {
            if (asn1 == null) throw new ArgumentNullException(nameof (asn1));
            if (asn1.Length == 0) throw new ArgumentOutOfRangeException(nameof (asn1), "LengthOfArrayToConvertMustGreaterThanZero");
            List<byte> byteList1 = new List<byte>(asn1.Length * 8 / 3);
            int n = 0;
            for (int index1 = 0; index1 < asn1.Length - 1; ++index1)
            {
                byte num = asn1[index1];
                for (int index2 = 0; index2 < 8; ++index2)
                {
                    if (((int) num & 1) == 1)
                        Asn1IntegerConverter.AddSecondDecimalToFirst(byteList1, Asn1IntegerConverter.TwoToThePowerOf(n));
                    ++n;
                    num >>= 1;
                }
            }
            byte num1 = asn1[asn1.Length - 1];
            for (int index = 0; index < 7; ++index)
            {
                if (((int) num1 & 1) == 1)
                    Asn1IntegerConverter.AddSecondDecimalToFirst(byteList1, Asn1IntegerConverter.TwoToThePowerOf(n));
                ++n;
                num1 >>= 1;
            }
            StringBuilder stringBuilder = new StringBuilder(byteList1.Count + 1);
            List<byte> byteList2;
            if (num1 == (byte) 0)
            {
                byteList2 = byteList1;
            }
            else
            {
                List<byte> first = new List<byte>((IEnumerable<byte>) Asn1IntegerConverter.TwoToThePowerOf(n));
                Asn1IntegerConverter.SubtractSecondDecimalFromFirst(first, byteList1);
                byteList2 = first;
                stringBuilder.Append('-');
            }
            int index3 = byteList2.Count - 1;
            while (index3 >= 0 && byteList2[index3] == (byte) 0)
                --index3;
            if (index3 < 0 && asn1.Length != 0)
            {
                stringBuilder.Append(Asn1IntegerConverter.digitMap[0]);
            }
            else
            {
                while (index3 >= 0)
                    stringBuilder.Append(Asn1IntegerConverter.digitMap[(int) byteList2[index3--]]);
            }
            return stringBuilder.ToString();
        }

        private static byte[] TwoToThePowerOf(int n)
        {
            lock (Asn1IntegerConverter.powersOfTwo)
            {
                if (n >= Asn1IntegerConverter.powersOfTwo.Count)
                {
                    for (int count = Asn1IntegerConverter.powersOfTwo.Count; count <= n; ++count)
                    {
                        List<byte> byteList = new List<byte>((IEnumerable<byte>) Asn1IntegerConverter.powersOfTwo[count - 1]);
                        byte num1 = 0;
                        for (int index = 0; index < byteList.Count; ++index)
                        {
                            byte num2 = (byte) (((uint) byteList[index] << 1) + (uint) num1);
                            byteList[index] = (byte) ((uint) num2 % 10U);
                            num1 = (byte) ((uint) num2 / 10U);
                        }
                        if (num1 > (byte) 0)
                            byteList.Add(num1);
                        Asn1IntegerConverter.powersOfTwo.Add(byteList.ToArray());
                    }
                }
                return Asn1IntegerConverter.powersOfTwo[n];
            }
        }

        private static void AddSecondDecimalToFirst(List<byte> first, byte[] second)
        {
            byte num1 = 0;
            for (int index = 0; index < second.Length || index < first.Count; ++index)
            {
                if (index >= first.Count)
                    first.Add((byte) 0);
                byte num2 = index >= second.Length ? (byte) ((uint) first[index] + (uint) num1) : (byte) ((uint) first[index] + (uint) second[index] + (uint) num1);
                first[index] = (byte) ((uint) num2 % 10U);
                num1 = (byte) ((uint) num2 / 10U);
            }
            if (num1 <= (byte) 0)
                return;
            first.Add(num1);
        }

        private static void SubtractSecondDecimalFromFirst(List<byte> first, List<byte> second)
        {
            byte num1 = 0;
            for (int index = 0; index < second.Count; ++index)
            {
                int num2 = (int) first[index] - (int) second[index] - (int) num1;
                if (num2 < 0)
                {
                    num1 = (byte) 1;
                    first[index] = (byte) (num2 + 10);
                }
                else
                {
                    num1 = (byte) 0;
                    first[index] = (byte) num2;
                }
            }
            if (num1 <= (byte) 0)
                return;
            for (int count = second.Count; count < first.Count; ++count)
            {
                int num2 = (int) first[count] - (int) num1;
                if (num2 < 0)
                {
                    num1 = (byte) 1;
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