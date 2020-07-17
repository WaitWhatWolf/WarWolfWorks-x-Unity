using UnityEngine;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Camera Manager for 2D use. Can be explicitly converted to FollowBehaviour, TransformLimiter and Camera.
    /// </summary>
    [RequireComponent(typeof(FollowBehavior), typeof(TransformLimiter), typeof(Camera))]
    public sealed class CameraManager2D : MonoBehaviour, IPosition, IEulerAngles, IRotation
    {
        internal FollowBehavior h_FollowBehavior;
        internal TransformLimiter h_TransformLimiter;
        internal Camera h_Camera;

        /// <summary>
        /// Default size of the camera.
        /// </summary>
        public float DefaultSize { get; private set; }
        /// <summary>
        /// Current size of the camera.
        /// </summary>
        public float CurrentSize
        {
            get => h_Camera.orthographicSize;
            private set => h_Camera.orthographicSize = value;
        }
        /// <summary>
        /// Size towards which the camera is scaling.
        /// </summary>
        public float DelayedSize { get; private set; }
        /// <summary>
        /// The speed at which the camera is currently resizing.
        /// </summary>
        public float DelayedSpeed { get; private set; }
        
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
        /// Returns the <see cref="TransformLimiter"/> attached to this camera manager.
        /// </summary>
        /// <returns></returns>
        public TransformLimiter GetLimiter() => h_TransformLimiter;

        private void Awake()
        {
            h_FollowBehavior = GetComponent<FollowBehavior>();
            h_TransformLimiter = GetComponent<TransformLimiter>();
            h_TransformLimiter.Is2D = true;
            h_Camera = GetComponent<Camera>();
            h_Camera.orthographic = true;
            DelayedSize = DefaultSize = h_Camera.orthographicSize;
        }

        /// <summary>
        /// Sets camera size to the specified size.
        /// </summary>
        /// <param name="size"></param>
        public void SetCameraSize(float size)
        {
            CurrentSize = size;
        }

        /// <summary>
        /// Sets camera size to the specified size and end it's delayed scaling.
        /// </summary>
        /// <param name="size"></param>
        public void SetCameraSizeFull(float size)
        {
            DelayedSpeed = 0;
            CurrentSize = DelayedSize = size;
        }

        /// <summary>
        /// Sets the camera size to the specified size with speed.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="speed"></param>
        public void SetDelayedSize(float size, float speed)
        {
            DelayedSize = size;
            DelayedSpeed = speed;
        }

        /// <summary>
        /// Sets the camera size to it's initial value.
        /// </summary>
        public void SetCameraSizeDefault() => SetCameraSize(DefaultSize);

        private void Update()
        {
            CurrentSize = Hooks.WWWMath.MoveTowards(CurrentSize, DelayedSize, DelayedSpeed);
        }

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
        public static explicit operator TransformLimiter(CameraManager2D cm2d)
           => cm2d.h_TransformLimiter;
        /// <summary>
        /// Implicitly returns the <see cref="CameraManager2D"/>'s Camera it is attached to.
        /// </summary>
        /// <param name="cm2d"></param>
        public static implicit operator Camera(CameraManager2D cm2d)
            => cm2d.h_Camera;
    }
}
