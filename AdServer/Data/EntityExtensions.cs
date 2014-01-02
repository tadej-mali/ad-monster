using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AdServer.Data
{
    public static class EntityExtensions
    {
        public static EntityState GetSavingState(this IEntityWithId target)
        {
            return target.Id == 0 ? EntityState.Added : EntityState.Modified;
        }

        public static EntityState GetTrackingState(this object target, DbContext ctx)
        {
            return ctx.Entry(target).State;
        }

        public static EntityState GetTrackingState(this object target, IDbContext ctx)
        {
            return ctx.Entry(target).State;
        }

        public static void EnforceAttach<T>(this DbContext ctx, T target) where T : class
        {
            var state = target.GetTrackingState(ctx);
            if (state == EntityState.Detached)
            {
                ctx.Set<T>().Attach(target);
            }
        }

        public static void EnforceAttach<T>(this IDbContext ctx, T target) where T : class
        {
            var state = target.GetTrackingState(ctx);
            if (state == EntityState.Detached)
            {
                ctx.Attach(target);
            }
        }

        public static int Fetch<T>(this ICollection<T> target)
        {
            if (target == null) { return 0; }

            return target.Count;
        }
    }

    public static class RowVersionExtensions
    {
        public static byte[] ToByte(this long value)
        {
            var result = BitConverter.GetBytes(value);

            if (result.Length < 8) { Array.Resize(ref result, 8); }
            if (BitConverter.IsLittleEndian) { result = result.Reverse().ToArray(); }

            return result;
        }

        public static long VersionStampAsLong(this IRowVersion source)
        {
            var toConvert = source.VersionStamp;

            if (toConvert.Length < 8) { Array.Resize(ref toConvert, 8); }
            if (BitConverter.IsLittleEndian) { toConvert = toConvert.Reverse().ToArray(); }

            return BitConverter.ToInt64(toConvert, 0);
        }

        public static bool IsModified(this IRowVersion source, long previous)
        {
            return source.VersionStampAsLong() != previous;
        }
    }
}