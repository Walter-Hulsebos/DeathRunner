using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UltEvents;

using UnityEngine;
using UnityEngine.InputSystem;

using static Unity.Mathematics.math;

using F32   = System.Single;
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
        public event Action<F32x2>                    OnMoveInputUpdated; 
        public event Action<F32x2>                    OnMoveInputChanged; 
        public event Action<F32x3>                    OnMoveInputFlatUpdated;
        public event Action<F32x3>                    OnMoveInputFlatChanged;
        public event Action<F32x3>                    OnMoveInputFlatChangedAndNotZero;
        public event Action<F32x3>                    OnMoveStarted;
        public event Action<F32x3>                    OnMoveStopped;
        private Bool                                  _hasMoveInput;
        
        [FoldoutGroup(groupName: "Aim")]
        [SerializeField] private InputActionReference aimInputActionReference;
        [field:FoldoutGroup(groupName: "Aim")]
        [field:SerializeField] public F32x2           AimInput          { get; private set; }
        [field:FoldoutGroup(groupName: "Aim")]
        public event Action<F32x2>                    OnAimInputUpdated; 
        public event Action<F32x2>                    OnAimInputChanged;
        public event Action<F32x2>                    OnAimStarted;
        public event Action<F32x2>                    OnAimStopped;
        private Bool                                  _hasAimInput;

        [FoldoutGroup(groupName: "Dash")]
        [SerializeField] private InputActionReference dashInputActionReference;
        [field:FoldoutGroup(groupName: "Dash")]
        [field:SerializeField] public Bool            DashInput          { get; private set; }
        public event Action<Bool>                     OnDashInputChanged;
        public event Action                           OnDashTriggered;

        [FoldoutGroup(groupName: "Primary Fire")]
        [SerializeField] private InputActionReference primaryFireInputActionReference;
        [field:FoldoutGroup(groupName: "Primary Fire")]
        [field:SerializeField] public Bool            PrimaryFireInput   { get; private set; }
        public event Action<Bool>                     OnPrimaryFireInputChanged;
        public event Action                           OnPrimaryFireStarted;
        public event Action                           OnPrimaryFireStopped;

        [FoldoutGroup(groupName: "Secondary Fire")]
        [SerializeField] private InputActionReference secondaryFireInputActionReference;
        [field:FoldoutGroup(groupName: "Secondary Fire")]
        [field:SerializeField] public Bool            SecondaryFireInput { get; private set; }
        public event Action<Bool>                     OnSecondaryFireInputChanged;
        public event Action                           OnSecondaryFireStarted;
        public event Action                           OnSecondaryFireStopped;
        
        //TODO: [Walter] TEMPORARY, REMOVE THIS
        [FoldoutGroup(groupName: "SlowMo")]
        [SerializeField] private InputActionReference slowMoToggleInputActionReference;
        [field:FoldoutGroup(groupName: "SlowMo")]
        [field:SerializeField] public Bool            IsSlowMoToggled  { get; private set; }
        [field:FoldoutGroup(groupName: "SlowMo")]
        [field:SerializeField] public UltEvent<Bool>  OnSlowMoToggleChanged  { get; private set; }
        
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
            aimInputActionReference.action.Enable();
            dashInputActionReference.action.Enable();
            primaryFireInputActionReference.action.Enable();
            secondaryFireInputActionReference.action.Enable();
            slowMoToggleInputActionReference.action.Enable();
        }
        
        private void OnDisable()
        {
            moveInputActionReference.action.Disable();
            aimInputActionReference.action.Disable();
            dashInputActionReference.action.Disable();
            primaryFireInputActionReference.action.Disable();
            secondaryFireInputActionReference.action.Disable();
            slowMoToggleInputActionReference.action.Disable();
        }
        
        private void Awake()
        {
            moveInputActionReference.action.started            += OnMoveInputStarted;
            aimInputActionReference.action.started             += OnAimInputStarted;
            dashInputActionReference.action.started            += OnDashInputStarted;
            primaryFireInputActionReference.action.started     += OnPrimaryFireInputStarted;
            secondaryFireInputActionReference.action.started   += OnSecondaryFireInputStarted;
            slowMoToggleInputActionReference.action.started    += OnSlowMoInputStarted;

            moveInputActionReference.action.performed          += OnMoveInputPerformed;
            aimInputActionReference.action.performed           += OnAimInputPerformed;
            dashInputActionReference.action.performed          += OnDashInputPerformed;
            primaryFireInputActionReference.action.performed   += OnPrimaryFireInputPerformed;
            secondaryFireInputActionReference.action.performed += OnSecondaryFireInputPerformed;
            slowMoToggleInputActionReference.action.performed  += OnSlowMoInputPerformed;
            
            moveInputActionReference.action.canceled           += OnMoveInputCanceled;
            aimInputActionReference.action.canceled            += OnAimInputCanceled;
            dashInputActionReference.action.canceled           += OnDashInputCanceled;
            primaryFireInputActionReference.action.canceled    += OnPrimaryFireInputCanceled;
            secondaryFireInputActionReference.action.canceled  += OnSecondaryFireInputCanceled;
            slowMoToggleInputActionReference.action.canceled   += OnSlowMoInputCanceled;
        }

        private void OnDestroy()
        {
            moveInputActionReference.action.started            -= OnMoveInputStarted;
            aimInputActionReference.action.started             -= OnAimInputStarted;
            dashInputActionReference.action.started            -= OnDashInputStarted;
            primaryFireInputActionReference.action.started     -= OnPrimaryFireInputStarted;
            secondaryFireInputActionReference.action.started   -= OnSecondaryFireInputStarted;
            slowMoToggleInputActionReference.action.started    -= OnSlowMoInputStarted;

            moveInputActionReference.action.performed          -= OnMoveInputPerformed;
            aimInputActionReference.action.performed           -= OnAimInputPerformed;
            dashInputActionReference.action.performed          -= OnDashInputPerformed;
            primaryFireInputActionReference.action.performed   -= OnPrimaryFireInputPerformed;
            secondaryFireInputActionReference.action.performed -= OnSecondaryFireInputPerformed;
            slowMoToggleInputActionReference.action.performed  -= OnSlowMoInputPerformed;
                                                    
            moveInputActionReference.action.canceled           -= OnMoveInputCanceled;
            aimInputActionReference.action.canceled            -= OnAimInputCanceled;
            dashInputActionReference.action.canceled           -= OnDashInputCanceled;
            primaryFireInputActionReference.action.canceled    -= OnPrimaryFireInputCanceled;
            secondaryFireInputActionReference.action.canceled  -= OnSecondaryFireInputCanceled;
            slowMoToggleInputActionReference.action.canceled   -= OnSlowMoInputCanceled;
        }

        #region Move Input Callbacks

        //NOTE: [Walter] If you want to use `PlayerInputs` components (using Unity Events), you'll want to make these public. With the current setup that isn't required.
        private void OnMoveInputStarted(InputAction.CallbackContext ctx)   => HandleMoveInput(newMoveInput: (F32x2)ctx.ReadValue<Vector2>());
        private void OnMoveInputPerformed(InputAction.CallbackContext ctx) => HandleMoveInput(newMoveInput: (F32x2)ctx.ReadValue<Vector2>());
        private void OnMoveInputCanceled(InputAction.CallbackContext ctx)  => HandleMoveInput(newMoveInput: F32x2.zero);

        private void HandleMoveInput(F32x2 newMoveInput)
        {
            Bool __inputHasChanged = any(MoveInput != newMoveInput);
            
            //Debug.Log($"Move Input: {MoveInput}, New Move Input: {newMoveInput}, Input Has Changed: {__inputHasChanged}");
            
            MoveInput     = newMoveInput;
            MoveInputFlat = new F32x3(x: MoveInput.x, y: 0, z: MoveInput.y);

            if (__inputHasChanged)
            {
                OnMoveInputChanged?.Invoke(MoveInput);
                OnMoveInputFlatChanged?.Invoke(MoveInputFlat);
                
                //Debug.Log("Move Input Changed");

                F32 __inputSqrMagnitude = lengthsq(MoveInput);
                if(__inputSqrMagnitude > 0)
                {
                    if (!_hasMoveInput) //This should be redundant, since we're already checking for input has changed, but just in case.
                    {
                        OnMoveStarted?.Invoke(MoveInputFlat);
                        //Debug.Log("Move Started");
                    }
                    
                    _hasMoveInput = true;
                }
                else
                {
                    if (_hasMoveInput)
                    {
                        OnMoveStopped?.Invoke(MoveInputFlat);
                        _hasMoveInput = false;
                        //Debug.Log("Move Stopped");
                    }
                }
                
                //if (all(MoveInputFlat == F32x3.zero))
                // {
                //     OnMoveStopped?.Invoke(MoveInputFlat);
                // }
                // else
                // {
                //     OnMoveStarted?.Invoke(MoveInputFlat);
                // }
            }

            OnMoveInputUpdated?.Invoke(MoveInput);
            OnMoveInputFlatUpdated?.Invoke(MoveInputFlat);
        }
        #endregion

        #region Aim Input Callbacks

        private void OnAimInputStarted(InputAction.CallbackContext ctx)   => HandleAimInput(newAimInput: (F32x2)ctx.ReadValue<Vector2>());
        private void OnAimInputPerformed(InputAction.CallbackContext ctx) => HandleAimInput(newAimInput: (F32x2)ctx.ReadValue<Vector2>());
        private void OnAimInputCanceled(InputAction.CallbackContext ctx)  => HandleAimInput(newAimInput: F32x2.zero);

        private void HandleAimInput(F32x2 newAimInput)
        {
            Bool __inputHasChanged = any(AimInput != newAimInput);
            AimInput = newAimInput;

            if (__inputHasChanged)
            {
                OnAimInputChanged?.Invoke(AimInput);

                //Debug.Log("Aim Input Changed");

                F32 __inputSqrMagnitude = lengthsq(AimInput);
                if(__inputSqrMagnitude > 0)
                {
                    if (!_hasAimInput) //This should be redundant, since we're already checking for input has changed, but just in case.
                    {
                        OnAimStarted?.Invoke(AimInput);
                    }
                    
                    _hasAimInput = true;
                }
                else
                {
                    if (_hasAimInput)
                    {
                        OnAimStopped?.Invoke(AimInput);
                        
                        _hasAimInput = false;
                    }
                }
            }

            OnAimInputUpdated?.Invoke(AimInput);
        }

        #endregion

        #region Dash Input Callbacks

        private void OnDashInputStarted(InputAction.CallbackContext ctx)   => HandleDashInput(newDashInput: ctx.ReadValueAsButton());
        private void OnDashInputPerformed(InputAction.CallbackContext ctx) => HandleDashInput(newDashInput: ctx.ReadValueAsButton());
        private void OnDashInputCanceled(InputAction.CallbackContext ctx)  => HandleDashInput(newDashInput: false);

        private void HandleDashInput(Bool newDashInput)
        {
            Bool __inputHasChanged = (DashInput != newDashInput);
            
            if (!__inputHasChanged) return;
            DashInput = newDashInput;

            OnDashInputChanged?.Invoke(DashInput);
                
            if (DashInput)
            {
                OnDashTriggered?.Invoke();
            }
        }
        
        #endregion

        #region Primary Fire Input Callbacks

        private void OnPrimaryFireInputStarted(InputAction.CallbackContext ctx)   => HandlePrimaryFireInput(newPrimaryFireInput: ctx.ReadValueAsButton());
        private void OnPrimaryFireInputPerformed(InputAction.CallbackContext ctx) => HandlePrimaryFireInput(newPrimaryFireInput: ctx.ReadValueAsButton());
        private void OnPrimaryFireInputCanceled(InputAction.CallbackContext ctx)  => HandlePrimaryFireInput(newPrimaryFireInput: false);

        private void HandlePrimaryFireInput(Bool newPrimaryFireInput)
        {
            PrimaryFireInput = newPrimaryFireInput;
            OnPrimaryFireInputChanged?.Invoke(PrimaryFireInput);
            
            if (PrimaryFireInput)
            {
                OnPrimaryFireStarted?.Invoke();
            }
            else
            {
                OnPrimaryFireStopped?.Invoke();
            }
        }
        
        #endregion
        
        #region Secondary Fire Input Callbacks
        
        private void OnSecondaryFireInputStarted(InputAction.CallbackContext ctx)   => HandleSecondaryFireInput(newSecondaryFireInput: ctx.ReadValueAsButton());
        private void OnSecondaryFireInputPerformed(InputAction.CallbackContext ctx) => HandleSecondaryFireInput(newSecondaryFireInput: ctx.ReadValueAsButton());
        private void OnSecondaryFireInputCanceled(InputAction.CallbackContext ctx)  => HandleSecondaryFireInput(newSecondaryFireInput: false);
        
        private void HandleSecondaryFireInput(Bool newSecondaryFireInput)
        {
            SecondaryFireInput = newSecondaryFireInput;
            OnSecondaryFireInputChanged?.Invoke(SecondaryFireInput);
            
            if (SecondaryFireInput)
            {
                OnSecondaryFireStarted?.Invoke();
            }
            else
            {
                OnSecondaryFireStopped?.Invoke();
            }
        }

        #endregion

        #region SlowMo Toggle Input Callbacks

        private void OnSlowMoInputStarted(InputAction.CallbackContext ctx)
        {
            //Debug.Log(message: "SlowMo Input Started");
        }

        private void OnSlowMoInputPerformed(InputAction.CallbackContext ctx)
        {
            //if (!ctx.ReadValueAsButton()) return;

            IsSlowMoToggled = !IsSlowMoToggled;
                
            if (IsSlowMoToggled)
            {
                OnSlowMoEnabled.Invoke();
            }
            else
            {
                OnSlowMoDisabled.Invoke();
            }
                
            OnSlowMoToggleChanged.Invoke(IsSlowMoToggled);
        }

        private void OnSlowMoInputCanceled(InputAction.CallbackContext ctx)
        {
            //Debug.Log(message: "SlowMo Input Canceled");
        }

        #endregion
        
        #endregion
    }
}