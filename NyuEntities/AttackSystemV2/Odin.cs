using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.NyuEntities.AttackSystemV2
{
    /// <summary>
    /// The core class which groups all necessary objects to make a functional attack. Handled by <see cref="NyuOdinHandler"/>.
    /// </summary>
    [CompleteNoS]
    [CreateAssetMenu(fileName = "Odin_", menuName = "WarWolfWorks/Attack System V2/Odin")]
    public sealed class Odin : ScriptableObject, IParentable<NyuOdinHandler>, IInstantiatable, IIndexable
    {
        [SerializeField]
        private Freki s_Freki;
        /// <summary>
        /// The attack of this attack group.
        /// </summary>
        public Freki GetFreki() => s_Freki;
        
        [SerializeField]
        private Geri s_Geri;
        /// <summary>
        /// The attack condition of this attack group.
        /// </summary>
        public Geri GetGeri() => s_Geri;
        /// <summary>
        /// The index of this odin in it's parent.
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// The parent of this <see cref="Odin"/>.
        /// </summary>
        public NyuOdinHandler Parent { get; internal set; }

        /// <summary>
        /// Returns a new instance of a <see cref="Odin"/>.
        /// </summary>
        /// <param name="freki"></param>
        /// <param name="geri"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Odin New(Freki freki, Geri geri, int index)
        {
            Odin toReturn = CreateInstance<Odin>();
            toReturn.s_Freki = freki;
            toReturn.s_Geri = geri;
            toReturn.Index = index;

            return toReturn;
        }

        internal void SetRespectiveParents()
        {
            s_Freki.Parent = s_Geri.Parent = this;
            s_Freki.Handler = s_Geri.Handler = Parent;
        }

        /// <summary>
        /// Call this after instantiating for a complete instantiation.
        /// </summary>
        public void PostInstantiate()
        {
            s_Freki = Instantiate(s_Freki);
            s_Geri = Instantiate(s_Geri);
            if(Parent)
                SetRespectiveParents();
        }

        /// <summary>
        /// Allows to implicitly cast an odin as a freki.
        /// </summary>
        /// <param name="odin"></param>
        public static implicit operator Freki(Odin odin)
            => odin.s_Freki;

        /// <summary>
        /// Allows to implicitly cast an odin as a geri.
        /// </summary>
        /// <param name="odin"></param>
        public static implicit operator Geri(Odin odin)
            => odin.s_Geri;
    }
}
