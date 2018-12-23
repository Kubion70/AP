using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AP.Entities;
using AP.Entities.Options;
using AP.Repositories.Contexts;

namespace AP.Repositories.Extensions.ConditionExtension
{
    public static class ConditionExtension
    {
        public static IQueryable<TEntity> Conditions<TEntity>(this IQueryable<TEntity> query, Conditions<TEntity> conditions, DatabaseContext databaseContext) where TEntity : Entity
        {
            foreach (var keyValuePair in conditions)
            {
                var key = UppercaseFirst(keyValuePair.Key);

                var propInfo = typeof(TEntity).GetProperty(key);

                if (propInfo != null)
                {
                    var converter = TypeDescriptor.GetConverter(propInfo.PropertyType);

                    var converdedValues = new List<object>();
                    foreach (var value in keyValuePair.Value)
                    {
                        object convertedVal = converter.ConvertFromInvariantString(value);
                        converdedValues.Add(convertedVal);
                    }

                    query = query.Where(e => converdedValues.Contains(e.GetType().GetProperty(key).GetValue(e, null)));
                }
                else
                {
                    key = key.EndsWith("Id") ? key : $"{key}Id";

                    var relationalPropInfo = typeof(TEntity)
                        .GetProperties()
                        .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericArguments()[0].GetProperty(key) != null)
                        .FirstOrDefault();

                    if(relationalPropInfo != null)
                    {
                        var converter = TypeDescriptor.GetConverter(relationalPropInfo.PropertyType.GetGenericArguments()[0].GetProperty(key).PropertyType);

                        var converdedValues = new List<object>();
                        foreach (var value in keyValuePair.Value)
                        {
                            object convertedVal = converter.ConvertFrom(value);
                            converdedValues.Add(convertedVal);
                        }

                        var relationProperty = databaseContext.GetType().GetProperties()
                            .Where(p => p.PropertyType.GetGenericArguments()[0].GetProperty(key) != null)
                            .FirstOrDefault();

                        var relations = ((IQueryable<object>)relationProperty.GetValue(databaseContext, null)).Where(x => converdedValues.Any(v => x.Equals(v)));

                        query = query.Where(e => relations.Any(r => r.Equals(e.Id)));
                    }
                }
            }
            return query;
        }

        private static string UppercaseFirst(string s)
        {
            if(string.IsNullOrEmpty(s))
                return string.Empty;

            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}