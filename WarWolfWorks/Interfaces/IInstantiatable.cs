using System.Collections.Generic;
using WarWolfWorks.Utility;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Used with an Unity Object to be called right after (preferably still inside) <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object)"/>.
    /// (Compatible with <see cref="Hooks.Enumeration.InstantiateList{T}(IEnumerable{T})"/>)
    /// </summary>
    public interface IInstantiatable
    {
#pragma warning disable 0649
        /// <summary>
        /// Instantiate sub-objects which are not automatically instantiated by <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object)"/>.
        /// </summary>
        void PostInstantiate();
    }
}
