using System;



namespace C6.Tests
{
    public static class RandomFactory
    {
        public static Random CreateInstance()
        {
            var seed = 0;
            System.Diagnostics.Debug.WriteLine($"New random generator constructed with seed {seed}.");
            return new Random(seed);
        }
    }
}
