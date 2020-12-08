#pragma warning disable CS1591

using System;
using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.MovementSystemV2
{
    /// <summary>
    /// A velocity which uses a func event to determine it's velocity.
    /// </summary>
    public class DynamicVelocity : IVelocity
    {
        public Func<DynamicVelocity, Vector3> DynamicVector;

        /// <summary>
        /// Is this velocity currently residing in a NyuMovement?
        /// </summary>
        public bool Initiated { get; private set; }

        /// <summary>
        /// The parent of this velocity.
        /// </summary>
        public NyuMovement Parent { get; private set; }

        public Nyu NyuMain => Parent.NyuMain;

        public Vector3 GetValue() => DynamicVector(this);

        /// <summary>
        /// Initiates this <see cref="DynamicVelocity"/>; Taken care of by <see cref="NyuMovement"/>.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public bool Init(NyuMovement parent)
        {
            if (Initiated || parent == null)
                return Initiated;

            Parent = parent;
            return Initiated = true;
        }

        public DynamicVelocity(Func<DynamicVelocity, Vector3> dynamicVector)
        {
            DynamicVector = dynamicVector;
        }
    }
}
