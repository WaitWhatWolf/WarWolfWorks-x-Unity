using UnityEngine;
using static WarWolfWorks.Utility.Hooks.MathF;
using static WarWolfWorks.Constants;
using System.Collections.Generic;
using System;

namespace WarWolfWorks.Utility
{

    /// <summary>
    /// Behaviour used to make an object follow a list of transforms. Useful for cameras.
    /// (Explicitly convertible to <see cref="Transform"/>)
    /// </summary>
    [AddComponentMenu(IN_ASSETMENU_WARWOLFWORKS + IN_ASSETMENU_UTILITY + nameof(FollowBehavior))]
    public class FollowBehavior : MonoBehaviour
    {
        /// <summary>
        /// All followed transform components.
        /// </summary>
        public List<Transform> Followed;

        /// <summary>
        /// If true, the Z axis is ignored.
        /// </summary>
        public bool Is2D;

        /// <summary>
        /// Speed at which this object will be moved towards the final destination.
        /// </summary>
        public float Speed = 1f;

        /// <summary>
        /// When far away from the desired position, the follow behavior will start accelerating by the given value based on distance.
        /// </summary>
        public float Acceleration = 1f;

        /// <summary>
        /// Required distance for the acceleration to be applied.
        /// </summary>
        public float AccelerationDistance = float.NegativeInfinity;

        /// <summary>
        /// Override this Vector3 to set a specific position to move; Points to transform.position by default.
        /// </summary>
        protected virtual Vector3 MovedPosition 
        { 
            get => Is2D ? new Vector3(transform.position.x, transform.position.y) : transform.position; 
            set => transform.position = value; 
        }

        /// <summary>
        /// Used to apply physics-based speed on follow; Points to <see cref="Time.deltaTime"/> by default.
        /// </summary>
        protected virtual float DeltaTime => Time.deltaTime;

        /// <summary>
        /// Returns the position towards which this <see cref="FollowBehavior"/> is going to.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetFinalPosition()
        {
            Vector3 destination = Vector3.zero;

            foreach (Transform t in Followed)
            {
                if(t is not null)
                    destination += t.position;
            }

            if(destination != Zero)
                destination /= Followed.Count;

            return destination;
        }

        private void FixedUpdate()
        {
            MovedPosition = MoveTowardsAccelerated(MovedPosition, GetFinalPosition(), Speed * DeltaTime, Acceleration * DeltaTime, AccelerationDistance);
        }
    }
}