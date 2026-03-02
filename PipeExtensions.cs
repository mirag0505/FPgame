using System;

namespace FPgame
{
    public static class PipeExtensions
    {
        public static TOut Pipe<TIn, TOut>(this TIn input, Func<TIn, TOut> func)
        {
            return func(input);
        }

        public static T Pipe<T>(this T input, Action<T> action)
        {
            action(input);
            return input;
        }
    }
}
