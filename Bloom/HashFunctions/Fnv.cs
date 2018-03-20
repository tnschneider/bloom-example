namespace Bloom.HashFunctions
{
    public static class Fnv
    {
        private const ulong _FNV_OFFSET_BASIS = 14695981039346656037;
        private const ulong _FNV_PRIME = 1099511628211;
        public static ulong Hash(byte[] bytes) 
        {
            var hash = _FNV_OFFSET_BASIS;

            foreach (var b in bytes)
            {
                hash *= _FNV_PRIME;
                hash ^= b;
            }
            return hash;
        }
    }
}