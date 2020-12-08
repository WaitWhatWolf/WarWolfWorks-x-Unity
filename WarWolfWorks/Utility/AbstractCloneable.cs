using System;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Used for advanced cloning of a class. An advanced version of <see cref="ICloneable"/>.
    /// </summary>
    public abstract class AbstractCloneable<T> : ICloneable
    {
        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var clone = (T)MemberwiseClone();
            HandleCloned(clone);
            return clone;
        }

        /// <summary>
        /// Handles type-specific cloning; Called in <see cref="Clone"/>.
        /// </summary>
        /// <param name="clone"></param>
        protected abstract void HandleCloned(T clone);
    }
}
