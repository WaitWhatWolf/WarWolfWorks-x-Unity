using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.MovementSystem
{
    /// <summary>
    /// Base class for an entity's 3D movement.
    /// </summary>
    [CompleteNoS]
    public class NyuMovement3D : NyuMovement, INyuPreAwake, INyuUpdate
    {
        private bool rigidSaved;
        private Rigidbody rb;
        /// <summary>
        /// Rigidbody used by this component.
        /// </summary>
        public Rigidbody Rigidbody
        {
            get
            {
                if (!rigidSaved)
                {
                    //if (MoveParent)
                    //    rb = EntityManager.OldestOf(EntityMain, true).GetComponent<Rigidbody>();

                    if (!MoveParent || rb == null) rb = NyuMain.GetComponent<Rigidbody>();

                    rigidSaved = true;
                }

                return rb;
            }
        }

        [SerializeField]
        private bool s_MoveParent = true;
        /// <summary>
        /// If true and this component's <see cref="Nyu"/> is <see cref="IEntityParentable"/>,
        /// it will move the parent instead.
        /// </summary>
        public bool MoveParent { get => s_MoveParent; set => s_MoveParent = value; }

        /// <summary>
        /// Initiated state of <see cref="NyuMovement3D"/>. (<see cref="INyuAwake"/> implementation)
        /// </summary>
        public bool NyuInitiated { get; set; }

        private void RefreshRigidbody(Nyu child, Nyu parent)
        {
            rigidSaved = !MoveParent;
        }

        /// <summary>
        /// Base OnUpdate method applies UsedVelocity to <see cref="Rigidbody"/>. (<see cref="INyuUpdate"/> implementation)
        /// </summary>
        public virtual void NyuUpdate()
        {
            Rigidbody.velocity = UsedVelocity;
        }

        /// <summary>
        /// Moves the entity to the specified position.
        /// </summary>
        /// <param name="position">Position to which the entity will be moved.</param>
        /// <param name="respectPhysics">If true, the position moving will respect other colliders and will try not to go inside or past them, otherwise it will ignore that.</param>
        public override void MovePosition(Vector3 position, bool respectPhysics)
        {
            if (!respectPhysics) NyuMain.Position = position;
            else Rigidbody.MovePosition(position);
        }

        void INyuPreAwake.NyuPreAwake()
        {
            if (NyuMain is INyuEntityParentable nyuParent) nyuParent.OnNyuParentSet += RefreshRigidbody;
        }
    }
}
