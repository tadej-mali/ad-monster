using System;
using System.Collections.Generic;
using System.Linq;

namespace AdServer.Utility
{
    public static class LinqExtensions
    {
        public static bool IsEmpty<T>(this ICollection<T> target)
        {
            if (target == null) { return true; }

            return !target.Any();
        }

        public static bool HasAny<T>(this ICollection<T> target)
        {
            if (target == null) { return false; }

            return target.Any();
        }

        public static bool HasAny<T>(this ICollection<T> target, Func<T, bool> predicate)
        {
            if (target == null) { return false; }
            if (predicate == null) { return target.HasAny(); }

            return target.Any(predicate);
        }

        public static void RemoveRange<TSource>(this ICollection<TSource> target, IEnumerable<TSource> source)
        {
            if ((target == null) || (source == null)) { return; }

            foreach (var item in source)
            {
                target.Remove(item);
            }
        }

        public static void Replace<TSource>(this ICollection<TSource> target, IEnumerable<TSource> source)
        {
            if (target == null) { return; }

            foreach (var item in target.ToList())
            {
                target.Remove(item);
            }

            if (source == null) { return; }

            target.AddRange(source);
        }


        public static ICollection<TSource> AddRange<TSource>(this ICollection<TSource> target, IEnumerable<TSource> source)
        {
            if (target == null) { return null; }
            if (source == null) { return target; }

            foreach (var item in source)
            {
                target.Add(item);
            }

            return target;
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == null)
                return;
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<int, TSource> action)
        {
            if (source == null)
                return;
            int i = 0;

            foreach (var item in source)
            {
                action(i, item);
                i++;
            }
        }
    }
}