using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Rates.API.Extensions;

public static class DbSetExtensions
{
    public static T AddIfNotExists<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, bool>>? predicate = null) where T : class, new()
    {
        var existingEntity = predicate != null ? dbSet.Where(predicate).FirstOrDefault() : dbSet.FirstOrDefault();
        return existingEntity ?? dbSet.Add(entity).Entity;
    }
}