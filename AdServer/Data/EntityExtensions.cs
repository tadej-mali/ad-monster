using System.Collections.Generic;
using System.Data.Entity;

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
}