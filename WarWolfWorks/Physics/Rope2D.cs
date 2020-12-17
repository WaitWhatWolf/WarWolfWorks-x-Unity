using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Physics
{
    /// <summary>
    /// Simulates a rope in 2D space with a line renderer.
    /// </summary>
    [AddComponentMenu(IN_ASSETMENU_WARWOLFWORKS + IN_ASSETMENU_PHYSICS + nameof(Rope2D)), RequireComponent(typeof(LineRenderer))]
    public class Rope2D : MonoBehaviour
    {
        /// <summary>
        /// How long each segment of the rope is.
        /// </summary>
        [Tooltip("How long each segment of the rope is.")]
        public float SegmentHeight = .2f;

        /// <summary>
        /// The width of the rope in the line renderer.
        /// </summary>
        [Tooltip("The width of the rope in the line renderer.")]
        public FloatRange RopeWidth = new FloatRange(0.1f, 0.1f);
        /// <summary>
        /// How many segments are on the rope.
        /// </summary>
        [Tooltip("How many segments are on the rope"), Range(2, 100)]
        public int SegmentCount = 5;

        /// <summary>
        /// How severe the distance constrain is applied between segments.
        /// </summary>
        [Tooltip("How severe the distance constrain is applied between segments."), Range(1, 100)]
        public int ConstrainSeverity = 10;

        /// <summary>
        /// If true, it will apply the world's gravity force to this rope.
        /// </summary>
        [Tooltip("If true, it will apply the world's gravity force to this rope.")]
        public bool UseGravity = true;

        /// <summary>
        /// If true, it will use a EdgeCollider2D on the rope.
        /// </summary>
        [Tooltip("If true, it will use a EdgeCollider2D on the rope.")]
        public bool UseEdgeCollider = false;

        /// <summary>
        /// This position designates where the rope starts; This cannot be null.
        /// </summary>
        [Tooltip("This position designates where the rope starts; This field cannot be empty.")]
        public Transform RopeStartPosition;

        /// <summary>
        /// This position designates where the tip of the rope will end up; Setting this to null will make the rope's last segment have no constraints.
        /// </summary>
        [Tooltip("This position designates where the tip of the rope will end up; Leaving this empty will automatically put it on the max distance.")]
        public Transform RopeEndPosition;

        /// <summary>
        /// Returns the current position of each joint in the rope.
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetSegmentPositions()
        {
            Vector3[] vectors = new Vector3[ns_Segments.Count];

            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i] = ns_Segments[i];
            }

            return vectors;
        }

        /// <summary>
        /// Returns the current position of each joint in the rope as Vector2.
        /// </summary>
        /// <returns></returns>
        public Vector2[] GetSegmentPositions2D()
        {
            Vector2[] vectors = new Vector2[ns_Segments.Count];

            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i] = ns_Segments[i];
            }

            return vectors;
        }

        /// <summary>
        /// Returns the current position of each joint in the rope as Vector2.
        /// </summary>
        /// <returns></returns>
        public Vector2[] GetSegmentPositions2DLocal()
        {
            Vector2[] vectors = new Vector2[ns_Segments.Count];

            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i] = ns_Segments[i] - RopeStartPosition.position;
            }

            return vectors;
        }

        /// <summary>
        /// The line renderer; Set in <see cref="Awake"/>.
        /// </summary>
        protected LineRenderer ns_LineRenderer;
        /// <summary>
        /// The edge collider added to this rope when <see cref="UseEdgeCollider"/> is true.
        /// </summary>
        protected EdgeCollider2D ns_EdgeCollider2D;

        /// <summary>
        /// Used when <see cref="UseGravity"/> is true. Points to <see cref="Physics2D.gravity"/>.
        /// </summary>
        protected virtual Vector2 Gravity => Physics2D.gravity;

        /// <summary>
        /// Gets the line renderer.
        /// </summary>
        protected virtual void Awake()
        {
            if (!RopeStartPosition)
                throw new Exception("Cannot use a rope when it's RopeStartPosition is null.");

            ns_LineRenderer = GetComponent<LineRenderer>();

            ns_EdgeCollider2D = GetComponent<EdgeCollider2D>();
            if(ns_EdgeCollider2D == null && UseEdgeCollider)
            {
                ns_EdgeCollider2D = gameObject.AddComponent<EdgeCollider2D>();
            }

            StartCoroutine(IE_CheckForChanges());
        }

        /// <summary>
        /// Initiates the rope and sets the line renderer by calling <see cref="UpdateRope"/>.
        /// </summary>
        protected virtual void Start()
        {
            ns_LineRenderer.useWorldSpace = true;

            UpdateRope();
        }

        /// <summary>
        /// Updates the rope to have the correct segment count and distance between each segment.
        /// </summary>
        public void UpdateRope()
        {
            ns_Segments.Clear();

            previousHeight = SegmentHeight;
            previousCount = SegmentCount;
            segmentStartPos = RopeStartPosition.position;

            for (int i = 0; i < SegmentCount; i++)
            {
                ns_Segments.Add(new Segment(segmentStartPos));
                segmentStartPos.y -= SegmentHeight;
            }
        }

        /// <summary>
        /// Calls <see cref="Draw"/>.
        /// </summary>
        protected virtual void Update()
        {
            Draw();
        }

        /// <summary>
        /// Calls <see cref="Simulate"/> and <see cref="Constrain(int)"/> using <see cref="ConstrainSeverity"/> as argument.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            Simulate();
            Constrain(ConstrainSeverity);
        }

        /// <summary>
        /// Simulates the physics of the rope.
        /// </summary>
        protected void Simulate()
        {
            Vector2 gravity = Gravity;

            for(int i = 0; i < SegmentCount; i++)
            {
                Segment current = ns_Segments[i];
                //ns_Segments[i].SetVelocity(gravity);

                Vector2 vel = current.Position - current.Previous;
                current.Previous = current.Position;
                current.Position += vel;
                current.Position += gravity * Time.deltaTime;
                ns_Segments[i] = current;
            }

            if(UseEdgeCollider)
            {
                Vector2[] points = GetSegmentPositions2DLocal();
                ns_EdgeCollider2D.points = points;
            }
        }

        /// <summary>
        /// Limits the distance between each segment to <see cref="SegmentHeight"/>.
        /// It also makes the first segment always set to <see cref="RopeStartPosition"/>,
        /// and last segment to <see cref="RopeEndPosition"/> if it was set to 0.
        /// </summary>
        /// <param name="amount">How severe the constrain is; Note that the higher the value the greater the performance hit.</param>
        protected void Constrain(int amount)
        {
            Segment startingSegment = ns_Segments[0];
            startingSegment.Position = RopeStartPosition.position;
            ns_Segments[0] = startingSegment;

            if (RopeEndPosition != null)
            {
                Segment endSegment = ns_Segments[SegmentCount - 1];
                endSegment.Position = RopeEndPosition.position;
                ns_Segments[SegmentCount - 1] = endSegment;
            }

            for (int i = 0; i < amount; i++)
                Internal_Constrain();
        }

        private void Internal_Constrain()
        {
            Segment next;
            Vector2 changeAmount;

            GetConstrainValues(0, out _, out next, out changeAmount);
            next.Position += changeAmount;
            ns_Segments[1] = next;

            for (int i = 1; i < SegmentCount - 1; i++)
            {
                GetConstrainValues(i, out Segment first, out next, out changeAmount);
                first.Position -= changeAmount * 0.5f;
                ns_Segments[i] = first;
                next.Position += changeAmount * 0.5f;
                ns_Segments[i + 1] = next;
            }
        }

        private void GetConstrainValues(int index, out Segment first, out Segment next, out Vector2 changeAmount)
        {
            first = ns_Segments[index];
            next = ns_Segments[index + 1];

            float dist = (first.Position - next.Position).magnitude;
            float error = dist - SegmentHeight;
            Vector2 changeDir = (first.Position - next.Position).normalized;

            changeAmount = changeDir * error;
        }

        /// <summary>
        /// Draws the rope on the <see cref="ns_LineRenderer"/>.
        /// </summary>
        protected void Draw()
        {
            ns_LineRenderer.startWidth = RopeWidth.Min;
            ns_LineRenderer.endWidth = RopeWidth.Max;

            ns_LineRenderer.positionCount = ns_Segments.Count;

            Vector3[] vectors = GetSegmentPositions();
            ns_LineRenderer.SetPositions(vectors);
        }

        private IEnumerator IE_CheckForChanges()
        {
            while(this != null)
            {
                yield return wfs_CheckForChanges;

                if (previousCount != SegmentCount || previousHeight != SegmentHeight)
                    UpdateRope();
            }
        }

        private struct Segment
        {
            public Vector2 Position;
            public Vector2 Previous;

            public Segment(Vector2 position)
            {
                Position = Previous = position;
            }

            public Vector2 GetVelocity() => Position - Previous;

            public void SetVelocity(Vector2 @new) 
            {
                Vector2 vel = Position - Previous;
                Previous = Position;
                Position += vel;
                Position += @new;
            }

            public static implicit operator Vector3(Segment segment)
                => segment.Position;

            public static implicit operator Vector2(Segment segment)
                => segment.Position;
        }

        private List<Segment> ns_Segments = new List<Segment>();
        private Vector3 segmentStartPos;
        private WaitForSeconds wfs_CheckForChanges = new WaitForSeconds(0.2f);

        private float previousHeight;
        private int previousCount;
    }
}
