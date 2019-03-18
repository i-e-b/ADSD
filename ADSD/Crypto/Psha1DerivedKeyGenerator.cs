using System;
using System.Security.Cryptography;

namespace ADSD.Crypto
{
    internal sealed class Psha1DerivedKeyGenerator
    {
        private readonly byte[] key;

        public Psha1DerivedKeyGenerator(byte[] key)
        {
            this.key = key ?? throw new ArgumentNullException(nameof (key));
        }

        public byte[] GenerateDerivedKey(byte[] label, byte[] nonce, int derivedKeySize, int position)
        {
            if (label == null)
                throw new ArgumentNullException(nameof (label));
            if (nonce == null)
                throw new ArgumentNullException(nameof (nonce));
            return new ManagedPsha1(key, label, nonce).GetDerivedKey(derivedKeySize, position);
        }

        private sealed class ManagedPsha1
        {
            private byte[] aValue;
            private readonly byte[] buffer;
            private byte[] chunk;
            private readonly KeyedHashAlgorithm hmac;
            private int index;
            private int position;
            private readonly byte[] seed;

            public ManagedPsha1(byte[] secret, byte[] label, byte[] seed)
            {
                this.seed = new byte[checked (label.Length + seed.Length)];
                label.CopyTo((Array) this.seed, 0);
                seed.CopyTo((Array) this.seed, label.Length);
                aValue = this.seed;
                chunk = new byte[0];
                index = 0;
                position = 0;
                hmac = CryptoHelper.NewHmacSha1KeyedHashAlgorithm(secret);
                buffer = new byte[checked (unchecked (hmac.HashSize / 8) + this.seed.Length)];
            }

            public byte[] GetDerivedKey(int derivedKeySize, int position)
            {
                if (derivedKeySize < 0) throw new ArgumentOutOfRangeException(nameof(derivedKeySize), "ValueMustBeNonNegative");
                if (this.position > position) throw new ArgumentOutOfRangeException(nameof(position), "ValueMustBeInRange");

                while (this.position < position)
                {
                    int num = (int) GetByte();
                }
                int length = derivedKeySize / 8;
                byte[] numArray = new byte[length];
                for (int index = 0; index < length; ++index)
                    numArray[index] = GetByte();
                return numArray;
            }

            private byte GetByte()
            {
                if (index >= chunk.Length)
                {
                    hmac.Initialize();
                    aValue = hmac.ComputeHash(aValue);
                    aValue.CopyTo((Array) buffer, 0);
                    seed.CopyTo((Array) buffer, aValue.Length);
                    hmac.Initialize();
                    chunk = hmac.ComputeHash(buffer);
                    index = 0;
                }
                ++position;
                return chunk[index++];
            }
        }
    }
}