﻿using UnityEngine;

namespace EasyCharacterMovement.CharacterMovementExamples
{
    public class SimulatableCharacter : MonoBehaviour, ISimulatable
    {
        #region EDITOR EXPOSED FIELDS

        public float rotationRate = 540.0f;

        [Space(15f)]
        public float maxSpeed = 5.0f;
        public float maxAcceleration = 20.0f;

        public float jumpImpulse = 6.5f;

        public float airFriction = 0.1f;

        [Range(0.0f, 1.0f)]
        public float airControl = 0.3f;

        public Vector3 gravity = Vector3.down * 9.81f;

        #endregion

        #region FIELDS

        private Vector3 _initialFramePosition;
        private Quaternion _initialFrameRotation;

        private Vector3 _movementDirection;

        private bool _jumpButtonPressed;

        #endregion

        #region PROPERTIES

        private CharacterMotor CharacterMotor { get; set; }

        #endregion

        #region ISimulatable

        public void PrePhysicsUpdate(float deltaTime)
        {
            // EMPTY (not used here)
        }

        public void PostPhysicsUpdate(float deltaTime)
        {
            // Save pre-simulation poses,
            // and make sure the character is at its up-to-date position and rotation (NOT INTERPOLATED ONES) before simulate it

            _initialFramePosition = CharacterMotor.updatedPosition;
            _initialFrameRotation = CharacterMotor.updatedRotation;

            CharacterMotor.SetPositionAndRotation(CharacterMotor.updatedPosition, CharacterMotor.updatedRotation);

            // Simulate this character (e.g. update its position and rotation)

            Simulate(deltaTime);
        }

        /// <summary>
        /// Interpolate character pose, this is only needed when using custom simulation with FIXED timestep.
        /// </summary>

        public void Interpolate(float interpolationFactor)
        {
            // Set transform's interpolated pose

            Vector3 p = Vector3.Lerp(_initialFramePosition, CharacterMotor.updatedPosition, interpolationFactor);
            Quaternion q = Quaternion.Slerp(_initialFrameRotation, CharacterMotor.updatedRotation, interpolationFactor);

            transform.SetPositionAndRotation(p, q);
        }

        #endregion

        #region METHODS

        public void Simulate(float deltaTime)
        {
            // Rotate character towards movement direction

            CharacterMotor.RotateTowards(_movementDirection, rotationRate * deltaTime);

            // Move character (e.g. update its velocity)
            
            Vector3 velocity = CharacterMotor.velocity;

            Vector3 desiredVelocity = _movementDirection * maxSpeed;

            if (CharacterMotor.isGrounded)
                velocity = Vector3.MoveTowards(velocity, desiredVelocity, maxAcceleration * deltaTime);
            else
            {
                if (desiredVelocity != Vector3.zero)
                {
                    Vector3 groundNormal = CharacterMotor.groundNormal;
                    if (groundNormal != Vector3.zero)
                    {
                        Vector3 groundNormal2D = groundNormal.onlyXZ();
                        desiredVelocity = desiredVelocity.projectedOnPlane(groundNormal2D);
                    }

                    Vector3 horizontalVelocity = Vector3.MoveTowards(velocity.onlyXZ(), desiredVelocity,
                        maxAcceleration * airControl * deltaTime);

                    velocity = horizontalVelocity + velocity.onlyY();
                }

                velocity += gravity * deltaTime;

                velocity -= velocity * airFriction * deltaTime;
            }

            // jump

            if (_jumpButtonPressed && CharacterMotor.isGrounded)
            {
                _jumpButtonPressed = false;

                Vector3 characterUp = transform.up;
                float actualJumpImpulse = Mathf.Max(Vector3.Dot(CharacterMotor.velocity, characterUp), jumpImpulse);

                CharacterMotor.PauseGroundConstraint();
                CharacterMotor.LaunchCharacter(characterUp * actualJumpImpulse, true);
            }

            // Do movement

            CharacterMotor.Move(velocity, deltaTime);
        }

        private void HandleInput()
        {
            // Movement input

            _movementDirection = Vector3.zero;

            _movementDirection += Vector3.forward * Input.GetAxisRaw($"Vertical");
            _movementDirection += Vector3.right * Input.GetAxisRaw($"Horizontal");

            _movementDirection = Vector3.ClampMagnitude(_movementDirection, 1.0f);

            // Jump input

            _jumpButtonPressed = Input.GetButton($"Jump");
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            CharacterMotor = GetComponent<CharacterMotor>();
            CharacterMotor.interpolation = RigidbodyInterpolation.None;

            CharacterMotor.enablePhysicsInteraction = false;

            CharacterMotor.impartPlatformMovement = true;
            CharacterMotor.impartPlatformRotation = true;
            CharacterMotor.impartPlatformVelocity = true;
        }

        private void OnEnable()
        {
            SimulationManager.instance.AddSimulatable(this);
        }

        private void OnDisable()
        {
            SimulationManager.instance.RemoveSimulatable(this);
        }

        private void Update()
        {
            HandleInput();
        }

        #endregion
    }
}
