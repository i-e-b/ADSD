using System;
using JetBrains.Annotations;

namespace ADSD.Crypto
{
    internal sealed class BigInt
    {
        [NotNull]private static readonly char[] decValues = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        [NotNull]private readonly byte[] m_elements;
        private int m_size;

        internal BigInt() { m_elements = new byte[128]; }

        internal BigInt(byte b)
        {
            m_elements = new byte[128];
            SetDigit(0, b);
        }

        internal int Size
        {
            get
            {
                return m_size;
            }
            set
            {
                if (value > 128) m_size = 128;
                if (value < 0) m_size = 0;
                m_size = value;
            }
        }

        internal byte GetDigit(int index)
        {
            if (index < 0 || index >= m_size) return 0;
            return m_elements[index];
        }

        internal void SetDigit(int index, byte digit)
        {
            if (index < 0 || index >= 128)
                return;
            m_elements[index] = digit;
            if (index >= m_size && digit != 0)
                m_size = index + 1;
            if (index != m_size - 1 || digit != 0)
                return;
            --m_size;
        }

        internal void SetDigit(int index, byte digit, ref int size)
        {
            if (index < 0 || index >= 128)
                return;
            m_elements[index] = digit;
            if (index >= size && digit != 0)
                size = index + 1;
            if (index != size - 1 || digit != 0)
                return;
            --size;
        }

        public static bool operator <(BigInt value1, BigInt value2)
        {
            if (value1 == null)
                return true;
            if (value2 == null)
                return false;
            int size1 = value1.Size;
            int size2 = value2.Size;
            if (size1 != size2)
                return size1 < size2;
            while (size1-- > 0)
            {
                if (value1.m_elements[size1] != value2.m_elements[size1])
                    return value1.m_elements[size1] < value2.m_elements[size1];
            }
            return false;
        }

        public static bool operator >(BigInt value1, BigInt value2)
        {
            if (value1 == null)
                return false;
            if (value2 == null)
                return true;
            int size1 = value1.Size;
            int size2 = value2.Size;
            if (size1 != size2)
                return size1 > size2;
            while (size1-- > 0)
            {
                if (value1.m_elements[size1] != value2.m_elements[size1])
                    return value1.m_elements[size1] > value2.m_elements[size1];
            }
            return false;
        }

        public static bool operator ==(BigInt value1, BigInt value2)
        {
            if ((object) value1 == null) return (object) value2 == null;
            if ((object) value2 == null) return false;

            int size1 = value1.Size;
            int size2 = value2.Size;
            if (size1 != size2) return false;

            for (int index = 0; index < size1; ++index)
            {
                if (value1.m_elements[index] != value2.m_elements[index]) return false;
            }
            return true;
        }

        public static bool operator !=(BigInt value1, BigInt value2)
        {
            return !(value1 == value2);
        }

        public override bool Equals(object obj)
        {
            if ((object) (obj as BigInt) != null)
                return this == (BigInt) obj;
            return false;
        }

        public override int GetHashCode()
        {
            int num = 0;
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            for (int index = 0; index < m_size; ++index) num += GetDigit(index);
            return num;
        }

        internal static void Add(BigInt a, byte b, ref BigInt c)
        {
            if (a == null) return;

            byte digit = b;
            int size1 = a.Size;
            int size2 = 0;
            for (int index = 0; index < size1; ++index)
            {
                int num = a.GetDigit(index) + digit;
                c.SetDigit(index, (byte) (num & byte.MaxValue), ref size2);
                digit = (byte) (num >> 8 & byte.MaxValue);
            }
            if (digit != 0) c.SetDigit(a.Size, digit, ref size2);
            c.Size = size2;
        }

        internal static void Negate(ref BigInt a)
        {
            int size = 0;
            for (int index = 0; index < 128; ++index)
                a.SetDigit(index, (byte) ((uint) ~a.GetDigit(index) & byte.MaxValue), ref size);
            for (int index = 0; index < 128; ++index)
            {
                a.SetDigit(index, (byte) (a.GetDigit(index) + 1U), ref size);
                if ((a.GetDigit(index) & byte.MaxValue) == 0)
                    a.SetDigit(index, (byte) (a.GetDigit(index) & (uint) byte.MaxValue), ref size);
                else
                    break;
            }
            a.Size = size;
        }

        internal static void Subtract(BigInt a, BigInt b, ref BigInt c)
        {
            if (a == null) return;
            if (b == null) return;

            byte num1 = 0;
            if (a < b)
            {
                Subtract(b, a, ref c);
                Negate(ref c);
            }
            else
            {
                int size1 = a.Size;
                int size2 = 0;
                for (int index = 0; index < size1; ++index)
                {
                    int num2 = a.GetDigit(index) - b.GetDigit(index) - num1;
                    num1 = 0;
                    if (num2 < 0)
                    {
                        num2 += 256;
                        num1 = 1;
                    }
                    c.SetDigit(index, (byte) (num2 & byte.MaxValue), ref size2);
                }
                c.Size = size2;
            }
        }

        private void Multiply(int b)
        {
            if (b == 0)
            {
                Clear();
            }
            else
            {
                int num1 = 0;
                int size1 = Size;
                int size2 = 0;
                for (int index = 0; index < size1; ++index)
                {
                    int num2 = b * GetDigit(index) + num1;
                    num1 = num2 / 256;
                    SetDigit(index, (byte) (num2 % 256), ref size2);
                }
                if (num1 != 0)
                {
                    byte[] bytes = BitConverter.GetBytes(num1);
                    for (int index = 0; index < bytes.Length; ++index)
                        SetDigit(size1 + index, bytes[index], ref size2);
                }
                Size = size2;
            }
        }

