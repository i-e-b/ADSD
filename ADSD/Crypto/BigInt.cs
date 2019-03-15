using System;

namespace ADSD
{
    internal sealed class BigInt
    {
        private static readonly char[] decValues = new char[10]
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
        private byte[] m_elements;
        private const int m_maxbytes = 128;
        private const int m_base = 256;
        private int m_size;

        internal BigInt()
        {
            this.m_elements = new byte[128];
        }

        internal BigInt(byte b)
        {
            this.m_elements = new byte[128];
            this.SetDigit(0, b);
        }

        internal int Size
        {
            get
            {
                return this.m_size;
            }
            set
            {
                if (value > 128)
                    this.m_size = 128;
                if (value < 0)
                    this.m_size = 0;
                this.m_size = value;
            }
        }

        internal byte GetDigit(int index)
        {
            if (index < 0 || index >= this.m_size)
                return 0;
            return this.m_elements[index];
        }

        internal void SetDigit(int index, byte digit)
        {
            if (index < 0 || index >= 128)
                return;
            this.m_elements[index] = digit;
            if (index >= this.m_size && digit != (byte) 0)
                this.m_size = index + 1;
            if (index != this.m_size - 1 || digit != (byte) 0)
                return;
            --this.m_size;
        }

        internal void SetDigit(int index, byte digit, ref int size)
        {
            if (index < 0 || index >= 128)
                return;
            this.m_elements[index] = digit;
            if (index >= size && digit != (byte) 0)
                size = index + 1;
            if (index != size - 1 || digit != (byte) 0)
                return;
            --size;
        }

        public static bool operator <(BigInt value1, BigInt value2)
        {
            if (value1 == (BigInt) null)
                return true;
            if (value2 == (BigInt) null)
                return false;
            int size1 = value1.Size;
            int size2 = value2.Size;
            if (size1 != size2)
                return size1 < size2;
            while (size1-- > 0)
            {
                if ((int) value1.m_elements[size1] != (int) value2.m_elements[size1])
                    return (int) value1.m_elements[size1] < (int) value2.m_elements[size1];
            }
            return false;
        }

        public static bool operator >(BigInt value1, BigInt value2)
        {
            if (value1 == (BigInt) null)
                return false;
            if (value2 == (BigInt) null)
                return true;
            int size1 = value1.Size;
            int size2 = value2.Size;
            if (size1 != size2)
                return size1 > size2;
            while (size1-- > 0)
            {
                if ((int) value1.m_elements[size1] != (int) value2.m_elements[size1])
                    return (int) value1.m_elements[size1] > (int) value2.m_elements[size1];
            }
            return false;
        }

