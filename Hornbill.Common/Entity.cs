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
    }
}