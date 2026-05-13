using System;

namespace Utils
{
    public static class MathExtensions
    {
        public static int Ceiling(this float value)
            => (int)Math.Ceiling(value);
    }
}