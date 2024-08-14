using System;

namespace Source.Mathematics
{
    public struct Range<T> where T : IComparable
    {
        public T Min { get; }
        public T Max { get; }

        public Range(T min, T max)
        {
            Min = min;
            Max = max;
        }
    
    }

    public static class RangeExtensions
    {
        public static float ChooseRandom(this Range<float> range)
        {
            return UnityEngine.Random.Range(range.Min, range.Max);
        }
        
        public static int ChooseRandom(this Range<int> range)
        {
            return UnityEngine.Random.Range(range.Min, range.Max);
        }
    }
}