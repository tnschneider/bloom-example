using System;

namespace Bloom.Models
{
    [Flags]
    public enum HashFunction
    {
        Murmur = 1,
        Fnv = 1 << 1
    }
}