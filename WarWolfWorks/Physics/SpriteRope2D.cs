#pragma warning disable 0649

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Physics
{
    /// <summary>
    /// Simulates a rope in 2D space with sprites.
    /// Note that moving the rope's transform will not move the rope; To simulate physics of the rope
    /// while moving it, move it's <see cref="GetFirstSegment()"/>.
    /// </summary>
    [AddComponentMenu(IN_ASSETMENU_WARWOLFWORKS + IN_ASSETMENU_PHYSICS + nameof(SpriteRope2D)), CompleteNoS]
    public sealed class SpriteRope2D : MonoBehaviour
    {
        /// <summary>
        /// Returns all segments of this rope.
        /// </summary>
        /// <returns></returns>
        public Segment[] GetSegments() => ns_Segments;

        /// <summary>
        /// Returns the first segment of the rope.
        /// </summary>
        /// <returns></returns>
        public Segment GetFirstSegment() => ns_FirstSegment;
        /// <summary>
        /// Returns the last segment of the rope.
        /// </summary>
        /// <returns></returns>
        public Segment GetLastSegment() => ns_LastSegment;

        /// <summary>
        /// The segment class used on all segments of a <see cref="SpriteRope2D"/> component.
        /// </summary>
        [RequireComponent(typeof(HingeJoint2D), typeof(SpriteRenderer), typeof(Rigidbody2D))]
        public sealed class Segment : MonoBehaviour
        {
            internal static readonly FloatRange NoLimitRange = new FloatRange(0f, 360f);

            /// <summary>
            /// The <see cref="HingeJoint2D"/> attached to this segment.
            /// </summary>
            [HideInInspector]
            public HingeJoint2D Joint;
            /// <summary>
            /// The <see cref="SpriteRenderer"/> attached to this segment.
            /// </summary>
            [HideInInspector]
            public SpriteRenderer Renderer;
            /// <summary>
            /// The <see cref="Rigidbody2D"/> attached to this segment.
            /// </summary>
            [HideInInspector]
            public Rigidbody2D Rigidbody;

            /// <summary>
            /// The <see cref="EdgeCollider2D"/> attached to this segment; 
            /// Keep in mind that this will be null if <see cref="s_UseEdgeCollider"/> was false at instantiation.
            /// </summary>
            [HideInInspector]
            public EdgeCollider2D EdgeCollider;

            /// <summary>
            /// Parent of this segment.
            /// </summary>
            public Segment Parent { get; internal set; }
            /// <summary>
            /// Child of this segment.
            /// </summary>
            public Segment Child { get; internal set; }

            internal void SetParent(Segment to)
            {
                if (to != null)
                {
                    Parent = to;
                    Joint.connectedBody = to.Rigidbody;
                }
            }

            internal void AddEdgeCollider()
            {
                EdgeCollider = gameObject.AddComponent<EdgeCollider2D>();
                Joint.enableCollision = true;
                EdgeCollider.points = new Vector2[]{ new(0f, 0f), new(0f, -Renderer.bounds.size.y)};
            }

            internal void SetSegmentVector(out float y)
            {
                Joint.connectedAnchor = new Vector2(0, y = -Math.Abs(Renderer.sprite.bounds.min.y / transform.parent.localScale.y));
                Joint.autoConfigureConnectedAnchor = true;
            }

            internal void SetJointLimits(FloatRange limits)
            {
                Joint.useLimits = !limits.Equals(NoLimitRange);
                if (Joint.useLimits)
                    Joint.limits = new JointAngleLimits2D()
                    {
                        min = limits.Min,
                        max = limits.Max
                    };
            }

            private void Awake()
            {
                Joint = GetComponent<HingeJoint2D>();
                Renderer = GetComponent<SpriteRenderer>();
                Rigidbody = GetComponent<Rigidbody2D>();
            }
        }

        private void Awake()
        {
            ns_Rigidbody = GetComponent<Rigidbody2D>();

            CheckValidity();
        }

        private void CheckValidity()
        {
            Vector2 validVector = new Vector2(0.5f, 1f);
            IEnumerable<Sprite> sprites = s_StartSprites.Concat(s_MidSprites).Concat(s_EndSprites);
            foreach(Sprite s in sprites)
            {
                if ((s.pivot / s.rect.size) != validVector)
                    throw new Exception("A sprite used in SpriteRope2D did not have it's pivot set to top-center; Aborting...");
            }
        }

        private void Start()
        {
            ns_SelectedCount = s_SegmentCount.GetRandom();

            ns_Segments = new Segment[ns_SelectedCount];

            for(int i = 0; i < ns_SelectedCount; i++)
            {
                GameObject segment = new GameObject("Segment_" + i);
                segment.transform.SetParent(transform);
                ns_Segments[i] = segment.AddComponent<Segment>();
            }

            ns_FirstSegment = InitSegment(0, 0, s_StartSprites, s_StartSegmentLimit);
            ns_LastSegment = InitSegment(ns_Segments.Length - 1, 2, s_EndSprites, s_EndSegmentLimit);

            ns_FirstSegment.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
            ns_FirstSegment.Joint.autoConfigureConnectedAnchor = false;

            for (int i = 1; i < ns_SelectedCount - 1; i++)
            {
                InitSegment(i, 69, s_MidSprites, s_MidSegmentLimit); //hehe, boy
            }

            if (s_UseEdgeCollider)
                Array.ForEach(ns_Segments, s => s.AddEdgeCollider());
        }

        private Segment InitSegment(int index, int type, Sprite[] sprites, FloatRange limits)
        {
            switch (type)
            {
                default:
                    ns_Segments[index].SetParent(ns_Segments[index - 1]);
                    ns_Segments[index].Child = ns_Segments[index + 1];
                    break;
                case 0:
                    ns_Segments[index].Child = ns_Segments[index + 1];
                    break;
                case 2:
                    ns_Segments[index].SetParent(ns_Segments[index - 1]);
                    break;
            }

            ns_Segments[index].Renderer.sprite = Hooks.Random.RandomItem(sprites);
            float spriteY;
            ns_Segments[index].SetSegmentVector(out spriteY);
            ns_Segments[index].transform.localPosition = new Vector3(0, spriteY * index, 0);
            ns_Segments[index].SetJointLimits(limits);
            ns_Segments[index].Rigidbody.mass = s_Mass;
            ns_Segments[index].Rigidbody.gravityScale = s_GravityScale;
            ns_Segments[index].Rigidbody.drag = s_LinearDrag;
            ns_Segments[index].Rigidbody.angularDrag = s_AngularDrag;

            return ns_Segments[index];
        }

        #region Unity Serialized
        [SerializeField, Tooltip("The possible sprites of the first rope segment.")]
        private Sprite[] s_StartSprites;
        [SerializeField, Tooltip("All possible sprites used in mid-segments of the rope.")]
        private Sprite[] s_MidSprites;
        [SerializeField, Tooltip("The possible sprites of the last rope segment.")]
        private Sprite[] s_EndSprites;

        [SerializeField, Tooltip("Amount of segments (excluding first and last) to spawn on this rope.")]
        private IntRange s_SegmentCount = new(1, 99);

        [SerializeField, Tooltip("The angle limit of the first segment; If left at 0-360, it will not limit the angle.")]
        private FloatRange s_StartSegmentLimit = new(0f, 360f);
        [SerializeField, Tooltip("The angle limit of all the mid segments; If left at 0-360, it will not limit the angle.")]
        private FloatRange s_MidSegmentLimit = new(0f, 360f);
        [SerializeField, Tooltip("The angle limit of the last segment; If left at 0-360, it will not limit the angle.")]
        private FloatRange s_EndSegmentLimit = new(0f, 360f);

        [SerializeField, Tooltip("The gravity applied to all rigidbodies in the rope.")]
        private float s_GravityScale = 1f;

        [SerializeField, Tooltip("Mass of all rigidbodies in the rope.")]
        private float s_Mass = 1f;

        [SerializeField, Tooltip("Linear drag applied to all rigidbodies in the rope.")]
        private float s_LinearDrag = 10f;

        [SerializeField, Tooltip("Angular drag applied to all rigidbodies in the rope.")]
        private float s_AngularDrag = 5f;

        [SerializeField, Tooltip("If true, adds an EdgeCollider2D to all segments.")]
        private bool s_UseEdgeCollider;
        #endregion

        private Segment ns_FirstSegment;
        private Segment ns_LastSegment;
        private Segment[] ns_Segments;
        private Rigidbody2D ns_Rigidbody;
        private int ns_SelectedCount;
    }
}
