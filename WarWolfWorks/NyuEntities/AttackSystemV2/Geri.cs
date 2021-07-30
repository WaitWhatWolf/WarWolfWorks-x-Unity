using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.AttackSystemV2
{
    /// <summary>
    /// Used as a "bonus" condition for an attack group (<see cref="Odin"/>). Usually used for Input detection or enemy AI-related checks.
    /// Supported interfaces: <see cref="INyuAwake"/>.
    /// </summary>
    public abstract class Geri : ScriptableObject, IParentable<Odin>, INyuReferencable, IIndexable
    {
        /// <summary>
        /// The <see cref="Geri"/> to which this <see cref="Geri"/> belongs to.
        /// </summary>
        public Odin Parent { get; internal set; }

        /// <summary>
        /// Pointer to the parent's NyuMain.
        /// </summary>
        public Nyu NyuMain => Handler.NyuMain;

        /// <summary>
        /// The <see cref="NyuOdinHandler"/> which manages this <see cref="Geri"/>.
        /// </summary>
        public NyuOdinHandler Handler { get; internal set; }

        /// <summary>
        /// The index of this geri in it's parent.
        /// </summary>
        public int Index { get; internal set; } = -1;

        /// <summary>
        /// Returns true when the condition of this <see cref="Geri"/> is met.
        /// </summary>
        /// <returns></returns>
        public abstract bool Met();
    }
}
