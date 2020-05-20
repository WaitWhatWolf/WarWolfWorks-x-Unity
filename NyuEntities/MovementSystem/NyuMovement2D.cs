using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.MovementSystem
{
    /// <summary>
    /// Base class for an entity's 2D movement.
    /// </summary>
    [CompleteNoS]
    public class NyuMovement2D : NyuMovement, INyuPreAwake, INyuUpdate
    {
        private Rigidbody2D rb;
        /// <summary>
        /// Rigidbody used by this component.
        /// </summary>
        public Rigidbody2D Rigidbody => rb;

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

        private void Event_RefreshRigidbody(Nyu child, Nyu parent)
        {
            rb = parent.gameObject.AddComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Base NyuUpdate method applies UsedVelocity to <see cref="Rigidbody"/>. (<see cref="INyuUpdate"/> implementation)
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
            if (NyuMain is INyuEntityParentable nyuParent) nyuParent.OnNyuParentSet += Event_RefreshRigidbody;

            rb = NyuMain.GetComponent<Rigidbody2D>();
        }
    }
}
