#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

using UnityEngine;

public class LookWithMouse : MonoBehaviour
{
    private const float _K_MOUSE_SENSITIVITY_MULTIPLIER = 0.01f;

    public float mouseSensitivity = 100f;

    public Transform playerBody;

    private float _xRotation = 0f;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        float __mouseX = 0, __mouseY = 0;

        if (Mouse.current != null)
        {
            var __delta = Mouse.current.delta.ReadValue() / 15.0f;
            __mouseX += __delta.x;
            __mouseY += __delta.y;
        }
        if (Gamepad.current != null)
        {
            var __value = Gamepad.current.rightStick.ReadValue() * 2;
            __mouseX += __value.x;
            __mouseY += __value.y;
        }

        __mouseX *= mouseSensitivity * _K_MOUSE_SENSITIVITY_MULTIPLIER;
        __mouseY *= mouseSensitivity * _K_MOUSE_SENSITIVITY_MULTIPLIER;
#else
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * k_MouseSensitivityMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * k_MouseSensitivityMultiplier;
#endif

        _xRotation -= __mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * __mouseX);
    }
}
