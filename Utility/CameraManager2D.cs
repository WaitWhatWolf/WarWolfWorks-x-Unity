using UnityEngine;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Camera Manager for 2D use. Can be explicitly converted to FollowBehaviour, TransformLimiter and Camera.
    /// </summary>
    [RequireComponent(typeof(FollowBehaviour), typeof(TransformLimiter), typeof(Camera))]
    public sealed class CameraManager2D : MonoBehaviour
    {
        internal FollowBehaviour FollowBehaviour;
        internal TransformLimiter TransformLimiter;
        internal Camera Camera;

        /// <summary>
        /// Default size of the camera.
        /// </summary>
        public float DefaultSize { get; private set; }
        /// <summary>
        /// Current size of the camera.
        /// </summary>
        public float CurrentSize
        {
            get => Camera.orthographicSize;
            private set => Camera.orthographicSize = value;
        }
        /// <summary>
        /// Size towards which the camera is scaling.
        /// </summary>
        public float DelayedSize { get; private set; }
        /// <summary>
        /// The speed at which the camera is currently resizing.
        /// </summary>
        public float DelayedSpeed { get; private set; }

        private void Awake()
        {
            FollowBehaviour = GetComponent<FollowBehaviour>();
            TransformLimiter = GetComponent<TransformLimiter>();
            TransformLimiter.Is2D = true;
            Camera = GetComponent<Camera>();
            Camera.orthographic = true;
            DelayedSize = DefaultSize = Camera.orthographicSize;
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
        /// Explicitly returns the <see cref="CameraManager2D"/>'s <see cref="FollowBehaviour"/>.
        /// </summary>
        /// <param name="cm2d"></param>
        public static explicit operator FollowBehaviour(CameraManager2D cm2d)
            => cm2d.FollowBehaviour;
        /// <summary>
        /// Explicitly returns the <see cref="CameraManager2D"/>'s <see cref="TransformLimiter"/>.
        /// </summary>
        /// <param name="cm2d"></param>
        public static explicit operator TransformLimiter(CameraManager2D cm2d)
           => cm2d.TransformLimiter;
        /// <summary>
        /// Implicitly returns the <see cref="CameraManager2D"/>'s Camera it is attached to.
        /// </summary>
        /// <param name="cm2d"></param>
        public static implicit operator Camera(CameraManager2D cm2d)
            => cm2d.Camera;
    }
}
