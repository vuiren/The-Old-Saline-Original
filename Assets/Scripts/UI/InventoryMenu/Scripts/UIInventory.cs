using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject prefabCell;
    [SerializeField] private GameObject prefabItemImage;
    [SerializeField] private GameObject panelForMoving;
    [SerializeField] private GameObject playerItemImages;
    [SerializeField] private GameObject containerItemImages;
    [SerializeField] private GameObject allToContainer;
    [SerializeField] private GameObject allToPlayer;
    private Inventory playerInventory;
    private Inventory containerInventory;
    private Control playerControl;
    public bool IsInventoryMenuOpen {get; private set;}
    
    public void OnClickAllToHero() => AllTo(playerInventory, containerInventory);

    public void OnClickAllToEnemy() => AllTo(containerInventory, playerInventory);

    void Start() 
    {
      //  playerInventory = player.GetComponent<Inventory>();
       // playerControl = player.GetComponent<Control>();
    }

    public void StartInventory(Inventory objectInventory = null)
    {
        if(!playerInventory.InventoryPanel.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            player.GetComponent<Control>().PlayerControls.PlayerControl.Disable();
            IsInventoryMenuOpen = true;
            containerInventory = objectInventory;
            if (!playerInventory.InventoryPanel.activeSelf)
                DrawInventory(playerInventory);
            StartCoroutine(UpdateInventory(playerInventory));
            playerInventory.InventoryPanel.SetActive(true);
            if(!(objectInventory is null))
            {
                if (!objectInventory.InventoryPanel.activeSelf)
                    DrawInventory(objectInventory);
                StartCoroutine(UpdateInventory(objectInventory));
                objectInventory.InventoryPanel.SetActive(true);
                allToContainer.SetActive(true);
                allToPlayer.SetActive(true);
            }

            panelForMoving.SetActive(true);
            mainPanel.SetActive(true);
        }
        else
            CloseInventory();
    }

    public IEnumerator UpdateInventory(Inventory objectInventory)
    {
        yield return new WaitForEndOfFrame();
        GameObject inventory = (objectInventory.GetInventoryType == Inventory.InventoryType.Hero) ? playerItemImages : containerItemImages;
        var charInventory = objectInventory.charInventory;
        if (!objectInventory.IsStartItemAdded)
        {
            foreach (var item in objectInventory.GetStartInventory)
                AddItemToInventory(item, objectInventory);
            objectInventory.StartItemAdded();
        }
        for (var i = 0; i < charInventory.GetLength(0); i++)
            for (var j = 0; j < charInventory.GetLength(1); j++)   
                if (charInventory[i, j].occupiedCell)
                    inventory.transform.GetChild(i + j * charInventory.GetLength(0)).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1);
                else
                    inventory.transform.GetChild(i + j * charInventory.GetLength(0)).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        FillItemImage(objectInventory);
    }

    public void CloseInventory()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mainPanel.SetActive(false);
        player.GetComponent<Control>().PlayerControls.PlayerControl.Enable();
        IsInventoryMenuOpen = false;
        for (var i = 0; i < panelForMoving.transform.childCount; i++)
            Destroy(panelForMoving.transform.GetChild(i).gameObject, 0);
        for (var i = 0; i < playerItemImages.transform.childCount; i++)
            Destroy(playerItemImages.transform.GetChild(i).gameObject, 0);
        foreach(var item in playerInventory.charInventory)
            item.drawn = false;
        playerInventory.InventoryPanel.SetActive(false);

        if(!(containerInventory is null))
        {
            for (var i = 0; i < containerItemImages.transform.childCount; i++)
                Destroy(containerItemImages.transform.GetChild(i).gameObject, 0);
            foreach(var item in containerInventory.charInventory)
                item.drawn = false;
            containerInventory.InventoryPanel.SetActive(false);
            allToContainer.SetActive(false);
            allToPlayer.SetActive(false);
        }
        panelForMoving.SetActive(false);
    }
    
    public void DrawInventory(Inventory objectInventory)
    {
        GameObject inventory = (objectInventory.GetInventoryType == Inventory.InventoryType.Hero) ? playerItemImages : containerItemImages;
        var charInventory = objectInventory.charInventory;

        inventory.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        inventory.GetComponent<GridLayoutGroup>().constraintCount = charInventory.GetLength(0);

        for (var i = 0; i < charInventory.Length; i++)
            Instantiate(prefabCell, inventory.transform);
    }

    public void FillItemImage(Inventory objectInventory)
    {
        var charInventory = objectInventory.charInventory;
        var widthCountCell = charInventory.GetLength(0);
        var heightCountCell = charInventory.GetLength(1);
        var PlayerInventoryRectTransform = playerItemImages.GetComponent<RectTransform>();
        var ContainerInventoryRectTransform = containerItemImages.GetComponent<RectTransform>();
        var MainRectTransform = mainPanel.GetComponent<RectTransform>();
        var offset = new Vector2(0, 0);
        if (objectInventory.GetInventoryType == Inventory.InventoryType.Hero)
            offset = PlayerInventoryRectTransform.anchoredPosition - new Vector2(PlayerInventoryRectTransform.sizeDelta.x / 2, 
                -PlayerInventoryRectTransform.sizeDelta.y / 2);
        else 
        {
            offset = ContainerInventoryRectTransform.anchoredPosition - new Vector2(ContainerInventoryRectTransform.sizeDelta.x / 2, 
                -ContainerInventoryRectTransform.sizeDelta.y / 2) 
                + new Vector2(MainRectTransform.sizeDelta.x / 2, 0)
                + new Vector2(MainRectTransform.anchoredPosition.x , 0);
        }

        for (var i = 0; i < widthCountCell; i++)
            for (var j = 0; j < heightCountCell; j++)
                if (charInventory[i, j].occupiedCell && !charInventory[i, j].drawn)
                {
                    for (var w = 0; w < charInventory[i, j].maxWidth; w++)
                        for (var h = 0; h < charInventory[i, j].maxHeight; h++)
                            if (charInventory[i, j].item.points.Contains(new Vector2Int(w, h)))
                            {
                                charInventory[i + w, j + h].indexOfMovingPanel = panelForMoving.transform.childCount;

                                var newImage = Instantiate(prefabItemImage, panelForMoving.transform) as GameObject;
                                newImage.GetComponent<Image>().sprite = charInventory[i + w, j + h].sprite;
                                var rt = newImage.GetComponent<RectTransform>();
                                rt.localScale = new Vector3(1, 1, 1);
                                var size = prefabCell.GetComponent<RectTransform>().sizeDelta;
                                rt.anchoredPosition = new Vector2(w * size.x + charInventory[i, j].firstPointCenterPosition.x, -h * size.y + charInventory[i, j].firstPointCenterPosition.y) + offset;
                                charInventory[i + w, j + h].drawn = true;
                                newImage.GetComponent<CellMoving>().Init(mainPanel, playerInventory, containerInventory, objectInventory.GetInventoryType, playerItemImages, containerItemImages, i + w, j + h);
                            }
                }
    }

    public void DeleteItemFromInventory(ItemInventory[,] charInventory, ItemInventory item)
    {
        for (var i = 0; i < item.maxWidth; i++)
            for (var j = 0; j < item.maxHeight; j++)
                if (item.item.points.Contains(new Vector2Int(i, j)))
                    charInventory[item.firstPoint.x + i, item.firstPoint.y + j] = new ItemInventory();
    }



    private void AllTo(Inventory containerIn, Inventory containerFrom)
    {
        var inventory = containerFrom.charInventory;
        for (var i = 0; i < inventory.GetLength(0); i++)
            for (var j = 0; j < inventory.GetLength(1); j++)
            {
                if (inventory[i, j].occupiedCell && AddItemToInventory(inventory[i, j].item, containerIn))
                    DeleteItemFromInventory(inventory, inventory[i, j]);
                inventory[i, j].drawn = false;
            }
        
        StartCoroutine(UpdateInventory(containerFrom));

        inventory = containerIn.charInventory;
        for (var i = 0; i < inventory.GetLength(0); i++)
            for (var j = 0; j < inventory.GetLength(1); j++)
                inventory[i, j].drawn = false;
        
        for (var i = 0; i < panelForMoving.transform.childCount; i++)
            Destroy(panelForMoving.transform.GetChild(i).gameObject, 0);
        
        StartCoroutine(UpdateInventory(containerIn));
    }

    public bool AddItemToInventory(Item item, Inventory objectInventory) 
    {
        var temp = item;
        var countSlotsRemainig = 0;
        for (var i = 0; i < objectInventory.GetWidthCountCell; i++)
            for (var j = 0; j < objectInventory.GetHeightCountCell; j++)   
                if (objectInventory.charInventory[i, j].occupiedCell != true)
                    countSlotsRemainig++;
        
        if (countSlotsRemainig < item.points.Count)
            return false;
        var firstPoint = new Vector2Int(-1, -1);
        var lastPoint = new Vector2Int(-1, -1);

        var neededCount = temp.points.Count;
        var maxWidth = 0;
        var maxHeight = 0;
        for (var i = 0; i < neededCount; i++)
        {
            if (temp.points[i].x + 1 > maxWidth)
                maxWidth = temp.points[i].x + 1;
            if (temp.points[i].y + 1 > maxHeight)
                maxHeight = temp.points[i].y + 1;
        }

        for (var i = 0; i < objectInventory.GetWidthCountCell; i++)
        {
            if (firstPoint.x != -1)
                break;
            for (var j = 0; j < objectInventory.GetHeightCountCell; j++)
            {
                if (firstPoint.x != -1)
                    break;
                if (objectInventory.charInventory[i, j].occupiedCell)
                    continue;

                var count = 0;
                if (firstPoint.x == -1)
                    for (var w = 0 ; w < maxWidth; w++)
                        for (var h = 0; h < maxHeight; h++)
                            if (item.points.Contains(new Vector2Int(w, h)) && i + w < objectInventory.charInventory.GetLength(0) &&
                                j + h < objectInventory.charInventory.GetLength(1) && !objectInventory.charInventory[i + w, j + h].occupiedCell)
                                count++;
                 
                if (count == neededCount)
                {
                    firstPoint = new Vector2Int(i, j); 
                    lastPoint = new Vector2Int(i + maxWidth - 1, j + maxHeight - 1);
                }
            }
        }
        
        if (firstPoint.x == -1)
            return false;

        var number = 0;
        GameObject inventory = (objectInventory.GetInventoryType == Inventory.InventoryType.Hero) ? playerItemImages : containerItemImages;
        for (var i = 0; i < maxWidth; i++)
            for (var j = 0; j < maxHeight; j++)
            {
                if (!item.points.Contains(new Vector2Int(i, j)))
                    continue;
                ItemInventory newItemInventory = new ItemInventory{
                    item = temp, 
                    occupiedCell = true, 
                    maxWidth = maxWidth, 
                    maxHeight = maxHeight, 
                    drawn = false, 
                    point = new Vector2Int(i, j), 
                    sprite = temp.sprites[number], 
                    firstPointCenterPosition = inventory.transform.GetChild(firstPoint.x + firstPoint.y 
                        * objectInventory.charInventory.GetLength(0)).GetComponent<RectTransform>().anchoredPosition, 
                    firstPoint = firstPoint, 
                    indexOfMovingPanel = objectInventory.charInventory[i + firstPoint.x, j + firstPoint.y].indexOfMovingPanel
                    };
                objectInventory.charInventory[i + firstPoint.x, j + firstPoint.y] = newItemInventory;
                number++;
            }

        return true; 
    }
}