        private static void Multiply(BigInt a, int b, ref BigInt c)
        {
            if (a == null) return;
            if (c == null) return;

            if (b == 0)
            {
                c.Clear();
            }
            else
            {
                int num1 = 0;
                int size1 = a.Size;
                int size2 = 0;
                for (int index = 0; index < size1; ++index)
                {
                    int num2 = b * a.GetDigit(index) + num1;
                    num1 = num2 / 256;
                    c.SetDigit(index, (byte) (num2 % 256), ref size2);
                }
                if (num1 != 0)
                {
                    byte[] bytes = BitConverter.GetBytes(num1);
                    for (int index = 0; index < bytes.Length; ++index)
                        c.SetDigit(size1 + index, bytes[index], ref size2);
                }
                c.Size = size2;
            }
        }

        private void Divide(int b)
        {
            int num1 = 0;
            int size1 = Size;
            int size2 = 0;
            while (size1-- > 0)
            {
                int num2 = 256 * num1 + GetDigit(size1);
                num1 = num2 % b;
                SetDigit(size1, (byte) (num2 / b), ref size2);
            }
            Size = size2;
        }

        internal static void Divide(
            BigInt numerator,
            BigInt denominator,
            ref BigInt quotient,
            ref BigInt remainder)
        {
            if (numerator < denominator)
            {
                quotient.Clear();
                remainder.CopyFrom(numerator);
            }
            else if (numerator == denominator)
            {
                quotient.Clear();
                quotient.SetDigit(0, 1);
                remainder.Clear();
            }
            else
            {
                BigInt c1 = new BigInt();
                c1.CopyFrom(numerator);
                BigInt a = new BigInt();
                a.CopyFrom(denominator);
                uint num = 0;
                while (a.Size < c1.Size)
                {
                    a.Multiply(256);
                    ++num;
                }
                if (a > c1)
                {
                    a.Divide(256);
                    --num;
                }
                BigInt c2 = new BigInt();
                quotient.Clear();
                for (int index = 0; index <= num; ++index)
                {
                    int b = (c1.Size == a.Size ? c1.GetDigit(c1.Size - 1) : 256 * c1.GetDigit(c1.Size - 1) + c1.GetDigit(c1.Size - 2)) / a.GetDigit(a.Size - 1);
                    if (b >= 256)
                        b = byte.MaxValue;
                    Multiply(a, b, ref c2);
                    while (c2 > c1)
                    {
                        --b;
                        Multiply(a, b, ref c2);
                    }
                    quotient.Multiply(256);
                    Add(quotient, (byte) b, ref quotient);
                    Subtract(c1, c2, ref c1);
                    a.Divide(256);
                }
                remainder.CopyFrom(c1);
            }
        }

        internal void CopyFrom(BigInt a)
        {
            if (a == null) return;
            Array.Copy(a.m_elements, m_elements, 128);
            m_size = a.m_size;
        }

        internal bool IsZero()
        {
            for (int index = 0; index < m_size; ++index)
            {
                if (m_elements[index] != 0)
                    return false;
            }
            return true;
        }

        internal byte[] ToByteArray()
        {
            byte[] numArray = new byte[Size];
            Array.Copy(m_elements, numArray, Size);
            return numArray;
        }

        internal void Clear()
        {
            m_size = 0;
        }
        internal static int GetHexArraySize([NotNull]byte[] hex)
        {
            int length = hex.Length;
            do{ } while (length-- > 0 && hex[length] == 0);
            return length + 1;
        }

        internal void FromHexadecimal(string hexNum)
        {
            byte[] hex = X509Utils.DecodeHexString(hexNum);
            Array.Reverse(hex);
            int hexArraySize = GetHexArraySize(hex);
            Array.Copy(hex, m_elements, hexArraySize);
            Size = hexArraySize;
        }

        internal void FromDecimal(string decNum)
        {
            if (string.IsNullOrWhiteSpace(decNum)) throw new ArgumentNullException(nameof(decNum));
            var c1 = new BigInt();
            var c2 = new BigInt();
            int length = decNum.Length;
            for (int index = 0; index < length; ++index)
            {
                if (decNum[index] <= '9' && decNum[index] >= '0')
                {
                    Multiply(c1, 10, ref c2);
                    Add(c2, (byte) (decNum[index] - 48U), ref c1);
                }
            }
            CopyFrom(c1);
        }

        internal string ToDecimal()
        {
            if (IsZero()) return "0";
            BigInt denominator = new BigInt(10);
            BigInt numerator = new BigInt();
            BigInt quotient = new BigInt();
            BigInt remainder = new BigInt();
            numerator.CopyFrom(this);
            char[] chArray = new char[(int) Math.Ceiling(m_size * 2 * 1.21)];
            int length = 0;
            do
            {
                Divide(numerator, denominator, ref quotient, ref remainder);
                chArray[length++] = decValues[remainder.IsZero() ? 0 : remainder.m_elements[0]];
                numerator.CopyFrom(quotient);
            }
            while (!quotient.IsZero());
            Array.Reverse(chArray, 0, length);
            return new string(chArray, 0, length);
        }
    }
}