        public static bool operator ==(BigInt value1, BigInt value2)
        {
            if ((object) value1 == null)
                return (object) value2 == null;
            if ((object) value2 == null)
                return (object) value1 == null;
            int size1 = value1.Size;
            int size2 = value2.Size;
            if (size1 != size2)
                return false;
            for (int index = 0; index < size1; ++index)
            {
                if ((int) value1.m_elements[index] != (int) value2.m_elements[index])
                    return false;
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
            for (int index = 0; index < this.m_size; ++index)
                num += (int) this.GetDigit(index);
            return num;
        }

        internal static void Add(BigInt a, byte b, ref BigInt c)
        {
            byte digit = b;
            int size1 = a.Size;
            int size2 = 0;
            for (int index = 0; index < size1; ++index)
            {
                int num = (int) a.GetDigit(index) + (int) digit;
                c.SetDigit(index, (byte) (num & (int) byte.MaxValue), ref size2);
                digit = (byte) (num >> 8 & (int) byte.MaxValue);
            }
            if (digit != (byte) 0)
                c.SetDigit(a.Size, digit, ref size2);
            c.Size = size2;
        }

        internal static void Negate(ref BigInt a)
        {
            int size = 0;
            for (int index = 0; index < 128; ++index)
                a.SetDigit(index, (byte) ((uint) ~a.GetDigit(index) & (uint) byte.MaxValue), ref size);
            for (int index = 0; index < 128; ++index)
            {
                a.SetDigit(index, (byte) ((uint) a.GetDigit(index) + 1U), ref size);
                if (((int) a.GetDigit(index) & (int) byte.MaxValue) == 0)
                    a.SetDigit(index, (byte) ((uint) a.GetDigit(index) & (uint) byte.MaxValue), ref size);
                else
                    break;
            }
            a.Size = size;
        }

        internal static void Subtract(BigInt a, BigInt b, ref BigInt c)
        {
            byte num1 = 0;
            if (a < b)
            {
                BigInt.Subtract(b, a, ref c);
                BigInt.Negate(ref c);
            }
            else
            {
                int size1 = a.Size;
                int size2 = 0;
                for (int index = 0; index < size1; ++index)
                {
                    int num2 = (int) a.GetDigit(index) - (int) b.GetDigit(index) - (int) num1;
                    num1 = (byte) 0;
                    if (num2 < 0)
                    {
                        num2 += 256;
                        num1 = (byte) 1;
                    }
                    c.SetDigit(index, (byte) (num2 & (int) byte.MaxValue), ref size2);
                }
                c.Size = size2;
            }
        }

        private void Multiply(int b)
        {
            if (b == 0)
            {
                this.Clear();
            }
            else
            {
                int num1 = 0;
                int size1 = this.Size;
                int size2 = 0;
                for (int index = 0; index < size1; ++index)
                {
                    int num2 = b * (int) this.GetDigit(index) + num1;
                    num1 = num2 / 256;
                    this.SetDigit(index, (byte) (num2 % 256), ref size2);
                }
                if (num1 != 0)
                {
                    byte[] bytes = BitConverter.GetBytes(num1);
                    for (int index = 0; index < bytes.Length; ++index)
                        this.SetDigit(size1 + index, bytes[index], ref size2);
                }
                this.Size = size2;
            }
        }

        private static void Multiply(BigInt a, int b, ref BigInt c)
        {
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
                    int num2 = b * (int) a.GetDigit(index) + num1;
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
            int size1 = this.Size;
            int size2 = 0;
            while (size1-- > 0)
            {
                int num2 = 256 * num1 + (int) this.GetDigit(size1);
                num1 = num2 % b;
                this.SetDigit(size1, (byte) (num2 / b), ref size2);
            }
            this.Size = size2;
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
                quotient.SetDigit(0, (byte) 1);
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
                for (int index = 0; (long) index <= (long) num; ++index)
                {
                    int b = (c1.Size == a.Size ? (int) c1.GetDigit(c1.Size - 1) : 256 * (int) c1.GetDigit(c1.Size - 1) + (int) c1.GetDigit(c1.Size - 2)) / (int) a.GetDigit(a.Size - 1);
                    if (b >= 256)
                        b = (int) byte.MaxValue;
                    BigInt.Multiply(a, b, ref c2);
                    while (c2 > c1)
                    {
                        --b;
                        BigInt.Multiply(a, b, ref c2);
                    }
                    quotient.Multiply(256);
                    BigInt.Add(quotient, (byte) b, ref quotient);
                    BigInt.Subtract(c1, c2, ref c1);
                    a.Divide(256);
                }
                remainder.CopyFrom(c1);
            }
        }

        internal void CopyFrom(BigInt a)
        {
            Array.Copy((Array) a.m_elements, (Array) this.m_elements, 128);
            this.m_size = a.m_size;
        }

        internal bool IsZero()
        {
            for (int index = 0; index < this.m_size; ++index)
            {
                if (this.m_elements[index] != (byte) 0)
                    return false;
            }
            return true;
        }

        internal byte[] ToByteArray()
        {
            byte[] numArray = new byte[this.Size];
            Array.Copy((Array) this.m_elements, (Array) numArray, this.Size);
            return numArray;
        }

        internal void Clear()
        {
            this.m_size = 0;
        }
        internal static int GetHexArraySize(byte[] hex)
        {
            int length = hex.Length;
            do{ } while (length-- > 0 && hex[length] == (byte) 0);
            return length + 1;
        }

        internal void FromHexadecimal(string hexNum)
        {
            byte[] hex = X509Utils.DecodeHexString(hexNum);
            Array.Reverse((Array) hex);
            int hexArraySize = GetHexArraySize(hex);
            Array.Copy((Array) hex, (Array) this.m_elements, hexArraySize);
            this.Size = hexArraySize;
        }

        internal void FromDecimal(string decNum)
        {
            BigInt c1 = new BigInt();
            BigInt c2 = new BigInt();
            int length = decNum.Length;
            for (int index = 0; index < length; ++index)
            {
                if (decNum[index] <= '9' && decNum[index] >= '0')
                {
                    BigInt.Multiply(c1, 10, ref c2);
                    BigInt.Add(c2, (byte) ((uint) decNum[index] - 48U), ref c1);
                }
            }
            this.CopyFrom(c1);
        }

        internal string ToDecimal()
        {
            if (this.IsZero())
                return "0";
            BigInt denominator = new BigInt((byte) 10);
            BigInt numerator = new BigInt();
            BigInt quotient = new BigInt();
            BigInt remainder = new BigInt();
            numerator.CopyFrom(this);
            char[] chArray = new char[(int) Math.Ceiling((double) (this.m_size * 2) * 1.21)];
            int length = 0;
            do
            {
                BigInt.Divide(numerator, denominator, ref quotient, ref remainder);
                chArray[length++] = BigInt.decValues[remainder.IsZero() ? 0 : (int) remainder.m_elements[0]];
                numerator.CopyFrom(quotient);
            }
            while (!quotient.IsZero());
            Array.Reverse((Array) chArray, 0, length);
            return new string(chArray, 0, length);
        }
    }
}