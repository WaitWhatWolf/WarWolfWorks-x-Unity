using System;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Used for advanced cloning of a class. An advanced version of <see cref="ICloneable"/>.
    /// </summary>
    public abstract class AbstractCloneable<T> : ICloneable
    {
        public object Clone()
        {
            var clone = (T)MemberwiseClone();
            HandleCloned(clone);
            return clone;
        }

        protected abstract void HandleCloned(T clone);
    }
}
