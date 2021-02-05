using System;
using UnityEngine;
using Assets.Scripts.NewCode.Data;
using Assets.Scripts.NewCode.Singletons;

enum PlayerPhysicsStates
{
    InAir,
    InWater,
    Climbing,
}

[RequireComponent(typeof(CharacterController))]
public class Control
{
    public PlayerControlsInputSystem PlayerControls { get; set; }

    public PlayerMovementSettings playerMovementSettings;

    #region Main Camera Settings
    [Header("Main Camera Settings")]
    [SerializeField] private GameObject mainCamera;
    private bool IsNewEffect;
    #endregion

    #region Mouse Look Settings
    private float MouseSpeed => playerMovementSettings.MouseLookSettings.MouseSpeed;
    private float SensitiveX => playerMovementSettings.MouseLookSettings.SensitiveX;
    private float SensitiveY => playerMovementSettings.MouseLookSettings.SensitiveY;
    private float ViewingAngleX => playerMovementSettings.MouseLookSettings.ViewingAngleX;
    private float ViewingAngleY => playerMovementSettings.MouseLookSettings.ViewingAngleY;
    private float rotationX, rotationY;
    private Quaternion originalRotation;
    #endregion

    #region Move Settings
    private float Speed => playerMovementSettings.MoveSettings.Speed;
    private float AcceleratorSpeed => playerMovementSettings.MoveSettings.AcceleratorSpeed;
    #endregion

    #region Jump Settings
    private float JumpStrength => playerMovementSettings.JumpSettings.JumpStrength;
    private float GroundCheckDistance => playerMovementSettings.JumpSettings.GroundCheckDistance;
    private bool isJumping;
    #endregion

    #region Crouch Settings
    private float HeightCrouch => playerMovementSettings.CrouchSettings.HeightCrouch;
    private float HeightStay => playerMovementSettings.CrouchSettings.HeightStay;
    private float CrouchMoveSpeed => playerMovementSettings.CrouchSettings.CrouchMoveSpeed;
    private float CrouchingSpeed => playerMovementSettings.CrouchSettings.CrouchingSpeed;
    private float ObstacleCheckDistance => playerMovementSettings.CrouchSettings.ObstacleCheckDistance;
    private bool isCrouching, isGettingUp, isStay = true;
    #endregion

    #region Swim Settings
    private float Air => playerMovementSettings.SwimSettings.Air;
    private float SwimingSpeed => playerMovementSettings.SwimSettings.SwimmingSpeed;
    private float SwimingAcceleratorSpeed => playerMovementSettings.SwimSettings.SwimmingAcceleratorSpeed;
    private bool isDiving;
    private float currentAir;
    private Water WaterType { get; set; }
    #endregion

    #region Climbing Settings
    private float ClimbingSpeed => playerMovementSettings.ClimbSettings.ClimbingSpeed;
    private float ClimbingDistance => playerMovementSettings.ClimbSettings.ClimbingDistance;
    private float ClimbingHeightMax => playerMovementSettings.ClimbSettings.ClimbingHeightMax;
    private float ClimbingHeightMin => playerMovementSettings.ClimbSettings.ClimbingHeightMin;
    private float LedgeCheckDistance => playerMovementSettings.ClimbSettings.LedgeCheckDistance;
    private float ClimbingError => playerMovementSettings.ClimbSettings.ClimbingError;
    private Vector3 ClimbPosition { get; set; }
    private bool isUp;
    #endregion

    #region Other Settings
    [Header("Other Settings")]
    [SerializeField] private UIMenuPauseButtonEvent MenuPause;
    [SerializeField] private UIInventory InventoryMenu;
    [SerializeField] private PlayerPhysicsStates CurrentState { get; set; } = PlayerPhysicsStates.InAir;
    private float SkinWidth;
    private CharacterController Controller;
    private Vector3 CeilingHit;
    #endregion

    public InputSingleton InputInstance => InputSingleton.Instance;

    private readonly GameObject playerGO;

    public bool TakingInput { get; set; } = true;
    public bool TakingMouseInput { get; set; } = true;

    public Vector3 Velocity { get => velocity; set => velocity = value; }
    private Vector3 velocity;

