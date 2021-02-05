using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MachineInteraction : MonoBehaviour
{
    private bool onTrigger, canPress;
    private GameObject newCamera;
    private GameObject mainCamera;
    private GameObject machine;
    private GameObject AnalysisText;
    [SerializeField] private UIInventory InventoryMenu;

    private void Awake() 
    {
       // GetComponent<Control>().PlayerControls.PlayerControl.Interaction.started += OnHandleInteraction;
       // GetComponent<Control>().PlayerControls.PlayerOther.Press.started += OnHandlePress;
    }

    private void OnHandleInteraction(InputAction.CallbackContext context)
    {
        if (!onTrigger)
                return;
        
        canPress = !canPress;

        if (Cursor.visible)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            GetComponent<Control>().PlayerControls.PlayerControl.Move.Enable();
            GetComponent<Control>().PlayerControls.PlayerControl.Jump.Enable();
            GetComponent<Control>().PlayerControls.PlayerControl.Crouch.Enable();
            GetComponent<Control>().PlayerControls.PlayerControl.Look.Enable();
            GetComponent<Control>().PlayerControls.PlayerControl.Fire.Enable();
            GetComponent<Control>().PlayerControls.PlayerOther.Inventory.Enable();
            
            mainCamera.SetActive(true);
            newCamera.SetActive(false);
   
            machine.GetComponent<BoxCollider>().enabled = true;
            transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().enabled = true;
            transform.GetComponent<CharacterController>().enabled = true;

            return;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GetComponent<Control>().PlayerControls.PlayerControl.Move.Disable();
        GetComponent<Control>().PlayerControls.PlayerControl.Jump.Disable();
        GetComponent<Control>().PlayerControls.PlayerControl.Crouch.Disable();
        GetComponent<Control>().PlayerControls.PlayerControl.Look.Disable();
        GetComponent<Control>().PlayerControls.PlayerControl.Fire.Disable();
        GetComponent<Control>().PlayerControls.PlayerOther.Inventory.Disable();

        mainCamera.SetActive(false);
        newCamera.SetActive(true);
        machine.GetComponent<BoxCollider>().enabled = false;
        transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().enabled = false;
        transform.GetComponent<CharacterController>().enabled = false;
    }

    private void OnHandlePress(InputAction.CallbackContext context)
    {
        var mouseVect = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0);
        if(canPress)
        {
            if(Physics.Raycast(newCamera.GetComponent<Camera>().ScreenPointToRay(mouseVect), out RaycastHit hit))
            {
                if (hit.transform.name.Equals("Entry"))
                {
                    var inventory = InventoryMenu.transform.GetChild(1);
                    if (!inventory.GetChild(2).gameObject.activeSelf)
                    {
                        machine.transform.GetChild(9).GetComponent<Container>().OnInteract(gameObject);
                        inventory.GetChild(3).gameObject.SetActive(false);
                        inventory.GetChild(4).gameObject.SetActive(false);
                        inventory.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "Substance";
                    }
                    else
                    {
                        InventoryMenu.CloseInventory();

                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                        GetComponent<Control>().PlayerControls.PlayerControl.Move.Disable();
                        GetComponent<Control>().PlayerControls.PlayerControl.Jump.Disable();
                        GetComponent<Control>().PlayerControls.PlayerControl.Crouch.Disable();
                        GetComponent<Control>().PlayerControls.PlayerControl.Look.Disable();
                        GetComponent<Control>().PlayerControls.PlayerControl.Fire.Disable();
                        GetComponent<Control>().PlayerControls.PlayerOther.Inventory.Disable();
                    }
                    Debug.Log("Entrance");
                }
                else if (hit.transform.name.Equals("Button1"))
                {
                    getText = !getText;
                    time = updateTime;
                    if (!getText)
                        AnalysisText.SetActive(false);
                    Debug.Log("Analysis");
                }
                else if (hit.transform.name.Equals("ExitDoors"))
                {
                    var inventory = InventoryMenu.transform.GetChild(1);
                    if (!inventory.GetChild(2).gameObject.activeSelf)
                    {
                        machine.transform.GetChild(9).GetComponent<Container>().OnInteract(gameObject);
                        inventory.GetChild(3).gameObject.SetActive(false);
                        inventory.GetChild(4).gameObject.SetActive(false);
                        inventory.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "Substance";
                    }
                    else
                    {
                        InventoryMenu.CloseInventory();

                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                        GetComponent<Control>().PlayerControls.PlayerControl.Move.Disable();
                        GetComponent<Control>().PlayerControls.PlayerControl.Jump.Disable();
                        GetComponent<Control>().PlayerControls.PlayerControl.Crouch.Disable();
                        GetComponent<Control>().PlayerControls.PlayerControl.Look.Disable();
                        GetComponent<Control>().PlayerControls.PlayerControl.Fire.Disable();
                        GetComponent<Control>().PlayerControls.PlayerOther.Inventory.Disable();
                    }

                    Debug.Log("Output");
                }
            }
        }
    }

    private bool getText;
    public float updateTime;
    private float time;

    private void Update() 
    {
        if (getText)
        {
            time -= Time.deltaTime;
            if (time.CompareTo(0) < 0)
                AnalysisText.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Machine"))
        {
            onTrigger = true;
            newCamera = other.gameObject.transform.GetChild(other.gameObject.transform.childCount - 1).gameObject;
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            machine = GameObject.FindGameObjectWithTag("Machine");
            AnalysisText = machine.transform.GetChild(machine.transform.childCount - 2).gameObject; 
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Machine"))
            onTrigger = false;
    }
}