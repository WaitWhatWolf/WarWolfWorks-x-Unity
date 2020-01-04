using UnityEngine;
using System.Collections.Generic;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Limits a transform's position to specified positions or transforms.
    /// (Explicitly convertible to Transform)
    /// </summary>
    [AddComponentMenu("WWW/Utility/Transform Limiter")]
    public sealed class TransformLimiter : MonoBehaviour
    {
        [SerializeField]
        private bool is2D;
        public bool Is2D
        {
            get => is2D;
            set => is2D = value;
        }

        public enum LimitationType
        {
            position,
            transform,
        }

        public LimitationType LimitType;

        [SerializeField]
        private List<Transform> limiters = new List<Transform>();
        public List<Transform> Limiters
        {
            get => limiters;
            set => limiters = value;
        }

        [SerializeField]
        private Vector3 boundsMin;
        public Vector3 MinBounds
        {
            get => boundsMin;
            set => boundsMin = value;
        }
        [SerializeField]
        private Vector3 boundsMax;
        public Vector3 MaxBounds
        {
            get => boundsMax;
            set => boundsMax = value;
        }
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        private void LimitWithPosition(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            Position = new Vector3(
                Mathf.Clamp(Position.x, minX, maxX), //x value
                Mathf.Clamp(Position.y, minY, maxY),
                Is2D ? Position.z : Mathf.Clamp(Position.z, minZ, maxZ)
                );
        }

        private void LimitWithPosition(Vector3 min, Vector3 max)
            => LimitWithPosition(min.x, min.y, min.z, max.x, max.y, max.z);

        private (Vector3 min, Vector3 max) GetLimitWithTransforms()
        {
            if (Limiters.Count == 0)
                goto OriginReturner;

            Transform posUseTransform = Limiters.Find(l => l != null);
            if (posUseTransform == null)
            {
                Limiters = Limiters.RemoveNull();
                goto OriginReturner;
            }

            Vector3 PosUse = posUseTransform.position;
            float minX = PosUse.x,
                maxX = PosUse.x,
                minY = PosUse.y,
                maxY = PosUse.y,
                minZ = PosUse.z,
                maxZ = PosUse.z;


            foreach (Transform t in Limiters)
            {
                try
                {
                    if (t.position.x < minX) minX = t.position.x;
                    else if (t.position.x > maxX) maxX = t.position.x;

                    if (t.position.y < minY) minY = t.position.y;
                    else if (t.position.y > maxY) maxY = t.position.y;

                    if (t.position.z < minZ) minZ = t.position.z;
                    else if (t.position.z > maxZ) maxZ = t.position.z;
                }
                catch { continue; }
            }

            float verif = minX + maxX + minY + maxY + minZ + maxZ;
            
            return (new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        OriginReturner:
            return (Position, Position);
        }

        private void FixedUpdate()
        {
            switch (LimitType)
            {
                case LimitationType.position:
                    LimitWithPosition(MinBounds, MaxBounds);
                    break;
                case LimitationType.transform:
                    (Vector3 min, Vector3 max) pos = GetLimitWithTransforms();
                    LimitWithPosition(pos.min, pos.max);
                    break;
            }
        }

        public static explicit operator Transform(TransformLimiter tl)
            => tl.transform;
    }
}