    public Control(PlayerMovementSettings playerMovementSettings, GameObject mainCamera, UIMenuPauseButtonEvent menuPause, UIInventory inventoryMenu, CharacterController controller, GameObject playerGO)
    {
        this.playerMovementSettings = playerMovementSettings;
        this.mainCamera = mainCamera;
        MenuPause = menuPause;
        InventoryMenu = inventoryMenu;
        Controller = controller;
        this.playerGO = playerGO;

        OnEnable();
        Awake();
        Start();
    }

    public Action CrouchDelegate { get; set; }
    public Action JumpStartDelegate { get; set; }
    public Action JumpCancelDelegate { get; set; }

    private void OnEnable()
    {
        SetInput();
    }

    private void SetInput()
    {
        PlayerControls = new PlayerControlsInputSystem();

        CrouchDelegate = () =>
        {
            isCrouching = true;
            velocity.x *= 0.5f;
            velocity.z *= 0.5f;
        };

        JumpStartDelegate = () =>
        {
            isJumping = ObstacleCheck(playerGO.transform.position, -GroundCheckDistance, -HeightStay * 0.5f, 0.0f);
            if (LedgeCheck() && !isDiving)
            {
                CurrentState = PlayerPhysicsStates.Climbing;
                TakingMouseInput = false;
            }
        };

        JumpCancelDelegate = () =>
        {
            isJumping = false;
        };
    }

    public void CeilingCheck(ControllerColliderHit hit)
    {
        if (((Controller.collisionFlags & (CollisionFlags.Above)) != 0) && velocity.y > .0f)
        {
            velocity.y = 0;
            isJumping = false;
        }

        if (Controller.collisionFlags == CollisionFlags.Above)
        {
            CeilingHit = hit.point.y > playerGO.transform.position.y ? hit.point : CeilingHit;
        }
    }

    private void Awake()
    {
        SetInput();
    }

    private void Start()
    {
        SkinWidth = Controller.skinWidth * 2;
        originalRotation = playerGO.transform.localRotation;
    }

    public void Update()
    {
        if (TakingMouseInput)
            CharacterLook();
        CharacterPhysics();

        Vector3 moveVector = Vector3.zero;
        if (TakingInput)
        {
            moveVector = GetMoveVector(CurrentState);

            if (CurrentState == PlayerPhysicsStates.InAir)
            {
                CharacterCrouch();
                CharacterJump(ref moveVector);
            }
        }

        CollisionFlags previousTouchSides = Controller.Move(moveVector * Time.deltaTime);
        if ((int)previousTouchSides == 7) Controller.Move(CorrectionMove() * Time.deltaTime);
    }

    public void EnterWater(Water water)
    {
        CurrentState = PlayerPhysicsStates.InWater;
        velocity.x *= 0.5f;
        velocity.z *= 0.5f;
        WaterType = water;
    }

    public void ExitWater()
    {
        CurrentState = CurrentState == PlayerPhysicsStates.Climbing ? PlayerPhysicsStates.Climbing : PlayerPhysicsStates.InAir;
        WaterType = null;
    }

    private void CharacterJump(ref Vector3 moveVector)
    {
        if (Controller.isGrounded && isJumping && isStay)
        {
            velocity.y = JumpStrength;
            isJumping = false;
        }
        moveVector.y = velocity.y;
    }

    private void CharacterPhysics()
    {
        if (CurrentState == PlayerPhysicsStates.InAir && !Controller.isGrounded
            && InputInstance.JumpKeyPressed && LedgeCheck())
        {
            CurrentState = PlayerPhysicsStates.Climbing;
            TakingMouseInput = false;
        }
        velocity.y = GetGravityValue(CurrentState);
    }

    private Vector3 GetMoveVector(PlayerPhysicsStates state)
    {
        return state switch
        {
            PlayerPhysicsStates.InAir => CharacterMove(),
            PlayerPhysicsStates.InWater => CharacterSwim(),
            PlayerPhysicsStates.Climbing => CharacterClimbing(),
            _ => Vector3.zero,
        };
    }

