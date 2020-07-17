using UnityEngine;

namespace WarWolfWorks.Utility
{
    using static Hooks.Vectors;
    using System.Collections.Generic;
    using System;

    /// <summary>
    /// Behaviour used to make an object follow a list of transforms. Useful for cameras.
    /// (Explicitly convertible to <see cref="Transform"/>)
    /// </summary>
    [AddComponentMenu("WWW/Utility/Follow Behaviour")]
    public sealed class FollowBehavior : MonoBehaviour
    {
        private Rigidbody2D rb;
        /// <summary>
        /// Follow behaviour's rigidbody; Only used if <see cref="UsesRigidbody"/> returns true.
        /// </summary>
        public Rigidbody2D RigidBody
        {
            get
            {
                if (rb == null)
                    rb = GetComponent<Rigidbody2D>();
                return rb;
            }
        }

        [SerializeField]
        private List<Transform> FollowObjects = new List<Transform>();

        [SerializeField]
        private float speed = 1;
        [SerializeField]
        private float acceleration = .2f;
        [SerializeField]
        private bool accelerateWhenFar = false;
        [SerializeField]
        private bool isFollowing = true;

        [SerializeField]
        private Vector3 positionBlocker = Vector3.zero;

        [SerializeField]
        private bool blockX = false;
        [SerializeField]
        private bool blockY = false;
        [SerializeField]
        private bool blockZ = false;

        /// <summary>
        /// If true, this class will use a rigidbody instead of it's transform.
        /// </summary>
        public bool UsesRigidbody;

        /// <summary>
        /// All objects followed by this FollowBehaviour.
        /// </summary>
        public Transform[] FollowedObjects => FollowObjects.ToArray();

        /// <summary>
        /// Speed at which this script follows <see cref="FollowedObjects"/>.
        /// </summary>
        public float Speed { get { return speed; } private set { speed = value; } }

        /// <summary>
        /// Speed at which this behaviour accelerates based on distance.
        /// </summary>
        public float Acceleration { get { return acceleration; } set { acceleration = value; } }

        /// <summary>
        /// Decides if this behaviour accelerates when further away from the object.
        /// </summary>
        public bool AccelerateWhenFar => accelerateWhenFar;

        /// <summary>
        /// If false, this behaviour will stop following.
        /// </summary>
        public bool IsFollowing
        {
            get => isFollowing;
            private set => isFollowing = value;
        }
        /// <summary>
        /// Position at which this behaviour will be blocked in based on BlocksX, BlocksY and BlocksZ.
        /// </summary>
        public Vector3 PositionBlocker => positionBlocker;

        /// <summary>
        /// Does this <see cref="FollowBehavior"/> block the X axis?
        /// </summary>
        public bool BlocksX => blockX;
        /// <summary>
        /// Does this <see cref="FollowBehavior"/> block the Y axis?
        /// </summary>
        public bool BlocksY => blockY;
        /// <summary>
        /// Does this <see cref="FollowBehavior"/> block the Z axis?
        /// </summary>
        public bool BlocksZ => blockZ;

        private Vector3 MoveVector(Vector3 destination)
        {
            Vector3 toReturn = accelerateWhenFar ? MoveTowardsAccelerated(transform.position, destination, Speed, Acceleration) : Vector3.MoveTowards(transform.position, destination, Speed);
            if (!blockX && !blockY && !blockZ)
                return toReturn;
            toReturn = new Vector3(blockX ? PositionBlocker.x : toReturn.x, blockY ? PositionBlocker.y : toReturn.y, blockZ ? PositionBlocker.z : toReturn.z);
            return toReturn;
        }

        /// <summary>
        /// Adds a Transform to the list of followed objects.
        /// </summary>
        /// <param name="t"></param>
        public void AddFollower(Transform t) => FollowObjects.Add(t);

        /// <summary>
        /// Removes an existing Transform from the list of followed objects.
        /// </summary>
        /// <param name="t"></param>
        public void RemoveFollower(Transform t) => FollowObjects.Remove(t);

        /// <summary>
        /// Returns true if the given transform is inside of the followed objects list.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool ContainsFollower(Transform t)
            => FollowObjects.Contains(t);

        private void FixedUpdate()
        {
            if (!IsFollowing)
                return;

            if (FollowObjects.Count == 1)
                FollowSingleObject(0);
            else if (FollowObjects.Count > 1) FollowAllObjects();
        }

        private void FollowSingleObject(int index)
        {
            try
            {
                if (!UsesRigidbody) transform.position = MoveVector(FollowObjects[index].position);
                else RigidBody.MovePosition(MoveVector(FollowObjects[index].position));
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Changes an axis block to the given boolean value.
        /// 0 = X axis, 1 = Y axis, 2 = Z axis.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="block"></param>
        public void ChangeBlock(int position, bool block)
        {
            switch (position)
            {
                default: blockZ = block; return;
                case 0: blockX = block; return;
                case 1: blockY = block; return;
            }
        }

        /// <summary>
        /// Sets the block position of an axis.
        /// 0 = X axis, 1 = Y axis, 2 = Z axis.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        public void ChangeBlock(int position, float value)
        {
            positionBlocker = new Vector3(position == 0 ? value : positionBlocker.x, position == 1 ? value : positionBlocker.y, position == 2 ? value : positionBlocker.z);
        }

        private void FollowAllObjects()
        {
            Vector3 toUse = Vector3.zero;
            foreach (Transform t in FollowObjects)
            {
                toUse += t.position;
            }

            toUse /= FollowObjects.Count;

            if (!UsesRigidbody) transform.position = MoveVector(toUse);
            else RigidBody.MovePosition(MoveVector(toUse));
        }

        /// <summary>
        /// Explicitly returns <see cref="FollowBehavior"/>.transform.
        /// </summary>
        /// <param name="fb"></param>
        public static explicit operator Transform(FollowBehavior fb)
            => fb.transform;

    }
}