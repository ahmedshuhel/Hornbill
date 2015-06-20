using System;
using System.ComponentModel;

namespace Hornbill.Common
{
    public abstract class Entity<T, TKey>
        where T : Entity<T, TKey>
    {
        private int? _oldHashCode;
        public TKey Id { get; protected set; }

        public override bool Equals(object obj)
        {
            var other = obj as T;
            if (other == null)
                return false;
            //to handle the case of comparing two new objects
            var otherIsTransient = Equals(other.Id, default(TKey));
            var thisIsTransient = Equals(Id, default(TKey));
            if (otherIsTransient && thisIsTransient)
                return ReferenceEquals(other, this);
            return other.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            //This is done se we won't change the hash code
            if (_oldHashCode.HasValue)
                return _oldHashCode.Value;
            var thisIsTransient = Equals(Id, default(TKey));
            //When we are transient, we use the base GetHashCode()
            //and remember it, so an instance can't change its hash code.
            if (thisIsTransient)
            {
                _oldHashCode = base.GetHashCode();
                return _oldHashCode.Value;
            }
            return Id.GetHashCode();
        }

        public static bool operator ==(Entity<T, TKey> x, Entity<T, TKey> y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(Entity<T, TKey> x, Entity<T, TKey> y)
        {
            return !(x == y);
        }

        public virtual bool IsTransient
        {
            get
            {
                if (Id.GetType() == typeof(Guid))
                    return (Guid)TypeDescriptor.GetConverter(Id).ConvertFrom(Id.ToString()) ==
                           Guid.Empty;

                double testDouble;

                if (Double.TryParse(Id.ToString(), out testDouble))
                    return
                        (double)TypeDescriptor.GetConverter(typeof(double)).ConvertFrom(Id.ToString()) ==
                        0d;

                if (Id.GetType() == (typeof(string)))
                    return
                        (string)TypeDescriptor.GetConverter(Id).ConvertFrom(Id.ToString()) ==
                        String.Empty;

                //if we get this far, we have a non-GUID, non-numeric, non string identity type and this is unsupported, so throw...
                throw new ArgumentException(
                    "IdentityPersistenceBase<TObject, TIdentity> Class only provides native support for Guid, Numeric (Int, Int32, Int64, Double, etc.) or String as the TIdentity type.  For other types (including any custom types), you *must* override the IsTransient virtual property to provide your own implementation!",
                    "TIdentity");
            }
        }
    }
}