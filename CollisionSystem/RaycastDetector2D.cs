using System;
using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Debugging;
using WarWolfWorks.Utility;

namespace WarWolfWorks.CollisionSystem
{
    /// <summary>
    /// Used to detect 2D collisions using raycasts.
    /// </summary>
    [AddComponentMenu("WWW/Collision/2D/RaycastDetector")]
    public sealed class RaycastDetector2D : Detector<Collider2D>
    {
        [SerializeField]//[Rename("Display Gizmos")]
        private bool ディスプレイギズモー;
        /// <summary>
        /// If true, all raycasts will be displayed with wire-type gizmos.
        /// </summary>
        public bool DisplayGizmos
        {
            get => ディスプレイギズモー;
            set => ディスプレイギズモー = value;
        }

        [SerializeField]//[Rename("Calculate on FixedUpdate")]
        [Tooltip("If true, all calculations of collision detection will be made inside FixedUpdate. Otherwise, it will be made inside Update.")]
        private bool ヒクセード;
        /// <summary>
        /// Determines if calculations are made inside FixedUpdate or Update.
        /// </summary>
        public bool IsFixedUpdate
        {
            get => ヒクセード;
            set => ヒクセード = value;
        }

        [SerializeField]//[Rename("Raycast Filter")]
        private RaycastFilter2D ヒールター;
        /// <summary>
        /// Filter applied to raycasts.
        /// </summary>
        public RaycastFilter2D RaycastFilter
        {
            get => ヒールター;
            set => ヒールター = value;
        }

        [SerializeField][EnumFlags("Raycasting Type")]
        private RD2DType RD2Dタイプ;

        /// <summary>
        /// What type of raycasting will the <see cref="RaycastDetector2D"/> perform.
        /// </summary>
        public RD2DType RaycastType
        {
            get => RD2Dタイプ;
            set => RD2Dタイプ = value;
        }

        /// <summary>
        /// (ほうめん)
        /// </summary>
        [SerializeField]//[Rename("Raycast Direction")]
        private Vector2 レーキヤスト方面;
        /// <summary>
        /// Direction at which all raycasts will be performed.
        /// </summary>
        public Vector2 RaycastDirection
        {
            get => レーキヤスト方面;
            set => レーキヤスト方面 = value;
        }

        /// <summary>
        /// (きょり)
        /// </summary>
        [SerializeField]//[Rename("Raycast Distance")]
        private float レーキヤスト距離;
        /// <summary>
        /// Distance at which all raycasts will be performed.
        /// </summary>
        public float RaycastDistance
        {
            get => レーキヤスト距離;
            set => レーキヤスト距離 = value;
        }

        /// <summary>
        /// (めんせき)
        /// </summary>
        [SerializeField]//[Rename("Raycast Size")]
        [Tooltip("If raycast takes float value instead of Vector2 to determine size, it will simply take the X value for non-overlap raycasts and Y value for overlap raycasts.")]
        private Vector2 レーキヤスト面積;
        /// <summary>
        /// Size of all raycasts performed (If raycast takes float value instead of Vector2 to determine size, it will simply take the X value for non-overlap raycasts and Y value for overlap raycasts).
        /// </summary>
        public Vector2 RaycastSize
        {
            get => レーキヤスト面積;
            set => レーキヤスト面積 = value;
        }

        [SerializeField]//[Rename("Areacast Point A")]
        private Vector2 打ち合わせ丸キヤストポイントA;
        [SerializeField]//[Rename("Areacast Point B")]
        private Vector2 打ち合わせ丸キヤストポイントB;

        /// <summary>
        /// Point A used for <see cref="Physics2D.OverlapAreaAll(Vector2, Vector2, int, float, float)"/>.
        /// </summary>
        public Vector2 AreaCastPointA
        {
            get => 打ち合わせ丸キヤストポイントA;
            set => 打ち合わせ丸キヤストポイントA = value;
        }

        /// <summary>
        /// Point B used for <see cref="Physics2D.OverlapAreaAll(Vector2, Vector2, int, float, float)"/>.
        /// </summary>
        public Vector2 AreaCastPointB
        {
            get => 打ち合わせ丸キヤストポイントB;
            set => 打ち合わせ丸キヤストポイントB = value;
        }

        /// <summary>
        /// (ちゅうしん)
        /// </summary>
        [SerializeField]//[Rename("Center Point")]
        private Transform 中心ポイント;
        /// <summary>
        /// Origin point of all raycasts.
        /// </summary>
        public Transform CenterPoint
        {
            get => 中心ポイント;
            set => 中心ポイント = value;
        }

        /// <summary>
        /// Equivalent to <see cref="CenterPoint"/>.position.
        /// </summary>
        public Vector2 Center => 中心ポイント.position;

        /// <summary>
        /// No condition, always true.
        /// </summary>
        protected override Predicate<Collidable> コンディション => か => true;

        /// <summary>
        /// Unity's FixedUpdate Method. All calculations are made here.
        /// </summary>
        protected override void FixedUpdate()
        {
            if(ヒクセード) 算定();
        }

        private void Update()
        {
            if (!ヒクセード) 算定();
        }

