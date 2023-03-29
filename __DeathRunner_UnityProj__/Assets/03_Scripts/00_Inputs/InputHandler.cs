using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UltEvents;
using UnityEngine;
using UnityEngine.InputSystem;

using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

using Bool  = System.Boolean; 

namespace DeathRunner.Inputs
{
    [PublicAPI]
    public sealed class InputHandler : MonoBehaviour
    {
        #region Variables
        
        //NOTE: [Walter] If we later want to have multiple cars you'll likely want to use the `PlayerInput` component.
        //For this reason I used methods such as OnThrottleInputStarted, OnThrottleInputPerformed, and OnThrottleInputCanceled to handle the input.
        //This leaves open the possibility of having multiple cars with very few changes, just comment out the Input Actions, add the `PlayerInput` component, and link up the events.
        
        [FoldoutGroup(groupName: "Move")]
        [SerializeField] private InputActionReference moveInputActionReference;
        [field:FoldoutGroup(groupName: "Move")]
        [field:SerializeField] public F32x2           MoveInput          { get; private set; }
        [field:FoldoutGroup(groupName: "Move")]
        [field:SerializeField] public F32x3           MoveInputFlat      { get; private set; }
        
        [FoldoutGroup(groupName: "Dash")]
        [SerializeField] private InputActionReference dashInputActionReference;
        [field:FoldoutGroup(groupName: "Dash")]
        [field:SerializeField] public Boolean         DashInput          { get; private set; }
        
        [FoldoutGroup(groupName: "Primary Fire")]
        [SerializeField] private InputActionReference primaryFireInputActionReference;
        [field:FoldoutGroup(groupName: "Primary Fire")]
        [field:SerializeField] public Boolean         PrimaryFireInput   { get; private set; }
        
        [FoldoutGroup(groupName: "Secondary Fire")]
        [SerializeField] private InputActionReference secondaryFireInputActionReference;
        [field:FoldoutGroup(groupName: "Secondary Fire")]
        [field:SerializeField] public Boolean         SecondaryFireInput { get; private set; }
        
        //TODO: [Walter] TEMPORARY, REMOVE THIS
        [FoldoutGroup(groupName: "SlowMo")]
        [SerializeField] private InputActionReference slowMoToggleInputActionReference;
        [field:FoldoutGroup(groupName: "SlowMo")]
        [field:SerializeField] public Bool            IsSlowMoToggled  { get; private set; }
        [field:FoldoutGroup(groupName: "SlowMo")]
        [field:SerializeField] public UltEvent<Bool>  OnSlowMoToggleChange  { get; private set; }
        [field:FoldoutGroup(groupName: "SlowMo")]
        [field:SerializeField] public UltEvent        OnSlowMoEnabled  { get; private set; }
        [field:FoldoutGroup(groupName: "SlowMo")]
        [field:SerializeField] public UltEvent        OnSlowMoDisabled { get; private set; }
        
        public Vector2                                MouseScreenPosition => Mouse.current.position.ReadValue();
        
        #endregion

        #region Methods

        private void OnEnable()
        {
            moveInputActionReference.action.Enable();
            dashInputActionReference.action.Enable();
            primaryFireInputActionReference.action.Enable();
            secondaryFireInputActionReference.action.Enable();
            slowMoToggleInputActionReference.action.Enable();
        }
        
        private void OnDisable()
        {
            moveInputActionReference.action.Disable();
            dashInputActionReference.action.Disable();
            primaryFireInputActionReference.action.Disable();
            secondaryFireInputActionReference.action.Disable();
            slowMoToggleInputActionReference.action.Disable();
        }
        
        private void Awake()
        {
            moveInputActionReference.action.started            += OnMoveInputStarted;
            dashInputActionReference.action.started            += OnDashInputStarted;
            primaryFireInputActionReference.action.started     += OnPrimaryFireInputStarted;
            secondaryFireInputActionReference.action.started   += OnSecondaryFireInputStarted;
            slowMoToggleInputActionReference.action.started    += OnSlowMoInputStarted;

            moveInputActionReference.action.performed          += OnMoveInputPerformed;
            dashInputActionReference.action.performed          += OnDashInputPerformed;
            primaryFireInputActionReference.action.performed   += OnPrimaryFireInputPerformed;
            secondaryFireInputActionReference.action.performed += OnSecondaryFireInputPerformed;
            slowMoToggleInputActionReference.action.performed  += OnSlowMoInputPerformed;
            
            moveInputActionReference.action.canceled           += OnMoveInputCanceled;
            dashInputActionReference.action.canceled           += OnDashInputCanceled;
            primaryFireInputActionReference.action.canceled    += OnPrimaryFireInputCanceled;
            secondaryFireInputActionReference.action.canceled  += OnSecondaryFireInputCanceled;
            slowMoToggleInputActionReference.action.canceled   += OnSlowMoInputCanceled;
        }

