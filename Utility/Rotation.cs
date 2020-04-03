using UnityEngine;
using WarWolfWorks.Interfaces;
using System;
using WarWolfWorks.Attributes;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Utility component for rotation.
    /// </summary>
    [CompleteNoS]
	public class Rotation : MonoBehaviour, IRotatable, ILockable
	{
        private Quaternion RotationToApply;
        /// <summary>
        /// Rotation towards which this <see cref="Rotation"/> is destinated.
        /// </summary>
        public Quaternion GetDestination() => RotationToApply;
        /// <summary>
        /// The Currently applied rotation.
        /// </summary>
        public Quaternion CurrentRotation => transform.rotation;
        /// <summary>
        /// The Euler Rotation that's used as the default rotation.
        /// </summary>
        public virtual Vector3 DefaultEulerRotation => Vector3.zero;

        /// <summary>
        /// Determines the base rotation speed of this <see cref="Rotation"/>.
        /// </summary>
        [SerializeField]
        internal float s_BaseRotationSpeed;
        /// <summary>
        /// Pointer to <see cref="s_BaseRotationSpeed"/>, overridable.
        /// </summary>
        protected virtual float BaseRotationSpeed => s_BaseRotationSpeed;
        /// <summary>
        /// The absolute RotationSpeed used by this <see cref="Rotation"/>.
        /// </summary>
        public float RotationSpeed => Locked ? 0 : BaseRotationSpeed * Time.deltaTime * 60;


#pragma warning disable 0649
        [SerializeField]
        internal Transform s_ToRotateX, s_ToRotateY, s_ToRotateZ;
#pragma warning restore 0649

        /// <summary>
        /// The object's Lock state; See <see cref="ILockable"/> for more info.
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// Called when the object is locked (<see cref="ILockable"/> implementation).
        /// </summary>
        public event Action<ILockable> OnLocked;
        /// <summary>
        /// Called when the object is unlocked (<see cref="ILockable"/> implementation).
        /// </summary>
        public event Action<ILockable> OnUnlocked;

        /// <summary>
        /// Locks or Unlocks this object (<see cref="ILockable"/> implementation).
        /// </summary>
        /// <param name="to"></param>
        public void SetLock(bool to)
        {
            if (to == Locked)
                return;

            Locked = to;
            if (Locked) OnLocked?.Invoke(this);
            else OnUnlocked?.Invoke(this);
        }

        /// <summary>
        /// <see cref="Transform"/> which will be used for the X rotation.
        /// </summary>
        public Transform ToRotateX { get => s_ToRotateX; internal set => s_ToRotateX = value; }
        /// <summary>
        /// <see cref="Transform"/> which will be used for the Y rotation.
        /// </summary>
        public Transform ToRotateY { get => s_ToRotateY; internal set => s_ToRotateY = value; }
        /// <summary>
        /// <see cref="Transform"/> which will be used for the Z rotation.
        /// </summary>
        public Transform ToRotateZ { get => s_ToRotateZ; internal set => s_ToRotateZ = value; }
        /// <summary>
        /// Unity's Update method, used by the <see cref="Rotation"/> to apply it's transform.rotation.
        /// </summary>
        internal virtual void Update()
        {
            if (Locked)
                return;

            Vector3 eulerRot = new Vector3(ToRotateX.eulerAngles.x, ToRotateY.eulerAngles.y, ToRotateZ.eulerAngles.z);
            Quaternion rot = Quaternion.RotateTowards(Quaternion.Euler(eulerRot), RotationToApply, RotationSpeed);

            Vector3 covertRotEuler = rot.eulerAngles;
            try
            {
                ToRotateX.eulerAngles = new Vector3(covertRotEuler.x, ToRotateX.eulerAngles.y, ToRotateX.eulerAngles.z);
                ToRotateY.eulerAngles = new Vector3(ToRotateY.eulerAngles.x, covertRotEuler.y, ToRotateY.eulerAngles.z);
                ToRotateZ.eulerAngles = new Vector3(ToRotateZ.eulerAngles.x, ToRotateZ.eulerAngles.y, covertRotEuler.z);
            }
            catch //(Exception e) when (e is MissingReferenceException || e is NullReferenceException)
            {
                //In case of a missingreferenceexception, it will first verify if a rotator is null before using it.
                //(This is here for performance's sake, not pretty code)
                if(ToRotateX)ToRotateX.eulerAngles = new Vector3(covertRotEuler.x, ToRotateX.eulerAngles.y, ToRotateX.eulerAngles.z);
                if(ToRotateY)ToRotateY.eulerAngles = new Vector3(ToRotateY.eulerAngles.x, covertRotEuler.y, ToRotateY.eulerAngles.z);
                if(ToRotateZ)ToRotateZ.eulerAngles = new Vector3(ToRotateZ.eulerAngles.x, ToRotateZ.eulerAngles.y, covertRotEuler.z);
            }
        }

        /// <summary>
        /// Gets the rotated transform of the specified axis. Returns false if null. (Doesn't support Axis Flags)
        /// </summary>
        /// <param name="rotator"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool TryGetRotator(out Transform rotator, Axis axis)
        {
            if (axis == Axis.None)
                rotator = null;
            else
            switch(axis)
            {
                default: rotator = ToRotateX; break;
                    case Axis.Y: rotator = ToRotateY; break;
                    case Axis.Z: rotator = ToRotateZ; break;
            }

            return rotator;
        }

        /// <summary>
        /// Sets the rotated transform of the specified Axis. (Can be set to null; Supports Axis Flags)
        /// </summary>
        /// <param name="to"></param>
        /// <param name="of"></param>
        public void SetRotator(Transform to, Axis of)
        {
            if (of.HasFlag(Axis.X)) ToRotateX = to;
            if (of.HasFlag(Axis.Y)) ToRotateY = to;
            if (of.HasFlag(Axis.Z)) ToRotateZ = to;
        }

        /// <summary>
        /// Sets the destination of this <see cref="Rotation"/>.
        /// </summary>
        /// <param name="toApply"></param>
        public void SetRotation(Quaternion toApply)
        {
            Vector3 applier = toApply.eulerAngles + DefaultEulerRotation;
            RotationToApply = Quaternion.Euler(applier);
        }

        /// <summary>
        /// Sets the destination of this <see cref="Rotation"/> in euler angles.
        /// </summary>
        /// <param name="toApply"></param>
        public void SetRotation(Vector3 toApply)
        {
            RotationToApply = Quaternion.Euler(DefaultEulerRotation + toApply);
        }
    }
}