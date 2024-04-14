namespace Source.Utilities
{
    public static class RandomUtils
    {
        public static T Choose<T>(params T[] choices)
        {
            var choice = UnityEngine.Random.Range(0, choices.Length);
            return choices[choice];
        }

    }
}