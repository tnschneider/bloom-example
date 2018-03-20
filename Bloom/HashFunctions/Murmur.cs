using System;
using System.Diagnostics;

namespace Bloom.HashFunctions
{
    public static class Murmur
    {
        private const uint _SEED = 5050;
        private const uint _C1 = 0xcc9e2d51;
        private const uint _C2 = 0x1b873593;
        private const uint _M = 5;
        private const uint _N = 0xe6546b64;
        private const uint _X1 = 0x85ebca6b;
        private const uint _X2 = 0xc2b2ae35;
        private const int _R1 = 15;
        private const int _R2 = 13;
        private const int _Y1 = 16;
        private const int _Y2 = 13;
        private static readonly bool _BIG_ENDIAN = !BitConverter.IsLittleEndian;
        private static readonly int[] _SHIFTS = !BitConverter.IsLittleEndian ? new[] { 0, 8, 16, 24 } : new[] { 24, 16, 8, 0 };

        public static ulong Hash(byte[] bytesToHash)
        {
            uint hash = _SEED;

            var (bytes, len) = Prepare(bytesToHash);
       
            int offset = 0;
            
            while (offset < len - (len % 4))
            {
                var k = BitConverter.ToUInt32(bytes, offset);

                k = k * _C1;

                k = ROL(k, _R1);

                k = k * _C2;

                hash ^= k;

                hash = ROL(hash, _R2);

                hash = (hash * _M) + _N;

                offset += 4;
            }

            if (len - offset > 0)
            {
                var rem = BitConverter.ToUInt32(bytes, offset);

                rem *= _C1;

                rem = ROL(rem, _R1);

                rem *= _C2;

                hash ^= rem;
            }

            hash ^= (uint)len;

            hash ^= hash >> _Y1;

            hash *= _X1;

            hash ^= hash >> _Y2;

            hash *= _X2;

            hash ^= hash >> _Y1;

            return hash;
        }

        private static (byte[] Bytes, int Length) Prepare(byte[] bytesToHash)
        {
            byte[] bytes;
            var len = bytesToHash.Length;

            var pad = (4 - (len % 4)) % 4;
            
            if (pad > 0)
            {
                bytes = new byte[len + pad];
                bytesToHash.CopyTo(bytes, 0);

                if (_BIG_ENDIAN) {
                    var last = len - 1;
                    byte a, b, c, d;
                    
                    a = bytes[last--];
                    b = bytes[last--];
                    c = bytes[last--];
                    d = bytes[last--];

                    bytes[last++] = a;
                    bytes[last++] = b;
                    bytes[last++] = c;
                    bytes[last++] = d;
                }

            }
            else 
            {
                bytes = bytesToHash;
            }

            return (bytes, len);
        }

        private static uint ROL(uint value, int bits)
        {
            return (value << bits) | (value >> (32 - bits));
        }
    }
}