    private float GetGravityValue(PlayerPhysicsStates state)
    {
        return state switch
        {
            PlayerPhysicsStates.InAir => !Controller.isGrounded ? velocity.y + Physics.gravity.y * Time.deltaTime : 0,//Gravity
            PlayerPhysicsStates.InWater => velocity.y < 0 ? velocity.y + WaterType.GetViscosity * Time.deltaTime : 0,//Swimming
            PlayerPhysicsStates.Climbing => 0f,
            _ => 0f,
        };
    }

    private Vector3 CharacterMove()
    {
        Vector2 movementDirection = InputInstance.MoveDirection;

        Vector3 resultVector = GetMoveVector(playerGO, movementDirection, AcceleratorSpeed,
            (isStay && !isCrouching && Velocity.z < Speed) || ((!isStay || isCrouching) && Velocity.z < CrouchMoveSpeed));

        return resultVector;
    }

    private void CharacterCrouch()
    {
        if (isCrouching && isStay)
        {
            SetOffsetControllerAndCamera(CrouchingSpeed, HeightCrouch - SkinWidth);
            if (Controller.height + SkinWidth <= HeightCrouch + 0.01f)
            {
                isGettingUp = isStay = isCrouching = false;
            }
        }
        else if (!isStay && (isCrouching || isJumping) && !ObstacleCheck(playerGO.transform.position, HeightStay * 0.5f, ObstacleCheckDistance, 0.0f))
        {
            isCrouching = true;
            SetOffsetControllerAndCamera(CrouchingSpeed, HeightStay - SkinWidth);
            if (Controller.height + SkinWidth >= HeightStay - 0.01f)
            {
                isStay = true;
                isGettingUp = isCrouching = false;
            }
        }
        else if (isCrouching && !isGettingUp) isCrouching = false;
    }

    private void CharacterLook()
    {
        Vector2 movementVector = MouseSpeed * InputInstance.MouseLook * Time.deltaTime;
        rotationX = Mathf.Clamp((rotationX + movementVector.x * SensitiveX) % 360, -ViewingAngleX, ViewingAngleX);
        rotationY = Mathf.Clamp((rotationY + movementVector.y * SensitiveY) % 360, -ViewingAngleY, ViewingAngleY);

        mainCamera.transform.localRotation = originalRotation * Quaternion.AngleAxis(rotationY, Vector3.left);
        playerGO.transform.localRotation = originalRotation * Quaternion.AngleAxis(rotationX, Vector3.up);
    }

    private Vector3 CharacterSwim()
    {
        if (isStay)
        {
            isCrouching = true;
            CharacterCrouch();
        }

        if (mainCamera.transform.position.y <= WaterType.WaterLevelY)
        {
            isDiving = true;
        }
        else
        {
            isDiving = false;
        }

        currentAir = isDiving ? currentAir - Time.deltaTime : Air;

        Vector2 movementDirection = InputInstance.MoveDirection;
        Vector3 resultVector = GetMoveVector(mainCamera, movementDirection, SwimingAcceleratorSpeed, Velocity.z < SwimingSpeed);
        resultVector.y += velocity.y;
        return resultVector;
    }

    private Vector3 CharacterClimbing()
    {
        if (isCrouching && isStay)
        {
            CharacterCrouch();
        }
        var transform = playerGO.transform;

        var resultMoveVector = isUp ?
            Vector3.up * (ClimbPosition.y - transform.position.y) * ClimbingSpeed :
            (ClimbPosition - transform.position).normalized * ClimbingSpeed;

        if (isUp)
        {
            isUp = !(Math.Abs(transform.position.y - ClimbPosition.y) <= ClimbingError);
        }
        else
        {
            if (Math.Abs(transform.position.x - ClimbPosition.x) <= ClimbingError && Math.Abs(transform.position.z - ClimbPosition.z) <= ClimbingError)
            {
                CurrentState = PlayerPhysicsStates.InAir;
                velocity.y = 0.0f;
                if (true)
                {
                    TakingMouseInput = true;
                }
            }
        }
        return resultMoveVector;
    }

