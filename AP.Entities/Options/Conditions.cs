using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace AP.Entities.Options
{
    public class Conditions<TEntity> : Dictionary<string, string[]> where TEntity : class
    {
        public void Validate()
        {
            foreach (var key in base.Keys)
            {
                DataCheck(key, base[key]);
            }
        }

        public new void Add(string key, string[] value)
        {
            DataCheck(key, value);

            base.Add(key, value);
        }

        public new bool TryAdd(string key, string[] value)
        {
            DataCheck(key, value);

            return base.TryAdd(key, value);
        }

        private void DataCheck(string key, string[] values)
        {
            key = UppercaseFirst(key);

            var prop = typeof(TEntity).GetProperty(key);

            if(prop == null)
                throw new PropertyNotFoundException(key);

            foreach(string value in values) {
                var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                converter.ConvertFromInvariantString(value);
            }
        }

        private string UppercaseFirst(string s)
        {
            if(string.IsNullOrEmpty(s))
                return string.Empty;

            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }

    public class PropertyNotFoundException : Exception
    {
        public PropertyNotFoundException()
        {
        }

        public PropertyNotFoundException(string message) : base(message)
        {
        }

        public PropertyNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PropertyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}