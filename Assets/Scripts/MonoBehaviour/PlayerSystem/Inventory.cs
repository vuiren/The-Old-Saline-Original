using UnityEngine;

public class Inventory : MonoBehaviour
{
    public enum InventoryType 
    {
        Hero,
        Container
    }

    [SerializeField] private InventoryType type = InventoryType.Container;
    [SerializeField] private Item[] startInventory;
    [SerializeField, Range(0, 10)] private int heightCountCell = 5;
    [SerializeField, Range(0, 10)] private int widthCountCell = 5;
    public ItemInventory[,] charInventory;
    public bool IsStartItemAdded {get; private set;}
    public GameObject InventoryPanel {get; private set;}

    public InventoryType GetInventoryType => type;
    public Item[] GetStartInventory => startInventory;
    public int GetHeightCountCell => heightCountCell;
    public int GetWidthCountCell => widthCountCell;

    public void StartItemAdded() => IsStartItemAdded = true;

    // private void Start()
    // {
    //     charInventory = new ItemInventory[widthCountCell, heightCountCell];
    //     for (var i = 0; i < widthCountCell; i++)
    //         for (var j = 0; j < heightCountCell; j++)
    //             charInventory[i, j] = new ItemInventory();
        
    //     var inventoryMenu =  GameObject.FindGameObjectWithTag("PlayerUI").transform.Find("InventoryMenu");
    //     switch(type)
    //     {
    //         case InventoryType.Hero:
    //             InventoryPanel = inventoryMenu.Find("PlayerPanel").gameObject;
    //             break;
    //         case InventoryType.Container:
    //             InventoryPanel = inventoryMenu.Find("ContainerPanel").gameObject;
    //             break;
    //         default:
    //             Debug.LogError("No InventoryType");
    //             break;
    //     }
    // }

    private bool findPlayerUI;
    private void Update() 
    {
        if (GameObject.FindGameObjectWithTag("PlayerUI") != null && !findPlayerUI)
        {
           charInventory = new ItemInventory[widthCountCell, heightCountCell];
           for (var i = 0; i < widthCountCell; i++)
               for (var j = 0; j < heightCountCell; j++)
                   charInventory[i, j] = new ItemInventory();
            
           var inventoryMenu =  GameObject.FindGameObjectWithTag("PlayerUI").transform.Find("InventoryMenu");
           switch(type)
           {
               case InventoryType.Hero:
                   InventoryPanel = inventoryMenu.Find("PlayerPanel").gameObject;
                   break;
               case InventoryType.Container:
                   InventoryPanel = inventoryMenu.Find("ContainerPanel").gameObject;
                   break;
               default:
                   Debug.LogError("No InventoryType");
                   break;
           }
           findPlayerUI = true;
        }
    }
}