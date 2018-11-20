using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AP.Entities.Options;

namespace AP.Repositories.Extensions.ConditionExtension
{
    public static class ConditionExtension
    {
        public static IQueryable<TEntity> Conditions<TEntity>(this IQueryable<TEntity> query, Conditions<TEntity> conditions) where TEntity : class
        {
            foreach (var keyValuePair in conditions)
            {
                var key = UppercaseFirst(keyValuePair.Key);

                var propInfo = typeof(TEntity).GetProperty(key);

                var converter = TypeDescriptor.GetConverter(propInfo.PropertyType);

                var converdedValues = new List<object>();
                foreach(var value in keyValuePair.Value)
                {
                    object convertedVal = converter.ConvertFromInvariantString(value);
                    converdedValues.Add(convertedVal);
                }
                
                query = query.Where(e => converdedValues.Contains(e.GetType().GetProperty(key).GetValue(e, null)));
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