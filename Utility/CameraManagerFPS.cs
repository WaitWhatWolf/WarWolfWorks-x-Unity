using System;
using UnityEngine;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Camera manager for an FPS game.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraManagerFPS : MonoBehaviour, IGetter<CameraManagerFPS>
    {
        CameraManagerFPS IGetter<CameraManagerFPS>.Drawer => this;
        
        private IRotation Rotation;
        private Camera Camera;

        [SerializeField]
        private float sensitivityHorizontal, sensitivityVertical;
        /// <summary>
        /// Sensitivity of the camera controller's horizontal axis.
        /// </summary>
        public float SensitivityHorizontal { get => sensitivityHorizontal; set => sensitivityHorizontal = value; }
        /// <summary>
        /// Sensitivity of the camera controller's vertical axis.
        /// </summary>
        public float SensitivityVertical { get => sensitivityVertical; set => sensitivityVertical = value; }
        [SerializeField]
        [Range(0, 180)]
        private float maxVerticalRotation;
        /// <summary>
        /// Sets the maximum rotation allowed vertically.
        /// </summary>
        public float MaxVerticalRotation { get => maxVerticalRotation; set => maxVerticalRotation = Mathf.Clamp(value, 0, 180); }

        private Vector3 previousPos;

        [SerializeField]
        private bool InversedHorizontal, InversedVertical;

        [SerializeField]
        private CursorLockMode LockStateStart;

        /// <summary>
        /// Returns true if an axis is inverted.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool IsInversed(RectTransform.Axis axis)
        {
            switch(axis)
            {
                default: return InversedHorizontal;
                case RectTransform.Axis.Vertical: return InversedVertical;
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
                default: InversedHorizontal = to; break;
                case RectTransform.Axis.Vertical: InversedVertical = to; break;
            }
        }


        private void Awake()
        {
            Rotation = GetComponentInParent<IRotation>();
            Camera = GetComponent<Camera>();
            Cursor.lockState = LockStateStart;
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
                    Hooks.WWWMath.ClampAngle(Rotation.ToRotateX.localEulerAngles.x +
                    (InversedVertical ? (CurrentPos.y * SensitivityVertical) : -(CurrentPos.y * SensitivityVertical)), -(MaxVerticalRotation / 2), MaxVerticalRotation / 2),
                    Rotation.ToRotateY.localEulerAngles.y +
                    (InversedHorizontal ? -(CurrentPos.x * SensitivityHorizontal) : (CurrentPos.x * SensitivityHorizontal)), 0);
                    break;
                case CursorLockMode.Locked:
                    toReturn = new Vector3(
                    Hooks.WWWMath.ClampAngle(Rotation.ToRotateX.localEulerAngles.x +
                    (InversedVertical ? MouseY * SensitivityVertical
                    : -MouseY * SensitivityVertical), -(MaxVerticalRotation / 2), MaxVerticalRotation / 2),
                    Rotation.ToRotateY.localEulerAngles.y + (InversedHorizontal ? -MouseX * SensitivityHorizontal :
                    MouseX * SensitivityHorizontal), Rotation.ToRotateZ.localEulerAngles.z);
                    break;
            }

            previousPos = Input.mousePosition;
            return toReturn;
        }

        private void Update()
        {
            try { Rotation.SetRotation(GetMouseRotation()); }
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
