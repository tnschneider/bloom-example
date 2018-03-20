using System;
using Bloom.HashFunctions;
using System.Text;
using Bloom;
using Bloom.Models;
using System.Diagnostics;
using System.Collections;

namespace Bloom.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var filter = new BloomFilter(HashFunction.Fnv | HashFunction.Murmur, 10000000, 0.0001d);

            Console.WriteLine(filter.ArraySize);

            var test = "Hello my name is Terry";

            Timed(() => filter.Add(test)).Speak();
            Timed(() => filter.Add(test)).Speak();
            Timed(() => filter.Add(test)).Speak();
            Timed(() => filter.Add(test)).Speak();
            Timed(() => filter.Add(test)).Speak();
            

            Timed(() => filter.Contains(test)).Speak();

            var test2 = "Hello my name is Bob";

            Timed(() => filter.Contains(test2)).Speak();

            Timed(() => filter.Add(test2)).Speak();

            Timed(() => filter.Contains(test2)).Speak();

            Timed(() => filter.Add("an boobus")).Speak();
        }

        public static TimedResult<T> Timed<T>(Func<T> function)
        {
            var watch = new Stopwatch();
            watch.Start();
            var result = function();
            watch.Stop();
            
            return new TimedResult<T>
            {
                Result = result,
                Time = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds)
            };
        }

        public static TimedResult<object> Timed(Action action)
        {
            var watch = new Stopwatch();
            watch.Start();
            action();
            watch.Stop();
            
            return new TimedResult<object>
            {
                Result = null,
                Time = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds)
            };
        }

        public class TimedResult<T> 
        {
            public T Result {get;set;}
            public TimeSpan Time {get;set;}

            public void Speak()
            {
                var result = Result == null ? "N/A" : Result.ToString();
                Console.WriteLine($"Result: {result}; Time: {Time.TotalMilliseconds} ms");
            }
        }
    }
}
