using UnityEngine;
using WarWolfWorks.Interfaces;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Camera Manager for 2D use. Can be explicitly converted to FollowBehaviour, TransformLimiter and Camera.
    /// </summary>
    [AddComponentMenu(IN_ASSETMENU_WARWOLFWORKS + IN_ASSETMENU_UTILITY + nameof(CameraManager2D)), RequireComponent(typeof(FollowBehavior), typeof(PositionLimiter), typeof(Camera))]
    public class CameraManager2D : MonoBehaviour, IPosition, IEulerAngles, IRotation
    {
        /// <summary>
        /// Explicitly returns the <see cref="CameraManager2D"/>'s <see cref="h_FollowBehavior"/>.
        /// </summary>
        /// <param name="cm2d"></param>
        public static explicit operator FollowBehavior(CameraManager2D cm2d)
            => cm2d.h_FollowBehavior;
        /// <summary>
        /// Explicitly returns the <see cref="CameraManager2D"/>'s <see cref="h_TransformLimiter"/>.
        /// </summary>
        /// <param name="cm2d"></param>
        public static explicit operator PositionLimiter(CameraManager2D cm2d)
           => cm2d.h_TransformLimiter;
        /// <summary>
        /// Implicitly returns the <see cref="CameraManager2D"/>'s Camera it is attached to.
        /// </summary>
        /// <param name="cm2d"></param>
        public static implicit operator Camera(CameraManager2D cm2d)
            => cm2d.h_Camera;

        /// <summary>
        /// The default size of the camera, used in cases of exceptions or <see cref="SetCameraSizeDefault"/>.
        /// </summary>
        [Tooltip("The size towards which the CameraManager2D will go to by default.")]
        public float CameraDefaultSize = 8f;

        /// <summary>
        /// The size towards which the camera's orthographic size will move.
        /// </summary>
        [Tooltip("Desired size of the camera.")]
        public float CameraDestinationSize = 8f;

        /// <summary>
        /// Speed at which the camera's orthographic size moves towards <see cref="CameraDestinationSize"/>.
        /// </summary>
        [Tooltip("Speed at which the camera will resize towards the desired size.")]
        public float CameraResizeSpeed = 1f;

        /// <summary>
        /// Current size of the camera.
        /// </summary>
        public virtual float CurrentSize
        {
            get => h_Camera.orthographicSize;
            protected set => h_Camera.orthographicSize = value;
        }

        /// <summary>
        /// Returns the size currently applied to the camera; This is useful if you're using cinemachine or
        /// other components which override the camera size in lateupdates.
        /// </summary>
        /// <returns></returns>
        public float GetCurrentlyAppliedSize() => pr_FuckUnity;

        /// <summary>
        /// Pointer to transform.position.
        /// </summary>
        public Vector3 Position { get => transform.position; set => transform.position = value; }
        /// <summary>
        /// Pointer to transform.rotation.
        /// </summary>
        public Quaternion Rotation { get => transform.rotation; set => transform.rotation = value; }
        /// <summary>
        /// Pointer to transform.eulerAngles.
        /// </summary>
        public Vector3 EulerAngles { get => transform.eulerAngles; set => transform.eulerAngles = value; }

        /// <summary>
        /// Returns the camera attached to this camera manager.
        /// </summary>
        /// <returns></returns>
        public Camera GetCamera() => h_Camera;
        /// <summary>
        /// Returns the <see cref="FollowBehavior"/> attached to this camera manager.
        /// </summary>
        /// <returns></returns>
        public FollowBehavior GetFollowBehavior() => h_FollowBehavior;
        /// <summary>
        /// Returns the <see cref="PositionLimiter"/> attached to this camera manager.
        /// </summary>
        /// <returns></returns>
        public PositionLimiter GetLimiter() => h_TransformLimiter;

        /// <summary>
        /// Sets the camera size to it's initial value.
        /// </summary>
        public void SetCameraSizeDefault() => CameraDestinationSize = CameraDefaultSize;

        /// <summary>
        /// Sets internal components; Make sure to include "base.Awake();" when overriding.
        /// </summary>
        protected virtual void Awake()
        {
            h_FollowBehavior = GetComponent<FollowBehavior>();
            h_TransformLimiter = GetComponent<PositionLimiter>();
            h_TransformLimiter.Is2D = true;
            h_Camera = GetComponent<Camera>();
            h_Camera.orthographic = true;
        }

        /// <summary>
        /// Sets the camera's orthographic size to the default size.
        /// </summary>
        protected virtual void Start()
        {
            h_Camera.orthographicSize = pr_FuckUnity = CameraDefaultSize;
        }

        /// <summary>
        /// Sets the camera's orthoghraphic size progressively towards <see cref="CameraDestinationSize"/> based on <see cref="CameraResizeSpeed"/>.
        /// </summary>
        protected virtual void Update()
        {
            pr_FuckUnity = Mathf.MoveTowards(pr_FuckUnity, CameraDestinationSize, CameraResizeSpeed * Time.deltaTime);
            CurrentSize = pr_FuckUnity;
        }

        /// <summary>
        /// The internal value (middleman value) which applies the progression of the orthographic camera's size based on <see cref="CameraResizeSpeed"/> and <see cref="CameraDestinationSize"/>.
        /// </summary>
        protected float pr_FuckUnity = 0;

        internal FollowBehavior h_FollowBehavior;
        internal PositionLimiter h_TransformLimiter;
        internal Camera h_Camera;

    }
}
