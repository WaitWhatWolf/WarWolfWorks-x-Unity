using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EntitiesSystem.Movement
{
    using System;
    using Statistics;
    using UnityEngine.Serialization;
    using WarWolfWorks.Attributes;
    using WarWolfWorks.Interfaces;
    using static WarWolfWorks.EntitiesSystem.Movement.EntityMovement;

    /// <summary>
    /// Base class used for 3D movement of an <see cref="Entity"/>. (Requires a <see cref="Rigidbody"/> component) (Inheritable)
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public class EntityMovement3D : EntityMovement
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
                    if (MoveParent)
                        rb = EntityManager.OldestOf(EntityMain, true).GetComponent<Rigidbody>();

                    if (!MoveParent || rb == null) rb = GetComponent<Rigidbody>();

                    rigidSaved = true;
                }

                return rb;
            }
        }

        [SerializeField, FormerlySerializedAs("moveParent")]
        private bool s_MoveParent = true;
        /// <summary>
        /// If true and this component's <see cref="Entity"/> is <see cref="IEntityParentable"/>,
        /// it will move the parent instead.
        /// </summary>
        public bool MoveParent { get => s_MoveParent; set => s_MoveParent = value; }

        /// <summary>
        /// Unity's Awake method called by EntityComponent system.
        /// </summary>
        public override void OnAwake()
        {
            if (EntityMain is IEntityParentable) ((IEntityParentable)EntityMain).OnParentSet += RefreshRigidbody;
        }

        private void RefreshRigidbody(Entity child, Entity parent)
        {
            rigidSaved = !MoveParent;
        }

        /// <summary>
        /// When overriding this class, make sure to call base.OnFixed(); inside it as it is what 
        /// calculates the time left in Velocities through <see cref="Velocity.Time"/>.
        /// </summary>
        public override void OnFixed()
        {
            if (Locked)
                return;

            for (int i = 0; i < Velocities.Count; i++)
            {
                Velocities[i].Time = Mathf.Clamp(Velocities[i].Time - Time.deltaTime, 0, Velocities[i].StartTime);
                if (Velocities[i].Time <= 0 && Velocities[i].DeleteOnCount0)
                    Velocities.RemoveAt(i);
            }
        }

        /// <summary>
        /// Base OnUpdate method applies UsedVelocity to <see cref="Rigidbody"/>.
        /// </summary>
        public override void OnUpdate()
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
            if (!respectPhysics) Position = position;
            else Rigidbody.MovePosition(position);
        }
    }
}