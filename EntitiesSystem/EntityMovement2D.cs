﻿using UnityEngine;
using WarWolfWorks.Interfaces;
using static WarWolfWorks.EntitiesSystem.Movement.EntityMovement;

namespace WarWolfWorks.EntitiesSystem.Movement
{
    /// <summary>
    /// Base class used for 2D movement of an <see cref="Entity"/>. (Requires a <see cref="Rigidbody2D"/> component) (Inheritable)
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityMovement2D : EntityMovement
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
                    rb = GetComponent<Rigidbody2D>();
                    rigidSaved = true;
                }

                return rb;
            }
        }

        [SerializeField]
        private bool moveParent = true;
        /// <summary>
        /// If true and this component's <see cref="Entity"/> is <see cref="IEntityParentable"/>,
        /// it will move the parent instead.
        /// </summary>
        public bool MoveParent { get => moveParent; set => moveParent = value; }

        /// <summary>
        /// Unity's Awake method called by EntityComponent system.
        /// </summary>
        public override void OnAwake()
        {
            if (EntityMain is IEntityParentable) ((IEntityParentable)EntityMain).OnParentSet += RefreshRigidbody;
        }

        private void RefreshRigidbody(Entity child, Entity parent)
        {
            if (MoveParent)
                rb = parent.GetComponent<Rigidbody2D>();
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// When overriding this class, make sure to call base.OnFixed(); inside it as it is what 
        /// calculates the time left in Velocities that have <see cref="Velocity.VelocityRunsOut"/>.
        /// </summary>
        public override void OnFixed()
        {
            for (int i = 0; i < Velocities.Count; i++)
            {
                if (!Velocities[i].VelocityRunsOut)
                    continue;
                Velocities[i].CurrentTimer -= Time.fixedDeltaTime;
                if (Velocities[i].CurrentTimer <= 0)
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
            if (!respectPhysics)
            {
                if (!MoveParent) Position = position;
                else EntityUtilities.OldestOf(EntityMain, true).Position = position;
            }
            else Rigidbody.MovePosition(position);
        }
    }
}