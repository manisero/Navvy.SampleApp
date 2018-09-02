using System;

namespace Navvy.SampleApp.Console.Utils
{
    public static class LazyExtensions
    {
        public static TValue ValueIfCreated<TValue>(
            this Lazy<TValue> lazy)
            where TValue : class
        {
            return lazy.IsValueCreated
                ? lazy.Value
                : null;
        }
    }
}