        /// <summary>
        /// Calculates all raycasts. (さんてい)
        /// </summary>
        private void 算定()
        {
            List<RaycastHit2D> レーイズ = new List<RaycastHit2D>();
            if (RD2Dタイプ.HasFlag(RD2DType.raycast)) レーイズ.AddRange(レーキヤスト());
            if (RD2Dタイプ.HasFlag(RD2DType.boxcast)) レーイズ.AddRange(箱キヤスト());
            if (RD2Dタイプ.HasFlag(RD2DType.circlecast)) レーイズ.AddRange(丸キヤスト());

            List<Collider2D> コールズ = new List<Collider2D>();
            if (RD2Dタイプ.HasFlag(RD2DType.overlapCircle)) コールズ.AddRange(打ち合わせ丸キヤスト());
            if (RD2Dタイプ.HasFlag(RD2DType.overlapArea)) コールズ.AddRange(打ち合わせ辺キヤスト());
            if (RD2Dタイプ.HasFlag(RD2DType.overlapBox)) コールズ.AddRange(打ち合わせ箱キヤスト());

            レーイズ.ForEach(レ =>
            {
                if (レ.collider)
                {
                    if (グループ追加(レ.collider, レ.collider.isTrigger))
                    {
                        エンター呼び出す(レ.collider, レ.collider.isTrigger);
                    }
                    else ソテー呼び出す(レ.collider, レ.collider.isTrigger);
                }
            });

            コールズ.ForEach(コ =>
            {
                if (グループ追加(コ, コ.isTrigger))
                {
                    エンター呼び出す(コ, コ.isTrigger);
                }
                else ソテー呼び出す(コ, コ.isTrigger);
            });

            ホーイチ(チ =>
            {
                if(!レーイズ.Find(アー => アー.collider == チ.コリダー))
                {
                    出口呼び出す(チ.コリダー, チ.トリガーは);
                    グループ削除(チ.コリダー);
                }
            });
        }

        /// <summary>
        /// Performs a raycast.
        /// </summary>
        private RaycastHit2D[] レーキヤスト()
        {
            return Physics2D.RaycastAll(Center, レーキヤスト方面, レーキヤスト距離,
                ヒールター.LayerMask, ヒールター.Depth.Min, ヒールター.Depth.Max);
        }

        /// <summary>
        /// Performs a boxcast. (はこ)
        /// </summary>
        private RaycastHit2D[] 箱キヤスト()
        {
            return Physics2D.BoxCastAll(Center, レーキヤスト面積, 0, レーキヤスト方面, レーキヤスト距離,
                ヒールター.LayerMask, ヒールター.Depth.Min, ヒールター.Depth.Max);
        }

        /// <summary>
        /// Performs a circlecast. (まる)
        /// </summary>
        /// <returns></returns>
        private RaycastHit2D[] 丸キヤスト()
        {
            return Physics2D.CircleCastAll(Center, レーキヤスト面積.x, レーキヤスト方面, レーキヤスト距離,
                ヒールター.LayerMask, ヒールター.Depth.Min, ヒールター.Depth.Max);
        }

        /// <summary>
        /// Performs a overlapcircle cast. (うちあわせまる)
        /// </summary>
        /// <returns></returns>
        private Collider2D[] 打ち合わせ丸キヤスト()
        {
            return Physics2D.OverlapCircleAll(Center, レーキヤスト面積.y, ヒールター.LayerMask,
                ヒールター.Depth.Min, ヒールター.Depth.Max);
        }

        /// <summary>
        /// Performs a overlaparea cast. (うちあわせへん)
        /// </summary>
        /// <returns></returns>
        private Collider2D[] 打ち合わせ辺キヤスト()
        {
            return Physics2D.OverlapAreaAll(打ち合わせ丸キヤストポイントA, 打ち合わせ丸キヤストポイントB, ヒールター.LayerMask,
                ヒールター.Depth.Min, ヒールター.Depth.Max);
        }

        /// <summary>
        /// Performs a overlapbox cast. (うちあわせはこ)
        /// </summary>
        /// <returns></returns>
        private Collider2D[] 打ち合わせ箱キヤスト()
        {
            return Physics2D.OverlapBoxAll(Center, レーキヤスト面積, 0, ヒールター.LayerMask,
                ヒールター.Depth.Min, ヒールター.Depth.Max);
        }

        private void OnDrawGizmosSelected()
        {
            if (ディスプレイギズモー && 中心ポイント)
            {
                Gizmos.color = Color.green;
                Vector3 maxRayPos = Center + (レーキヤスト方面 * レーキヤスト距離);
                if(RD2Dタイプ.HasFlag(RD2DType.raycast)) Gizmos.DrawLine(Center, maxRayPos);
                if (RD2Dタイプ.HasFlag(RD2DType.boxcast)) Gizmos.DrawWireCube(maxRayPos, レーキヤスト面積);
                if (RD2Dタイプ.HasFlag(RD2DType.circlecast)) Gizmos.DrawWireSphere(maxRayPos, レーキヤスト面積.x);

                Gizmos.color = Color.cyan;
                if (RD2Dタイプ.HasFlag(RD2DType.overlapCircle)) Gizmos.DrawWireSphere(Center, レーキヤスト面積.y);
                if (RD2Dタイプ.HasFlag(RD2DType.overlapArea)) Gizmos.DrawWireCube((打ち合わせ丸キヤストポイントA + 打ち合わせ丸キヤストポイントB) / 2, 打ち合わせ丸キヤストポイントB - 打ち合わせ丸キヤストポイントA);
                if (RD2Dタイプ.HasFlag(RD2DType.overlapBox)) Gizmos.DrawWireCube(Center, レーキヤスト面積);
            }
        }
    }
}
