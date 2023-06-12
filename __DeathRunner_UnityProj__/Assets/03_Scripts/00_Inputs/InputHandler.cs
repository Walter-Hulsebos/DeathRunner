using System;
using System.Threading;

using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Mathematics.math;

using GenericScriptableArchitecture;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;

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

        [FoldoutGroup(groupName: "Secondary Fire")]
        [SerializeField] private InputActionReference secondaryFireInputActionReference;
        [field:FoldoutGroup(groupName: "Secondary Fire")]
        [field:SerializeField] public Bool            SecondaryFireInput { get; private set; }


        public F32x2                                  MouseScreenPosition => (F32x2)Mouse.current.position.ReadValue();
        
        //private CancellationTokenSource _dashInputCancellationTokenSource;
        //private CancellationToken       _dashInputCancellationToken;

        #endregion

        #region Methods

        private void OnEnable()
        {
            primaryFireInputQueue = new InputQueue<Bool>(bufferTimeInSeconds: primaryFireInputBufferTimeSeconds.Value);
            //shortDashInputQueue   = new InputQueue<Bool>(bufferTimeInSeconds: shortDashInputBufferTimeSeconds.Value);
            
            moveInputActionReference.action.Enable();
            aimInputActionReference.action.Enable();
            dashInputActionReference.action.Enable();
            primaryFireInputActionReference.action.Enable();
            secondaryFireInputActionReference.action.Enable();

            //_dashInputCancellationTokenSource = new CancellationTokenSource();
            //_dashInputCancellationToken       = _dashInputCancellationTokenSource.Token;
        }
        
        private void OnDisable()
        {
            moveInputActionReference.action.Disable();
            aimInputActionReference.action.Disable();
            dashInputActionReference.action.Disable();
            primaryFireInputActionReference.action.Disable();
            secondaryFireInputActionReference.action.Disable();

            //_dashInputCancellationTokenSource.Cancel();
        }
        
        private void Awake()
        {
            moveInputActionReference.action.started            += OnMoveInputStarted;
            aimInputActionReference.action.started             += OnAimInputStarted;
            dashInputActionReference.action.started            += OnDashInputStarted;
            primaryFireInputActionReference.action.started     += OnPrimaryFireInputStarted;
            secondaryFireInputActionReference.action.started   += OnSecondaryFireInputStarted;

            moveInputActionReference.action.performed          += OnMoveInputPerformed;
            aimInputActionReference.action.performed           += OnAimInputPerformed;
            dashInputActionReference.action.performed          += OnDashInputPerformed;
            //shortDashInputActionReference.action.performed     += OnDashInputPerformed;
            secondaryFireInputActionReference.action.performed += OnSecondaryFireInputPerformed;

            moveInputActionReference.action.canceled           += OnMoveInputCanceled;
            aimInputActionReference.action.canceled            += OnAimInputCanceled;
            dashInputActionReference.action.canceled           += OnDashInputCanceled;
            //shortDashInputActionReference.action.canceled      += OnDashInputCanceled;
            secondaryFireInputActionReference.action.canceled  += OnSecondaryFireInputCanceled;
        }

        private void OnDestroy()
        {
            moveInputActionReference.action.started            -= OnMoveInputStarted;
            aimInputActionReference.action.started             -= OnAimInputStarted;
            dashInputActionReference.action.started            -= OnDashInputStarted;
            primaryFireInputActionReference.action.started     -= OnPrimaryFireInputStarted;
            secondaryFireInputActionReference.action.started   -= OnSecondaryFireInputStarted;

            moveInputActionReference.action.performed          -= OnMoveInputPerformed;
            aimInputActionReference.action.performed           -= OnAimInputPerformed;
            dashInputActionReference.action.performed          -= OnDashInputPerformed;
            //shortDashInputActionReference.action.performed     -= OnDashInputPerformed;
            secondaryFireInputActionReference.action.performed -= OnSecondaryFireInputPerformed;

            moveInputActionReference.action.canceled           -= OnMoveInputCanceled;
            aimInputActionReference.action.canceled            -= OnAimInputCanceled;
            dashInputActionReference.action.canceled           -= OnDashInputCanceled;
            //shortDashInputActionReference.action.canceled      -= OnDashInputCanceled;
            secondaryFireInputActionReference.action.canceled  -= OnSecondaryFireInputCanceled;
        }

        private void Update()
        {
            InputSystem.Update();
        }

        #region Move Input Callbacks
        
        [FoldoutGroup(groupName: "Move")]
        [SerializeField] private InputActionReference moveInputActionReference;
        [field:FoldoutGroup(groupName: "Move")]
        [field:SerializeField] public F32x2           MoveInput          { get; private set; }
        [field:FoldoutGroup(groupName: "Move")]
        [field:SerializeField] public F32x3           MoveInputFlat      { get; private set; }
        private Bool                                  _hasMoveInput;

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

            if (!__inputHasChanged) return;
            
            F32 __inputSqrMagnitude = lengthsq(MoveInput);
            if(__inputSqrMagnitude > 0)
            {
                _hasMoveInput = true;
            }
            else
            {
                if (_hasMoveInput)
                {
                    _hasMoveInput = false;
                }
            }
        }
        #endregion

        #region Aim Input Callbacks
        
        [FoldoutGroup(groupName: "Aim")]
        [SerializeField] private InputActionReference aimInputActionReference;
        [field:FoldoutGroup(groupName: "Aim")]
        [field:SerializeField] public F32x2           AimInput          { get; private set; }
        private Bool                                  _hasAimInput;

        private void OnAimInputStarted(InputAction.CallbackContext ctx)   => HandleAimInput(newAimInput: (F32x2)ctx.ReadValue<Vector2>());
        private void OnAimInputPerformed(InputAction.CallbackContext ctx) => HandleAimInput(newAimInput: (F32x2)ctx.ReadValue<Vector2>());
        private void OnAimInputCanceled(InputAction.CallbackContext ctx)  => HandleAimInput(newAimInput: F32x2.zero);

        private void HandleAimInput(F32x2 newAimInput)
        {
            Bool __inputHasChanged = any(AimInput != newAimInput);
            AimInput = newAimInput;

            if (!__inputHasChanged) return;
            
            F32 __inputSqrMagnitude = lengthsq(AimInput);
            if(__inputSqrMagnitude > 0)
            {
                _hasAimInput = true;
            }
            else
            {
                if (_hasAimInput)
                {
                    _hasAimInput = false;
                }
            }
        }

        #endregion

        #region Short Dash Input Callbacks
        
        [FoldoutGroup(groupName: "Dash")]
        [SerializeField] private InputActionReference dashInputActionReference;

        //private F32 _shortDashInputStartTime = 0.0f;

        // F32 is the time since the dash input was started.
        //public InputQueue<Bool> shortDashInputQueue;
        
        public Bool DashInputIsHeld { get; private set; }

        private void OnDashInputStarted(InputAction.CallbackContext ctx)   => DashInputIsHeld = ctx.ReadValueAsButton();
        private void OnDashInputPerformed(InputAction.CallbackContext ctx) => DashInputIsHeld = ctx.ReadValueAsButton();
        private void OnDashInputCanceled(InputAction.CallbackContext ctx)  => DashInputIsHeld = false;
        
        // [FoldoutGroup(groupName: "Dash")]
        // [SerializeField] private Constant<F32> minHoldTimeForLongDashSeconds;

        //private void OnDashInputStarted(InputAction.CallbackContext ctx)   => ShortDashInputIsHeld = ctx.ReadValueAsButton();
        // private void OnDashInputPerformed(InputAction.CallbackContext ctx) => ShortDashInputIsHeld = ctx.ReadValueAsButton();
        // private void OnDashInputCanceled(InputAction.CallbackContext ctx)  => ShortDashInputIsHeld = false;

        // private Bool _shortDashInputIsHeldBackingField = false;
        // private Bool ShortDashInputIsHeld
        // {
        //     get => _shortDashInputIsHeldBackingField;
        //     set
        //     {
        //         // Bool __inputHasChanged = _shortDashInputIsHeldBackingField != value;
        //         // _shortDashInputIsHeldBackingField = value;
        //         //
        //         // if(!__inputHasChanged) return;
        //         //
        //         // if (_shortDashInputIsHeldBackingField)
        //         // {
        //         //     _shortDashInputStartTime = Time.time;
        //         // }
        //         // else
        //         // {
        //         //     F32 __dashHoldTime = Time.time - _shortDashInputStartTime;
        //         //     shortDashInputQueue.Enqueue(input: __dashHoldTime).Forget();
        //         // }
        //     }
        //}

        #endregion

        #region Long Dash Input Callbacks

        

        #endregion

        #region Primary Fire Input Callbacks
        
        [FoldoutGroup(groupName: "Primary Fire")]
        [SerializeField] private InputActionReference primaryFireInputActionReference;
        [FoldoutGroup(groupName: "Primary Fire")]
        [SerializeField] private Constant<F32>       primaryFireInputBufferTimeSeconds;// = new(value: 1.0f);
        
        public InputQueue<Bool> primaryFireInputQueue;

        private void OnPrimaryFireInputStarted(InputAction.CallbackContext ctx) => primaryFireInputQueue.Enqueue(input: true).Forget();

        #endregion
        
        #region Secondary Fire Input Callbacks
        
        private void OnSecondaryFireInputStarted(InputAction.CallbackContext ctx)   => HandleSecondaryFireInput(newSecondaryFireInput: ctx.ReadValueAsButton());
        private void OnSecondaryFireInputPerformed(InputAction.CallbackContext ctx) => HandleSecondaryFireInput(newSecondaryFireInput: ctx.ReadValueAsButton());
        private void OnSecondaryFireInputCanceled(InputAction.CallbackContext ctx)  => HandleSecondaryFireInput(newSecondaryFireInput: false);
        
        private void HandleSecondaryFireInput(Bool newSecondaryFireInput)
        {
            SecondaryFireInput = newSecondaryFireInput;
        }

        #endregion

        #endregion
    }
}