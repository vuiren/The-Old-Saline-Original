using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Inventory))]
public class Container : InteractableObject
{
    private Inventory containerInventory;

    public Inventory GetContainerInventory()
    {
        return containerInventory;
    }
    
    private UIInventory inventoryMenu;

    public override void OnInteract()
    {
        inventoryMenu.StartInventory(containerInventory);
    }

    // void Start()
    // {
    //     containerInventory = GetComponent<Inventory>();
    //     inventoryMenu = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<UIInventory>();
    // }

    private bool findPlayerUI;

    private void Update() 
    {
        if (GameObject.FindGameObjectWithTag("PlayerUI") != null && !findPlayerUI)
        {
            containerInventory = GetComponent<Inventory>();
            inventoryMenu = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<UIInventory>();
            findPlayerUI = true;
        }
    }
}
