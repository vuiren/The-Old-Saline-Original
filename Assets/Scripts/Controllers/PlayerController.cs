using Assets.Scripts.Components;
using Assets.Scripts.NewCode.Data;
using Assets.Scripts.NewCode.Singletons;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Assets.Scripts.Controllers
{
    class PlayerController: MonoBehaviour
    {
        [SerializeField]
        PlayerMovementSettings movementSettings;

        [SerializeField]
        private GameObject playerGO;

        [SerializeField]
        private GameObject playerHead;

        [SerializeField]
        PostProcessVolume postProcess;

        [SerializeField] 
        private PostProcessProfile onAirProfile;

        [SerializeField]
        private PostProcessProfile inWaterProfile;

        [SerializeField] 
        private UIMenuPauseButtonEvent menuPause;

        [SerializeField] 
        private UIInventory inventoryMenu;

        [SerializeField, Range(0.0f, 10.0f)] 
        private float interactDistance = 1.0f;

        [SerializeField]
        Transform raycastStartPoint;

        [SerializeField]
        LayerMask interactionLayers;

        Camera playerCamera;

        CharacterController controller;
        Control control;
        WaterTrigger waterTrigger;
        Interactor interactor;
        CameraFiltersApplier filtersApplier;

        Action pauseDelegate;
        Action inventoryDelegate;

        private void OnEnable()
        {
            playerCamera = Camera.main;
            AddComponents();
            ConnectComponents();
            ConnectInput();

            DontDestroyOnLoad(gameObject);
        }

        private void OnDisable()
        {
            DisconnectComponents();
            DisconnectInput();
        }
        private void AddComponents()
        {
            controller = gameObject.GetComponent<CharacterController>();
            control = new Control(movementSettings, playerCamera.gameObject, menuPause, inventoryMenu, controller, playerGO);
            waterTrigger = gameObject.AddComponent<WaterTrigger>();
            interactor = new Interactor(interactDistance, raycastStartPoint, interactionLayers);
            filtersApplier = new CameraFiltersApplier(playerHead.transform, onAirProfile, inWaterProfile, postProcess);
        }

        private void ConnectInput()
        {
            var input = InputSingleton.Instance;
            input.OnLMBDown += interactor.CharacterInteract;
            input.OnCrouchKeyPressed += control.CrouchDelegate;
            input.OnJumpKeyDown += control.JumpStartDelegate;
            input.OnJumpKeyUp += control.JumpCancelDelegate;

            pauseDelegate = () =>
            {
                if (!inventoryMenu.IsInventoryMenuOpen)
                    menuPause.OnShowMenuPause();
                else
                    inventoryMenu.CloseInventory();

                if(menuPause.IsPauseMenuOpen)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            };

            inventoryDelegate = () =>
            {
                if (!menuPause.IsPauseMenuOpen)
                    inventoryMenu.StartInventory();
            };

            input.OnInventoryOpenKeyPressed += inventoryDelegate;
            input.OnEscapeKeyPressed += pauseDelegate;
        }
        private void DisconnectInput()
        {
            var input = InputSingleton.Instance;
            if (!input) return;
            input.OnLMBDown -= interactor.CharacterInteract;
            input.OnCrouchKeyPressed -= control.CrouchDelegate;
            input.OnJumpKeyDown -= control.JumpStartDelegate;
            input.OnJumpKeyUp -= control.JumpCancelDelegate;

            input.OnInventoryOpenKeyPressed -= inventoryDelegate;
            input.OnEscapeKeyPressed -= pauseDelegate;
        }

        private void ConnectComponents()
        {
            waterTrigger.OnWaterEnter += control.EnterWater;
            waterTrigger.OnWaterExit += control.ExitWater;
            waterTrigger.OnWaterEnter += filtersApplier.TryToApplyWaterFilter;
            waterTrigger.OnWaterExit += filtersApplier.ApplyOnAirFilter;
        }
        private void DisconnectComponents()
        {
            waterTrigger.OnWaterEnter -= control.EnterWater;
            waterTrigger.OnWaterExit -= control.ExitWater;
            waterTrigger.OnWaterEnter -= filtersApplier.TryToApplyWaterFilter;
            waterTrigger.OnWaterExit -= filtersApplier.ApplyOnAirFilter;
        }

        private void Update()
        {
            control.Update();
            filtersApplier.Update();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            control.CeilingCheck(hit);
        }
    }
}
