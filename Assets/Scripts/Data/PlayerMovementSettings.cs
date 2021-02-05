using System;
using UnityEngine;

namespace Assets.Scripts.NewCode.Data
{
    [Serializable]
    public class MouseLookSettings
    {
        //  [Header("Mouse Look Settings")]
        [SerializeField, Range(0f, 1000f)] private float mouseSpeed = 10f;
        [SerializeField, Range(0.0f, 1000.0f)] private float sensitiveX = 5.0f;
        [SerializeField, Range(0.0f, 1000.0f)] private float sensitiveY = 5.0f;
        [SerializeField, Range(0.0f, 360.0f)] private float viewingAngleX = 360, viewingAngleY = 90;

        public float SensitiveX { get => sensitiveX; }
        public float SensitiveY { get => sensitiveY; }
        public float ViewingAngleX { get => viewingAngleX; }
        public float ViewingAngleY { get => viewingAngleY; }
        public float MouseSpeed { get => mouseSpeed; }
    }

    [Serializable]
    public class MoveSettings
    {
        [SerializeField, Range(0.0f, 10.0f)] private float speed = 5.0f;
        [SerializeField, Range(0.0f, 50.0f)] private float acceleratorSpeed = 10.0f;

        public float Speed { get => speed; }
        public float AcceleratorSpeed { get => acceleratorSpeed; }
    }

    [Serializable]
    public class JumpSettings
    {
        [SerializeField, Range(0.0f, 10.0f)] private float jumpStrength = 4.25f;
        [SerializeField, Range(0.0f, 1.0f)] private float groundCheckDistance = 0.5f;

        public float JumpStrength { get => jumpStrength; }
        public float GroundCheckDistance { get => groundCheckDistance; }
    }

    [Serializable]
    public class CrouchSettings
    {
        [SerializeField, Range(0.0f, 2.0f)] private float heightCrouch = 0.75f;
        [SerializeField, Range(0.0f, 2.0f)] private float heightStay = 1.5f;
        [SerializeField, Range(0.0f, 10.0f)] private float crouchMoveSpeed = 3.0f;
        [SerializeField, Range(0.0f, 5.0f)] private float crouchingSpeed = 2.5f;
        [SerializeField, Range(0.0f, 0.1f)] private float obstacleCheckDistance = 0.05f;

        public float HeightCrouch { get => heightCrouch; }
        public float HeightStay { get => heightStay; }
        public float CrouchMoveSpeed { get => crouchMoveSpeed; }
        public float CrouchingSpeed { get => crouchingSpeed; }
        public float ObstacleCheckDistance { get => obstacleCheckDistance; }
    }

    [Serializable]
    public class SwimSettings
    {
        [SerializeField, Range(0.0f, 100.0f)] private float air = 50.0f;
        [SerializeField, Range(0.0f, 10.0f)] private float swimmingSpeed = 2.0f;
        [SerializeField, Range(0.0f, 10.0f)] private float swimmingAcceleratorSpeed = 3.0f;

        public float Air { get => air; }
        public float SwimmingSpeed { get => swimmingSpeed; }
        public float SwimmingAcceleratorSpeed { get => swimmingAcceleratorSpeed; }
    }

    [Serializable]
    public class ClimbSettings
    {
        [SerializeField, Range(0.0f, 10.0f)] private float climbingSpeed = 5.0f;
        [SerializeField, Range(0.0f, 1.0f)] private float climbingDistance = 0.2f;
        [SerializeField, Range(0.0f, 5.0f)] private float climbingHeightMax = 2.1f;
        [SerializeField, Range(0.0f, 5.0f)] private float climbingHeightMin = 0.2f;
        [SerializeField, Range(0.0f, 1.0f)] private float ledgeCheckDistance = 0.2f;
        [SerializeField, Range(-0.5f, 0.5f)] private float climbingError = 0.05f;

        public float ClimbingSpeed { get => climbingSpeed; }
        public float ClimbingDistance { get => climbingDistance; }
        public float ClimbingHeightMax { get => climbingHeightMax; }
        public float ClimbingHeightMin { get => climbingHeightMin; }
        public float LedgeCheckDistance { get => ledgeCheckDistance; }
        public float ClimbingError { get => climbingError; }
    }

    [CreateAssetMenu(fileName = "New Player Settings", menuName = "Create Player Settings")]
    public class PlayerMovementSettings : ScriptableObject
    {
        [SerializeField]
        MouseLookSettings mouseLookSettings;

        [SerializeField]
        MoveSettings moveSettings;

        [SerializeField]
        JumpSettings jumpSettings;

        [SerializeField]
        CrouchSettings crouchSettings;

        [SerializeField]
        SwimSettings swimSettings;

        [SerializeField]
        ClimbSettings climbSettings;

        public MouseLookSettings MouseLookSettings { get => mouseLookSettings; }
        public MoveSettings MoveSettings { get => moveSettings; }
        public JumpSettings JumpSettings { get => jumpSettings; }
        public CrouchSettings CrouchSettings { get => crouchSettings; }
        public SwimSettings SwimSettings { get => swimSettings; }
        public ClimbSettings ClimbSettings { get => climbSettings; }
    }
}