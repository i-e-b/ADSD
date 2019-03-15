using System;
using System.Security.Cryptography;

namespace ADSD
{
    internal sealed class Psha1DerivedKeyGenerator
    {
        private byte[] key;

        public Psha1DerivedKeyGenerator(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof (key));
            this.key = key;
        }

        public byte[] GenerateDerivedKey(byte[] label, byte[] nonce, int derivedKeySize, int position)
        {
            if (label == null)
                throw new ArgumentNullException(nameof (label));
            if (nonce == null)
                throw new ArgumentNullException(nameof (nonce));
            return new Psha1DerivedKeyGenerator.ManagedPsha1(this.key, label, nonce).GetDerivedKey(derivedKeySize, position);
        }

        private sealed class ManagedPsha1
        {
            private byte[] aValue;
            private byte[] buffer;
            private byte[] chunk;
            private KeyedHashAlgorithm hmac;
            private int index;
            private int position;
            private byte[] secret;
            private byte[] seed;

            public ManagedPsha1(byte[] secret, byte[] label, byte[] seed)
            {
                this.secret = secret;
                this.seed = new byte[checked (label.Length + seed.Length)];
                label.CopyTo((Array) this.seed, 0);
                seed.CopyTo((Array) this.seed, label.Length);
                this.aValue = this.seed;
                this.chunk = new byte[0];
                this.index = 0;
                this.position = 0;
                this.hmac = CryptoHelper.NewHmacSha1KeyedHashAlgorithm(secret);
                this.buffer = new byte[checked (unchecked (this.hmac.HashSize / 8) + this.seed.Length)];
            }

            public byte[] GetDerivedKey(int derivedKeySize, int position)
            {
                if (derivedKeySize < 0) throw new ArgumentOutOfRangeException(nameof(derivedKeySize), "ValueMustBeNonNegative");
                if (this.position > position) throw new ArgumentOutOfRangeException(nameof(position), "ValueMustBeInRange");

                while (this.position < position)
                {
                    int num = (int) this.GetByte();
                }
                int length = derivedKeySize / 8;
                byte[] numArray = new byte[length];
                for (int index = 0; index < length; ++index)
                    numArray[index] = this.GetByte();
                return numArray;
            }

            private byte GetByte()
            {
                if (this.index >= this.chunk.Length)
                {
                    this.hmac.Initialize();
                    this.aValue = this.hmac.ComputeHash(this.aValue);
                    this.aValue.CopyTo((Array) this.buffer, 0);
                    this.seed.CopyTo((Array) this.buffer, this.aValue.Length);
                    this.hmac.Initialize();
                    this.chunk = this.hmac.ComputeHash(this.buffer);
                    this.index = 0;
                }
                ++this.position;
                return this.chunk[this.index++];
            }
        }
    }
}