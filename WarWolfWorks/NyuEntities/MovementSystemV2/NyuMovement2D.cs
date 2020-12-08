using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.MovementSystemV2
{
    /// <summary>
    /// Base class for an entity's 2D movement.
    /// </summary>
    public class NyuMovement2D : NyuMovement, INyuUpdate
    {
        private bool rigidSaved;
        private Rigidbody2D rb;
        /// <summary>
        /// Rigidbody used by this component.
        /// </summary>
        public Rigidbody2D Rigidbody
        {
            get
            {
                if (!rigidSaved)
                {
                    if (!rb) rb = NyuMain.GetComponent<Rigidbody2D>();

                    rigidSaved = rb;
                }

                return rb;
            }
        }

        /// <summary>
        /// Base NyuUpdate method applies UsedVelocity to <see cref="Rigidbody"/> 
        /// and calls all INyuUpdate implementations of it's velocities. (<see cref="INyuUpdate"/> implementation)
        /// </summary>
        public override void NyuUpdate()
        {
            base.NyuUpdate();
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
    }
}