        private void OnDestroy()
        {
            moveInputActionReference.action.started            -= OnMoveInputStarted;
            dashInputActionReference.action.started            -= OnDashInputStarted;
            primaryFireInputActionReference.action.started     -= OnPrimaryFireInputStarted;
            secondaryFireInputActionReference.action.started   -= OnSecondaryFireInputStarted;
            slowMoToggleInputActionReference.action.started    -= OnSlowMoInputStarted;

            moveInputActionReference.action.performed          -= OnMoveInputPerformed;
            dashInputActionReference.action.performed          -= OnDashInputPerformed;
            primaryFireInputActionReference.action.performed   -= OnPrimaryFireInputPerformed;
            secondaryFireInputActionReference.action.performed -= OnSecondaryFireInputPerformed;
            slowMoToggleInputActionReference.action.performed  -= OnSlowMoInputPerformed;
                                                    
            moveInputActionReference.action.canceled           -= OnMoveInputCanceled;
            dashInputActionReference.action.canceled           -= OnDashInputCanceled;
            primaryFireInputActionReference.action.canceled    -= OnPrimaryFireInputCanceled;
            secondaryFireInputActionReference.action.canceled  -= OnSecondaryFireInputCanceled;
            slowMoToggleInputActionReference.action.canceled   -= OnSlowMoInputCanceled;
        }

        #region Move Input Callbacks

        //NOTE: [Walter] If you want to use `PlayerInputs` components (using Unity Events), you'll want to make these public. With the current setup that isn't required.
        private void OnMoveInputStarted(InputAction.CallbackContext ctx)
        {
            MoveInput     = ctx.ReadValue<Vector2>();
            MoveInputFlat = new F32x3(x: MoveInput.x, y: 0, z: MoveInput.y); 
        }
        private void OnMoveInputPerformed(InputAction.CallbackContext ctx)
        {
            MoveInput     = ctx.ReadValue<Vector2>();
            MoveInputFlat = new F32x3(x: MoveInput.x, y: 0, z: MoveInput.y); 
        }
        private void OnMoveInputCanceled(InputAction.CallbackContext ctx)
        {
            MoveInput     = F32x2.zero;
            MoveInputFlat = F32x3.zero;
        }
        #endregion

        #region Dash Input Callbacks

        private void OnDashInputStarted(InputAction.CallbackContext ctx)
        {
            DashInput = ctx.ReadValueAsButton();
        }
        private void OnDashInputPerformed(InputAction.CallbackContext ctx)
        {
            DashInput = ctx.ReadValueAsButton();
        }
        private void OnDashInputCanceled(InputAction.CallbackContext ctx)
        {
            DashInput = false;
        }
        
        #endregion

        #region Primary Fire Input Callbacks

        private void OnPrimaryFireInputStarted(InputAction.CallbackContext ctx)
        {
            PrimaryFireInput = ctx.ReadValueAsButton();
        }

        private void OnPrimaryFireInputPerformed(InputAction.CallbackContext ctx)
        {
            PrimaryFireInput = ctx.ReadValueAsButton();
        }

        private void OnPrimaryFireInputCanceled(InputAction.CallbackContext ctx)
        {
            PrimaryFireInput = false;
        }
        
        #endregion
        
        #region Secondary Fire Input Callbacks

        private void OnSecondaryFireInputStarted(InputAction.CallbackContext ctx)
        {
            SecondaryFireInput = ctx.ReadValueAsButton();
        }

        private void OnSecondaryFireInputPerformed(InputAction.CallbackContext ctx)
        {
            SecondaryFireInput = ctx.ReadValueAsButton();
        }

        private void OnSecondaryFireInputCanceled(InputAction.CallbackContext ctx)
        {
            SecondaryFireInput = false;
        }

        #endregion

        #region SlowMo Toggle Input Callbacks

        private void OnSlowMoInputStarted(InputAction.CallbackContext ctx)
        {
            //Debug.Log(message: "SlowMo Input Started");
        }

        private void OnSlowMoInputPerformed(InputAction.CallbackContext ctx)
        {
            if (!ctx.ReadValueAsButton()) return;
            
            IsSlowMoToggled = !IsSlowMoToggled;
                
            if (IsSlowMoToggled)
            {
                OnSlowMoEnabled.Invoke();
            }
            else
            {
                OnSlowMoDisabled.Invoke();
            }
                
            OnSlowMoToggleChange.Invoke(IsSlowMoToggled);
        }

        private void OnSlowMoInputCanceled(InputAction.CallbackContext ctx)
        {
            //Debug.Log(message: "SlowMo Input Canceled");
        }

        #endregion
        
        #endregion
    }
}