    #region Helper Methods
    private void SetOffsetControllerAndCamera(float actionSpeed, float height)
    {
        isGettingUp = true;
        actionSpeed *= Time.deltaTime;
        float newHeight = Mathf.MoveTowards(Controller.height, height, actionSpeed);
        var razn = newHeight - Controller.height;

        Controller.height = newHeight;
        Controller.center = new Vector3(0, Controller.center.y + razn / 2, 0);
        var oldCameraPosition = mainCamera.transform.position;
        mainCamera.transform.position = new Vector3(oldCameraPosition.x, oldCameraPosition.y + razn, oldCameraPosition.z);
    }

    private bool ObstacleCheck(Vector3 playerPosition, float bottomParameter, float topParameter, float radius)
    {
        Vector3 bottom = playerPosition + Vector3.up * (Controller.radius + Controller.skinWidth + radius + bottomParameter);
        Vector3 top = playerPosition + Vector3.up * (HeightStay - Controller.radius - radius + topParameter);
        bool result = Physics.CheckCapsule(bottom, top, Controller.radius + radius, ~(1 << 9), QueryTriggerInteraction.Ignore);
        Debug.DrawLine(bottom, top, Color.red, 5.0f);
        return result;
    }

    private bool LedgeCheck()
    {
        var transform = playerGO.transform;
        var result = false;
        Debug.DrawLine(transform.position + Vector3.up * HeightStay * 0.5f, transform.position + Vector3.up * HeightStay * 1.5f, Color.cyan, 5.0f);
        if (!isGettingUp && !isCrouching
            && !Physics.Linecast(transform.position + Vector3.up * HeightStay * 0.5f, transform.position + Vector3.up * HeightStay * 2.0f)
            && ObstacleCheck(transform.position + transform.forward + new Vector3(LedgeCheckDistance, 0.0f, LedgeCheckDistance), 0.0f, 0.0f, 0.0f))
        {
            Vector3 groundCheckDistance = transform.position + transform.forward.normalized * (ClimbingDistance + Controller.radius);
            Vector3 groundCheckStart = groundCheckDistance + Vector3.up * ClimbingHeightMax;
            Vector3 groundCheckEnd = groundCheckDistance + Vector3.up * ClimbingHeightMin;
            Debug.DrawLine(groundCheckStart, groundCheckEnd, Color.blue, 5.0f);
            RaycastHit positionHit;
            if (Physics.Linecast(groundCheckStart, groundCheckEnd, out positionHit, ~(1 << 9), QueryTriggerInteraction.Ignore)
                && !ObstacleCheck(positionHit.point, ClimbingError, -(HeightStay - HeightCrouch) + ClimbingError, -ClimbingError))
            {
                result = isUp = true;
                ClimbPosition = positionHit.point;
                isCrouching = ObstacleCheck(positionHit.point, ClimbingError, ClimbingError, -ClimbingError);
            }
        }
        return result;
    }

    private Vector3 GetMoveVector(GameObject objectRelativity, Vector2 direction, float acceleratorSpeed, bool shouldAccelerate)
    {
        Vector3 moveVector = Vector3.zero;
        Accelerate(acceleratorSpeed, shouldAccelerate);
        if (Input.anyKey)
        {
            moveVector = objectRelativity.transform.forward * direction.y * Velocity.z +
                objectRelativity.transform.right * direction.x * Velocity.z / 2;
        }
        else
            velocity.x = velocity.z = 0f;
        return moveVector;
    }

    private void Accelerate(float acceleratorSpeed, bool shouldAccelerate)
    {
        if (shouldAccelerate)
        {
            velocity.x += acceleratorSpeed * Time.deltaTime;
            velocity.z += acceleratorSpeed * Time.deltaTime;
        }
    }

    private Vector3 CorrectionMove()
    {
        var transform = playerGO.transform;
        velocity.x = velocity.z = 0.0f;
        Vector3 ceilingVector = CeilingHit - transform.position;
        var result = new Vector3(-(ceilingVector.z * transform.forward.z) / (ceilingVector.x), 0, transform.forward.z).normalized;
        Debug.DrawLine(transform.position, transform.position + result, Color.green, 5.0f);
        return result;
    }
    #endregion
}