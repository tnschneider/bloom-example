using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bloom.Models;
using Bloom.HashFunctions;
using HashFunc = System.Func<byte[], ulong>;

namespace Bloom
{
    public class BloomFilter
    {
        private readonly IEnumerable<HashFunc> _hashFunctions;
        private int _arraySize;
        private bool[] _array;
        private object _gate = new object();

        public BloomFilter(HashFunction hashFunctionFlags, int arraySize)
        {
            _hashFunctions = GetHashFunctions(hashFunctionFlags);
            _arraySize = arraySize;
            _array = new bool[arraySize];
        }

        public BloomFilter(HashFunction hashFunctionFlags, int estimatedValues, double targetFalsePositiveRate)
        {
            _hashFunctions = GetHashFunctions(hashFunctionFlags);
            _arraySize = GetOptimalM(_hashFunctions.Count(), estimatedValues, targetFalsePositiveRate);
            _array = new bool[_arraySize];
        }

        public int ArraySize => _arraySize;

        public void SetHash(ulong hashValue)
        {
            var i = (int)(hashValue % (ulong) _arraySize);
            lock(_gate)
            {
                _array[i] = true;
            }
        }

        public bool GetHash(ulong hashValue)
        {
            var i = (int)(hashValue % (ulong) _arraySize);
            bool isSet;
            lock(_gate)
            {
                isSet = _array[i];
            }
            return isSet;
        }

        public void Add(string value, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            
            var bytes = encoding.GetBytes(value); 

            _hashFunctions.AsParallel().ForAll(func => SetHash(func(bytes)));
        }

        public bool Contains(string value, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            
            var bytes = encoding.GetBytes(value); 

            return _hashFunctions.AsParallel().Select(func => GetHash(func(bytes))).All(x => x);
        }

        private IEnumerable<HashFunc> GetHashFunctions(HashFunction flags)
        {
            var res = new List<HashFunc>();

            if (flags.HasFlag(HashFunction.Fnv)) res.Add(Fnv.Hash);
            if (flags.HasFlag(HashFunction.Murmur)) res.Add(Murmur.Hash);

            return res;
        }

        private int GetOptimalM(int k, int n, double targetRate)
        {
            return Convert.ToInt32((-k*n)/(Math.Log(1 - Math.Pow(targetRate, 1.0/k))));
        }
    }
}