using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hornbill.Common
{
    public abstract class ValueObject<T> : IEquatable<T> where T : ValueObject<T>
    {
        public virtual bool Equals(T other)
        {
            if (other == null)
                return false;

            var type1 = GetType();
            var type2 = other.GetType();

            if (type1 != type2)
                return false;

            foreach (var field in type1.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var obj1 = field.GetValue(other);
                var obj2 = field.GetValue(this);
                if (obj1 == null)
                {
                    if (obj2 != null)
                        return false;
                }
                else if (!obj1.Equals(obj2))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var type1 = GetType();
            var type2 = obj.GetType();

            if (type1 != type2)
                return false;

            return Equals(obj as T);
        }

        public override int GetHashCode()
        {
            return GetFields().Select(field => field.GetValue(this)).Where(value => value != null)
                .Aggregate(17, (current, value) => current * 59 + value.GetHashCode());
        }

        IEnumerable<FieldInfo> GetFields()
        {
            var type = GetType();
            var fieldInfoList = new List<FieldInfo>();
            for (; type != typeof(object); type = type.BaseType)
                fieldInfoList.AddRange(
                    type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            return fieldInfoList;
        }

        public static bool operator ==(ValueObject<T> x, ValueObject<T> y) => x.Equals(y);

        public static bool operator !=(ValueObject<T> x, ValueObject<T> y) => !(x == y);
    }
}