using System;
using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Interfaces;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Camera manager for an FPS game.
    /// </summary>
    [AddComponentMenu(IN_ASSETMENU_WARWOLFWORKS + IN_ASSETMENU_UTILITY + nameof(CameraManagerFPS)), RequireComponent(typeof(Camera))]
    public sealed class CameraManagerFPS : MonoBehaviour, IPosition, IEulerAngles, IRotation
    {
        /// <summary>
        /// Returns implicitly the Camera component.
        /// </summary>
        /// <param name="cmfps"></param>
        public static implicit operator Camera(CameraManagerFPS cmfps)
            => cmfps.pv_Camera;
        
        /// <summary>
        /// Sensitivity of the camera controller's horizontal axis.
        /// </summary>
        [FormerlySerializedAs("s_SensitivityHorizontal")]
        public float SensitivityHorizontal;
        /// <summary>
        /// Sensitivity of the camera controller's vertical axis.
        /// </summary>
        [FormerlySerializedAs("s_SensitivityVertical")]
        public float SensitivityVertical;
        /// <summary>
        /// Sets the maximum rotation allowed vertically.
        /// </summary>
        [FormerlySerializedAs("s_MaxVerticalRotation"), SerializeField, Range(0, 180)]
        public float MaxVerticalRotation;

        /// <summary>
        /// If true, the horizontal axis of the camera rotation will be reversed.
        /// </summary>
        [FormerlySerializedAs("s_InversedHorizontal")]
        public bool InversedHorizontal;
        /// <summary>
        /// If true, the vertical axis of the camera rotation will be reversed.
        /// </summary>
        [FormerlySerializedAs("s_InversedVertical")]
        public bool InversedVertical;

        /// <summary>
        /// Describes the lock state of the mouse when it's first instantiated in the scene.
        /// </summary>
        [FormerlySerializedAs("s_LockStateStart")]
        public CursorLockMode LockStateStart;


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
        public Camera GetCamera() => pv_Camera;

        /// <summary>
        /// Returns true if an axis is inverted.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool IsInversed(RectTransform.Axis axis)
            => axis switch
               {
                   RectTransform.Axis.Vertical => InversedVertical,
                   RectTransform.Axis.Horizontal => InversedHorizontal,
                   _ => false,
                };

        /// <summary>
        /// Inverts the controls of an axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="to"></param>
        public void SetInversed(RectTransform.Axis axis, bool to)
        {
            switch (axis)
            {
                default:  break;
                case RectTransform.Axis.Horizontal: InversedHorizontal = to; break;
                case RectTransform.Axis.Vertical: InversedVertical = to; break;
            }
        }

        /// <summary>
        /// Sets the Field of View of the camera.
        /// </summary>
        /// <param name="degreesDelta"></param>
        public void SetFOV(float degreesDelta)
        {
            pv_Camera.fieldOfView = degreesDelta;
        }

        private void Awake()
        {
            pv_IRotatable = GetComponentInParent<IRotatable>();
            pv_Camera = GetComponent<Camera>();
            Cursor.lockState = LockStateStart;
        }

        private Vector3 GetMouseRotation()
        {
            Vector3 CurrentPos = Input.mousePosition - pv_PreviousPos;

            Vector3 toReturn;

            float MouseY = Input.GetAxis("Mouse Y");
            float MouseX = Input.GetAxis("Mouse X");

            switch (Cursor.lockState)
            {
                default:
                    toReturn = new Vector3(
                    Hooks.MathF.ClampAngle(pv_IRotatable.ToRotateX.localEulerAngles.x +
                    (InversedVertical ? (CurrentPos.y * SensitivityVertical) : -(CurrentPos.y * SensitivityVertical)), -(MaxVerticalRotation / 2), MaxVerticalRotation / 2),
                    pv_IRotatable.ToRotateY.localEulerAngles.y +
                    (InversedHorizontal ? -(CurrentPos.x * SensitivityHorizontal) : (CurrentPos.x * SensitivityHorizontal)), 0);
                    break;
                case CursorLockMode.Locked:
                    toReturn = new Vector3(
                    Hooks.MathF.ClampAngle(pv_IRotatable.ToRotateX.localEulerAngles.x +
                    (InversedVertical ? MouseY * SensitivityVertical
                    : -MouseY * SensitivityVertical), -(MaxVerticalRotation / 2), MaxVerticalRotation / 2),
                    pv_IRotatable.ToRotateY.localEulerAngles.y + (InversedHorizontal ? -MouseX * SensitivityHorizontal :
                    MouseX * SensitivityHorizontal), pv_IRotatable.ToRotateZ.localEulerAngles.z);
                    break;
            }

            pv_PreviousPos = Input.mousePosition;
            return toReturn;
        }

        private void Update()
        {
            try 
            {
                Vector3 rotation = GetMouseRotation();
                if (rotation != Hooks.MathF.Zero)
                    pv_IRotatable.SetRotation(rotation); 
            }
            catch (NullReferenceException)
            {
                AdvancedDebug.LogError($"No IRotation component was found in {gameObject.name}, " +
                  $"make sure you set one by either setting a premade WWWLibrary script (MonoRotation or NyuRotation) " +
                  $"or making one by implementing the IRotation interface.", DEBUG_LAYER_EXCEPTIONS_INDEX);
            }
        }

        private IRotatable pv_IRotatable;
        private Camera pv_Camera;

        private Vector3 pv_PreviousPos;
    }
}
