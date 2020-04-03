using System;
using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Camera manager for an FPS game.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class CameraManagerFPS : MonoBehaviour, IPosition, IEulerAngles, IRotation
    {
        private IRotatable Rotatable;
        private Camera Camera;

        [FormerlySerializedAs("sensitivityHorizontal"), SerializeField]
        private float s_SensitivityHorizontal;
        [FormerlySerializedAs("sensitivityVertical"), SerializeField]
        private float s_SensitivityVertical;
        /// <summary>
        /// Sensitivity of the camera controller's horizontal axis.
        /// </summary>
        public float SensitivityHorizontal { get => s_SensitivityHorizontal; set => s_SensitivityHorizontal = value; }
        /// <summary>
        /// Sensitivity of the camera controller's vertical axis.
        /// </summary>
        public float SensitivityVertical { get => s_SensitivityVertical; set => s_SensitivityVertical = value; }
        [FormerlySerializedAs("maxVerticalRotation"), SerializeField, Range(0, 180)]
        private float s_MaxVerticalRotation;
        /// <summary>
        /// Sets the maximum rotation allowed vertically.
        /// </summary>
        public float MaxVerticalRotation { get => s_MaxVerticalRotation; set => s_MaxVerticalRotation = Mathf.Clamp(value, 0, 180); }

        private Vector3 previousPos;

        [FormerlySerializedAs("InversedHorizontal"), SerializeField]
        private bool s_InversedHorizontal;
        [FormerlySerializedAs("InversedVertical"), SerializeField]
        private bool s_InversedVertical;

        [FormerlySerializedAs("LockStateStart"), SerializeField]
        private CursorLockMode s_LockStateStart;


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
        /// Returns true if an axis is inverted.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool IsInversed(RectTransform.Axis axis)
        {
            switch(axis)
            {
                default: return s_InversedHorizontal;
                case RectTransform.Axis.Vertical: return s_InversedVertical;
            }
        }

        /// <summary>
        /// Inverts the controls of an axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="to"></param>
        public void SetInversed(RectTransform.Axis axis, bool to)
        {
            switch (axis)
            {
                default: s_InversedHorizontal = to; break;
                case RectTransform.Axis.Vertical: s_InversedVertical = to; break;
            }
        }


        private void Awake()
        {
            Rotatable = GetComponentInParent<IRotatable>();
            Camera = GetComponent<Camera>();
            Cursor.lockState = s_LockStateStart;
        }

        private Vector3 GetMouseRotation()
        {
            Vector3 CurrentPos = Input.mousePosition - previousPos;

            Vector3 toReturn;

            float MouseY = Input.GetAxis("Mouse Y");
            float MouseX = Input.GetAxis("Mouse X");

            switch (Cursor.lockState)
            {
                default:
                    toReturn = new Vector3(
                    Hooks.WWWMath.ClampAngle(Rotatable.ToRotateX.localEulerAngles.x +
                    (s_InversedVertical ? (CurrentPos.y * SensitivityVertical) : -(CurrentPos.y * SensitivityVertical)), -(MaxVerticalRotation / 2), MaxVerticalRotation / 2),
                    Rotatable.ToRotateY.localEulerAngles.y +
                    (s_InversedHorizontal ? -(CurrentPos.x * SensitivityHorizontal) : (CurrentPos.x * SensitivityHorizontal)), 0);
                    break;
                case CursorLockMode.Locked:
                    toReturn = new Vector3(
                    Hooks.WWWMath.ClampAngle(Rotatable.ToRotateX.localEulerAngles.x +
                    (s_InversedVertical ? MouseY * SensitivityVertical
                    : -MouseY * SensitivityVertical), -(MaxVerticalRotation / 2), MaxVerticalRotation / 2),
                    Rotatable.ToRotateY.localEulerAngles.y + (s_InversedHorizontal ? -MouseX * SensitivityHorizontal :
                    MouseX * SensitivityHorizontal), Rotatable.ToRotateZ.localEulerAngles.z);
                    break;
            }

            previousPos = Input.mousePosition;
            return toReturn;
        }

        private void Update()
        {
            try { Rotatable.SetRotation(GetMouseRotation()); }
            catch(NullReferenceException)
            { AdvancedDebug.LogError($"No IRotation component was found in {gameObject.name}, " +
                $"make sure you set one by either setting a premade WWWLibrary script (Rotation or EntityRotation) " +
                $"or making one by implementing the IRotation interface.", AdvancedDebug.DEBUG_LAYER_EXCEPTIONS_INDEX); }
        }

        /// <summary>
        /// Sets the Field of View of the camera.
        /// </summary>
        /// <param name="degreesDelta"></param>
        public void SetFOV(float degreesDelta)
        {
            Camera.fieldOfView = degreesDelta;
        }

        /// <summary>
        /// Returns implicitly the Camera component.
        /// </summary>
        /// <param name="cmfps"></param>
        public static implicit operator Camera(CameraManagerFPS cmfps)
            => cmfps.Camera;
    }
}
