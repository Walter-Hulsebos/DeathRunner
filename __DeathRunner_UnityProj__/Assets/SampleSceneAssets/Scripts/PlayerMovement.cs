#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference shootInputActionReference;
    
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -10f;
    public float jumpHeight = 2f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    

    Vector3 _velocity;
    bool _isGrounded;
    [SerializeField] public bool isShooting;

#if ENABLE_INPUT_SYSTEM
    InputAction _movement;
    InputAction _jump;
    InputAction _shooting;

    private void OnEnable()
    {
        shootInputActionReference.action.Enable();
    }
    private void OnDisable()
    {
        shootInputActionReference.action.Disable();
    }

    void Start()
    {
        _movement = new InputAction("PlayerMovement", binding: "<Gamepad>/leftStick");
        _movement.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/s")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/a")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/d")
            .With("Right", "<Keyboard>/rightArrow");
        
        _jump = new InputAction("PlayerJump", binding: "<Gamepad>/a");
        _jump.AddBinding("<Keyboard>/space");

        _shooting = new InputAction("PlayerShooting", binding: "Mouse.leftButton");
        _shooting.AddBinding("Mouse.leftButton");

        _movement.Enable();
        _jump.Enable();
        _shooting.Enable();
    }
#endif

    // Update is called once per frame
    void Update()
    {
        float __x;
        float __z;
        bool __jumpPressed = false;
        //bool shootingPressed = false;

#if ENABLE_INPUT_SYSTEM
        var __delta = _movement.ReadValue<Vector2>();
        __x = __delta.x;
        __z = __delta.y;
        __jumpPressed = Mathf.Approximately(_jump.ReadValue<float>(), 1);
        /*
        if (Shooting.enabled)
        {
            isShooting = true; 
        }
        else
        {
            isShooting = false;
        }
        */
        //isShooting = Mathf.Approximately(Shooting.ReadValue<float>(), 1);

        isShooting = shootInputActionReference.action.IsPressed();
#else
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        jumpPressed = Input.GetButtonDown("Jump");
#endif

        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        Vector3 __move = transform.right * __x + transform.forward * __z;

        controller.Move(__move * speed * Time.deltaTime);

        if(__jumpPressed && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        _velocity.y += gravity * Time.deltaTime;

        controller.Move(_velocity * Time.deltaTime);
    }
}
