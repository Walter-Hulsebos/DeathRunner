#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using UnityEngine;

namespace UnityTemplateProjects
{
    public class SimpleCameraController : MonoBehaviour
    {
        private class CameraState
        {
            public float yaw;
            public float pitch;
            public float roll;
            public float x;
            public float y;
            public float z;

            public void SetFromTransform(Transform t)
            {
                pitch = t.eulerAngles.x;
                yaw = t.eulerAngles.y;
                roll = t.eulerAngles.z;
                x = t.position.x;
                y = t.position.y;
                z = t.position.z;
            }

            public void Translate(Vector3 translation)
            {
                Vector3 __rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

                x += __rotatedTranslation.x;
                y += __rotatedTranslation.y;
                z += __rotatedTranslation.z;
            }

            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
            {
                yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
                pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
                roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);
                
                x = Mathf.Lerp(x, target.x, positionLerpPct);
                y = Mathf.Lerp(y, target.y, positionLerpPct);
                z = Mathf.Lerp(z, target.z, positionLerpPct);
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(pitch, yaw, roll);
                t.position = new Vector3(x, y, z);
            }
        }

        private const float _K_MOUSE_SENSITIVITY_MULTIPLIER = 0.01f;

        private CameraState _mTargetCameraState = new();
        private CameraState _mInterpolatingCameraState = new();

        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        public float boost = 3.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        public float positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("Multiplier for the sensitivity of the rotation.")]
        public float mouseSensitivity = 60.0f;

        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve mouseSensitivityCurve = new(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.01f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
        public bool invertY = false;

#if ENABLE_INPUT_SYSTEM
        private InputAction _movementAction;
        private InputAction _verticalMovementAction;
        private InputAction _lookAction;
        private InputAction _boostFactorAction;
        private bool        _mouseRightButtonPressed;

        private void Start()
        {
            var __map = new InputActionMap("Simple Camera Controller");

            _lookAction = __map.AddAction("look", binding: "<Mouse>/delta");
            _movementAction = __map.AddAction("move", binding: "<Gamepad>/leftStick");
            _verticalMovementAction = __map.AddAction("Vertical Movement");
            _boostFactorAction = __map.AddAction("Boost Factor", binding: "<Mouse>/scroll");

            _lookAction.AddBinding("<Gamepad>/rightStick").WithProcessor("scaleVector2(x=15, y=15)");
            _movementAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");
            _verticalMovementAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/pageUp")
                .With("Down", "<Keyboard>/pageDown")
                .With("Up", "<Keyboard>/e")
                .With("Down", "<Keyboard>/q")
                .With("Up", "<Gamepad>/rightshoulder")
                .With("Down", "<Gamepad>/leftshoulder");
            _boostFactorAction.AddBinding("<Gamepad>/Dpad").WithProcessor("scaleVector2(x=1, y=4)");

            _movementAction.Enable();
            _lookAction.Enable();
            _verticalMovementAction.Enable();
            _boostFactorAction.Enable();
        }
#endif

        private void OnEnable()
        {
            _mTargetCameraState.SetFromTransform(transform);
            _mInterpolatingCameraState.SetFromTransform(transform);
        }

        private Vector3 GetInputTranslationDirection()
        {
            Vector3 __direction = Vector3.zero;
#if ENABLE_INPUT_SYSTEM
            var __moveDelta = _movementAction.ReadValue<Vector2>();
            __direction.x = __moveDelta.x;
            __direction.z = __moveDelta.y;
            __direction.y = _verticalMovementAction.ReadValue<Vector2>().y;
#else
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                direction += Vector3.down;
            }
            if (Input.GetKey(KeyCode.E))
            {
                direction += Vector3.up;
            }
#endif
            return __direction;
        }

        private void Update()
        {
            if (IsEscapePressed())
            {
                Application.Quit();
				#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false; 
				#endif
            }

            // Hide and lock cursor when right mouse button pressed
            if (IsRightMouseButtonDown())
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            // Unlock and show cursor when right mouse button released
            if (IsRightMouseButtonUp())
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            // Rotation
            if (IsCameraRotationAllowed())
            {
                var __mouseMovement = GetInputLookRotation() * _K_MOUSE_SENSITIVITY_MULTIPLIER * mouseSensitivity;
                if (invertY)
                    __mouseMovement.y = -__mouseMovement.y;
                
                var __mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(__mouseMovement.magnitude);

                _mTargetCameraState.yaw += __mouseMovement.x * __mouseSensitivityFactor;
                _mTargetCameraState.pitch += __mouseMovement.y * __mouseSensitivityFactor;
            }
            
            // Translation
            var __translation = GetInputTranslationDirection() * Time.deltaTime;

            // Speed up movement when shift key held
            if (IsBoostPressed())
            {
                __translation *= 10.0f;
            }
            
            // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
            boost += GetBoostFactor();
            __translation *= Mathf.Pow(2.0f, boost);

            _mTargetCameraState.Translate(__translation);

            // Framerate-independent interpolation
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            var __positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
            var __rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
            _mInterpolatingCameraState.LerpTowards(_mTargetCameraState, __positionLerpPct, __rotationLerpPct);

            _mInterpolatingCameraState.UpdateTransform(transform);
        }

        private float GetBoostFactor()
        {
#if ENABLE_INPUT_SYSTEM
            return _boostFactorAction.ReadValue<Vector2>().y * 0.01f;
#else
            return Input.mouseScrollDelta.y * 0.01f;
#endif
        }

        private Vector2 GetInputLookRotation()
        {
            // try to compensate the diff between the two input systems by multiplying with empirical values
#if ENABLE_INPUT_SYSTEM
            var __delta = _lookAction.ReadValue<Vector2>();
            __delta *= 0.5f; // Account for scaling applied directly in Windows code by old input system.
            __delta *= 0.1f; // Account for sensitivity setting on old Mouse X and Y axes.
            return __delta;
#else
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#endif
        }

        private bool IsBoostPressed()
        {
#if ENABLE_INPUT_SYSTEM
            bool __boost = Keyboard.current != null ? Keyboard.current.leftShiftKey.isPressed : false; 
            __boost |= Gamepad.current != null ? Gamepad.current.xButton.isPressed : false;
            return __boost;
#else
            return Input.GetKey(KeyCode.LeftShift);
#endif

        }

        private bool IsEscapePressed()
        {
#if ENABLE_INPUT_SYSTEM
            return Keyboard.current != null ? Keyboard.current.escapeKey.isPressed : false; 
#else
            return Input.GetKey(KeyCode.Escape);
#endif
        }

        private bool IsCameraRotationAllowed()
        {
#if ENABLE_INPUT_SYSTEM
            bool __canRotate = Mouse.current != null ? Mouse.current.rightButton.isPressed : false;
            __canRotate |= Gamepad.current != null ? Gamepad.current.rightStick.ReadValue().magnitude > 0 : false;
            return __canRotate;
#else
            return Input.GetMouseButton(1);
#endif
        }

        private bool IsRightMouseButtonDown()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current != null ? Mouse.current.rightButton.isPressed : false;
#else
            return Input.GetMouseButtonDown(1);
#endif
        }

        private bool IsRightMouseButtonUp()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current != null ? !Mouse.current.rightButton.isPressed : false;
#else
            return Input.GetMouseButtonUp(1);
#endif
        }

    }

}
