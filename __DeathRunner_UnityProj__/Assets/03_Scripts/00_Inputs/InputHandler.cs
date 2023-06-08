using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using ExtEvents;
using GenericScriptableArchitecture;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
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
        
        //[SerializeField] private F32                  inputBufferTime = 1.0f;

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
        [FoldoutGroup(groupName: "Dash")]
        private Bool                                  dashInputIsHeldBackingField;
        public Bool                                   DashInputIsHeld
        {
            get => dashInputIsHeldBackingField;
            set
            {
                Bool __inputHasNotChanged = (value == dashInputIsHeldBackingField);
                
                if (__inputHasNotChanged) return;
                
                dashInputIsHeldBackingField = value;
                
                // Dash input has changed
                OnDashInputChanged?.Invoke(dashInputIsHeldBackingField);
                
                // Changed from "not held" to "held"
                if (dashInputIsHeldBackingField)
                {
                    DashInputStartedHandler().Forget();
                }
                // Changed from "held" to "not held"
                else
                {
                    DashInputStoppedHandler().Forget();
                }
            }
        }

        private async UniTask DashInputStartedHandler()
        {
            _timeOfLastDashInputStart = Time.time;
            
            OnDashStarted?.Invoke();
            
            DashInputStartedThisFrame = true;
            //Debug.Log($"[{Time.time}] DashInputStartedThisFrame = true");
            //await UniTask.Yield(); //Wait one frame.
            await UniTask.DelayFrame(delayFrameCount: 1, delayTiming: PlayerLoopTiming.Update, cancellationToken: _dashInputCancellationToken);
            //Debug.Log($"[{Time.time}] DashInputStartedThisFrame = false");
            DashInputStartedThisFrame = false;
        }
        
        private async UniTask DashInputStoppedHandler()
        {
            _timeOfLastDashInputStop = Time.time;
            
            OnDashStopped?.Invoke();
            
            DashInputStoppedThisFrame = true;
            //Debug.Log($"[{Time.time}] DashInputStoppedThisFrame = true");
            //await UniTask.Yield(); //Wait one frame.
            await UniTask.DelayFrame(delayFrameCount: 1, delayTiming: PlayerLoopTiming.Update, cancellationToken: _dashInputCancellationToken);
            //Debug.Log($"[{Time.time}] DashInputStoppedThisFrame = false");
            DashInputStoppedThisFrame = false;
        }

        [field:FoldoutGroup(groupName: "Dash")]
        [field:SerializeField] public Bool            DashInputStartedThisFrame   { get; private set; }
        [field:FoldoutGroup(groupName: "Dash")]
        [field:SerializeField] public Bool            DashInputStoppedThisFrame   { get; private set; }
        
        private F32 _timeOfLastDashInputStart;
        private F32 _timeOfLastDashInputStop;

        public F32 DashHoldTime
        {
            get
            {
                Bool __stopIsAfterStart = _timeOfLastDashInputStop > _timeOfLastDashInputStart;

                if (__stopIsAfterStart) //If the stop is after the start, return the difference.
                {
                    return (_timeOfLastDashInputStop - _timeOfLastDashInputStart);
                }

                if (DashInputIsHeld) //If the stop is before the start, compare with current time.
                {
                    return (Time.time - _timeOfLastDashInputStart);   
                }

                //If the stop is before the start, and the input is not held, return 0.
                return 0f;
            }
        }
        //public F32                                    DashHoldTime       { get; private set; }
        public event Action<Bool>                     OnDashInputChanged;
        public event Action                           OnDashStarted;
        public event Action                           OnDashStopped;

        [FoldoutGroup(groupName: "Primary Fire")]
        [SerializeField] private InputActionReference primaryFireInputActionReference;
        
        [FoldoutGroup(groupName: "Primary Fire")]
        [SerializeField] private Constant<F32>       primaryFireInputBufferTimeSeconds;// = new(value: 1.0f);
        //[field:FoldoutGroup(groupName: "Primary Fire")]
        //[field:SerializeField] public Bool            PrimaryFireInput   { get; private set; }
        //public event Action<Bool>                     OnPrimaryFireInputChanged;
        //public event Action                           OnPrimaryFire;
        //public event Action                           OnPrimaryFireStopped;

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
        //[field:FoldoutGroup(groupName: "SlowMo")]
        //[field:SerializeField] public UltEvent<Bool>  OnSlowMoToggleChanged  { get; private set; }
        
        //[field:FoldoutGroup(groupName: "SlowMo")]
        //[field:SerializeField] public UltEvent        OnSlowMoEnabled  { get; private set; }
        //[field:FoldoutGroup(groupName: "SlowMo")]
        //[field:SerializeField] public UltEvent        OnSlowMoDisabled { get; private set; }
        
        public F32x2                                  MouseScreenPosition => (F32x2)Mouse.current.position.ReadValue();
        
        private CancellationTokenSource _dashInputCancellationTokenSource;
        private CancellationToken       _dashInputCancellationToken;

        #endregion

        #region Methods

        private void OnEnable()
        {
            PrimaryFireInputQueue = new InputQueue<Bool>(bufferTimeInSeconds: primaryFireInputBufferTimeSeconds);
            
            moveInputActionReference.action.Enable();
            aimInputActionReference.action.Enable();
            dashInputActionReference.action.Enable();
            primaryFireInputActionReference.action.Enable();
            secondaryFireInputActionReference.action.Enable();
            slowMoToggleInputActionReference.action.Enable();
            
            _dashInputCancellationTokenSource = new CancellationTokenSource();
            _dashInputCancellationToken       = _dashInputCancellationTokenSource.Token;
        }
        
        private void OnDisable()
        {
            moveInputActionReference.action.Disable();
            aimInputActionReference.action.Disable();
            dashInputActionReference.action.Disable();
            primaryFireInputActionReference.action.Disable();
            secondaryFireInputActionReference.action.Disable();
            slowMoToggleInputActionReference.action.Disable();
            
            _dashInputCancellationTokenSource.Cancel();
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
            //primaryFireInputActionReference.action.performed   += OnPrimaryFireInputPerformed;
            secondaryFireInputActionReference.action.performed += OnSecondaryFireInputPerformed;
            slowMoToggleInputActionReference.action.performed  += OnSlowMoInputPerformed;
            
            moveInputActionReference.action.canceled           += OnMoveInputCanceled;
            aimInputActionReference.action.canceled            += OnAimInputCanceled;
            dashInputActionReference.action.canceled           += OnDashInputCanceled;
            //primaryFireInputActionReference.action.canceled    += OnPrimaryFireInputCanceled;
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
            //primaryFireInputActionReference.action.performed   -= OnPrimaryFireInputPerformed;
            secondaryFireInputActionReference.action.performed -= OnSecondaryFireInputPerformed;
            slowMoToggleInputActionReference.action.performed  -= OnSlowMoInputPerformed;
                                                    
            moveInputActionReference.action.canceled           -= OnMoveInputCanceled;
            aimInputActionReference.action.canceled            -= OnAimInputCanceled;
            dashInputActionReference.action.canceled           -= OnDashInputCanceled;
            //primaryFireInputActionReference.action.canceled    -= OnPrimaryFireInputCanceled;
            secondaryFireInputActionReference.action.canceled  -= OnSecondaryFireInputCanceled;
            slowMoToggleInputActionReference.action.canceled   -= OnSlowMoInputCanceled;
        }

        private void Update()
        {
            InputSystem.Update();
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

        private void OnDashInputStarted(InputAction.CallbackContext ctx)   => DashInputIsHeld = ctx.ReadValueAsButton();
        private void OnDashInputPerformed(InputAction.CallbackContext ctx) => DashInputIsHeld = ctx.ReadValueAsButton();
        private void OnDashInputCanceled(InputAction.CallbackContext ctx)  => DashInputIsHeld = false;
        
        // private void HandleDashInput(Bool newDashInputIsHeld)
        // {
        //     Bool __inputHasNotChanged = (newDashInputIsHeld == DashInputIsHeld);
        //     
        //     if (__inputHasNotChanged) return;
        //     
        //     DashInputIsHeld = newDashInputIsHeld;
        //
        //     OnDashInputChanged?.Invoke(DashInputIsHeld);
        //     
        //     // Just started holding the dash input
        //     if (DashInputIsHeld)
        //     {
        //         OnDashStarted?.Invoke();
        //         DashInputStarted = true;
        //         
        //         _timeOfLastStartDashInput = Time.time;
        //         
        //         //HoldTimeCounter().Forget();
        //     }
        //     // Just released the dash input
        //     else
        //     {
        //         OnDashStopped?.Invoke();
        //         DashInputStopped = true;
        //         
        //         _timeOfLastStopDashInput = Time.time;
        //     }
        // }
        
        //TODO: [Walter] Instead just save the times when the input started and stopped, and calculate the hold time when needed. (on poll or on stop)

        // private async UniTask HoldTimeCounter()
        // {
        //     while(DashInputIsHeld)
        //     {
        //         DashInputStarted = false;
        //         
        //         await UniTask.Yield();
        //         
        //         if (_holdTimeCounterCancellationToken.IsCancellationRequested)
        //         {
        //             DashHoldTime = 0;
        //             return;
        //         }
        //         
        //         DashHoldTime += Time.deltaTime;
        //     }
        //     
        //     DashInputStopped = true;
        //     
        //     await UniTask.Yield();
        //     
        //     DashInputStopped = false;
        //     
        //     //Clear the hold time counter a frame after the dash input is released, so that things that rely on the dash hold time can still get the value.
        //     DashHoldTime = 0;
        // } 
        
        #endregion

        #region Primary Fire Input Callbacks
        
        public InputQueue<Bool>      PrimaryFireInputQueue;

        private void OnPrimaryFireInputStarted(InputAction.CallbackContext ctx) => PrimaryFireInputQueue.Enqueue(input: true).Forget();
        private void OnPrimaryFireInputPerformed(InputAction.CallbackContext ctx) {} //=> HandlePrimaryFireInput(newPrimaryFireInput: false);
        private void OnPrimaryFireInputCanceled(InputAction.CallbackContext ctx)  {} //=> HandlePrimaryFireInput(newPrimaryFireInput: false);

        // private void HandlePrimaryFireInput(Bool newPrimaryFireInput)
        // {
        //     Bool __inputHasChanged = (newPrimaryFireInput != PrimaryFireInput);
        //
        //     if (!__inputHasChanged) return;
        //
        //     PrimaryFireInput = newPrimaryFireInput;
        //     OnPrimaryFireInputChanged?.Invoke(PrimaryFireInput);
        //
        //     if (PrimaryFireInput)
        //     {
        //         Debug.Log("Primary Fire Started");
        //         OnPrimaryFire?.Invoke();
        //     }
        // }

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
            //     
            // if (IsSlowMoToggled)
            // {
            //     OnSlowMoEnabled.Invoke();
            // }
            // else
            // {
            //     OnSlowMoDisabled.Invoke();
            // }
            //     
            // OnSlowMoToggleChanged.Invoke(IsSlowMoToggled);
        }

        private void OnSlowMoInputCanceled(InputAction.CallbackContext ctx)
        {
            //Debug.Log(message: "SlowMo Input Canceled");
        }

        #endregion
        
        #endregion
    }
}