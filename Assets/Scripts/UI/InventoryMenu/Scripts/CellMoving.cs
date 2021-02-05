using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellMoving : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{   
    private GameObject mainPanel;
    private PlayerControlsInputSystem UIControls;
    private Inventory player;
    private Inventory container;
    private ItemInventory[,] charInventory;
    private GameObject startInventoryPanel;
    private Inventory startActor;
    private GameObject playerItemImages;
    private GameObject containerItemImages;
    private ItemInventory item;
    private Transform panelForMoving;
    private GameObject currentItemImages;
    private Inventory currentActor;
    private Vector3 startPosition;
    private Vector2 offset;
    private bool canMove;
    private Vector2Int firstPoint;
    private Inventory.InventoryType currentInventoryType;
    private Vector2Int resultFirstPoint;
    private RectTransform thisRectTransform;

    private void OnEnable() => UIControls.UI.Enable();

    private void OnDisable() => UIControls.UI.Disable();
    private void Awake() 
    {
        UIControls = new PlayerControlsInputSystem();
    }
    private void Start() 
    {
        thisRectTransform = GetComponent<RectTransform>();
    }
    
    public void Init(GameObject mainPanel, Inventory player, Inventory container, Inventory.InventoryType inventoryType, GameObject playerInventory, GameObject containerInventory, int i, int j) 
    {
        this.mainPanel = mainPanel;
        this.player = player;
        this.container = container;
        playerItemImages = playerInventory;
        containerItemImages = containerInventory;
        panelForMoving = transform.parent;

        if (inventoryType == Inventory.InventoryType.Hero)
        {
            charInventory = this.player.charInventory;
            startInventoryPanel = playerItemImages;
            startActor = this.player;
        }
        else
        {
            charInventory = this.container.charInventory;
            startInventoryPanel = containerItemImages;
            startActor = this.container;
        }

        item = charInventory[i, j];
        firstPoint = item.firstPoint;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        player.gameObject.GetComponent<Control>().PlayerControls.PlayerOther.Disable();
        startPosition = panelForMoving.GetChild(item.indexOfMovingPanel).position;

        for (var w = 0; w < item.maxWidth; w++)
            for (var h = 0; h < item.maxHeight; h++)
                if (item.item.points.Contains(new Vector2Int(w, h)))
                    charInventory[w + firstPoint.x, h + firstPoint.y].occupiedCell = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        canMove = true;
        
        var pos = new Vector3(UIControls.UI.Point.ReadValue<Vector2>().x,
            UIControls.UI.Point.ReadValue<Vector2>().y,
            thisRectTransform.position.z);
        var delta = pos - panelForMoving.GetChild(item.indexOfMovingPanel).position;
        for (var w = 0; w < item.maxWidth; w++)
            for (var h = 0; h < item.maxHeight; h++)
                if (item.item.points.Contains(new Vector2Int(w, h)))
                {
                    var index = startActor.charInventory[firstPoint.x + w, firstPoint.y + h].indexOfMovingPanel;
                    var cell = panelForMoving.GetChild(index);
                    cell.transform.position += delta;
                }

        var mainRectTrans = mainPanel.GetComponent<RectTransform>();
        if (!containerItemImages.activeInHierarchy || GetComponent<RectTransform>().anchoredPosition.x < mainRectTrans.sizeDelta.x / 2)
        {
            var PlayerInventoryRectTransform = playerItemImages.GetComponent<RectTransform>();
            offset = PlayerInventoryRectTransform.anchoredPosition - new Vector2(PlayerInventoryRectTransform.sizeDelta.x / 2, 
                -PlayerInventoryRectTransform.sizeDelta.y / 2);
            charInventory = player.charInventory;
            currentItemImages = playerItemImages;
            currentActor = player;
            currentInventoryType = Inventory.InventoryType.Hero;
        }
        else  
        {
            var contInvRectTrans = containerItemImages.GetComponent<RectTransform>();
            offset = contInvRectTrans.anchoredPosition - new Vector2(contInvRectTrans.sizeDelta.x / 2, 
                -contInvRectTrans.sizeDelta.y / 2) 
                + new Vector2(mainRectTrans.sizeDelta.x / 2, 0)
                + new Vector2(mainRectTrans.anchoredPosition.x , 0);
            charInventory = container.charInventory;
            currentItemImages = containerItemImages;
            currentActor = container;
            currentInventoryType = Inventory.InventoryType.Container;
        }

        var cellAnchoredPosition = thisRectTransform.anchoredPosition;
        
        var firstCell = currentItemImages.transform.GetChild(0).GetComponent<RectTransform>();
        var topLeftPosition = new Vector2(firstCell.anchoredPosition.x - firstCell.rect.width / 2 + offset.x,
            firstCell.anchoredPosition.y + firstCell.rect.height / 2 + offset.y);
        resultFirstPoint = new Vector2Int((int)((cellAnchoredPosition.x - thisRectTransform.rect.width * item.point.x - topLeftPosition.x) / firstCell.rect.width),
            -(int)((cellAnchoredPosition.y + thisRectTransform.rect.height * item.point.y - topLeftPosition.y) / firstCell.rect.height));

        var isBreak = true;
        for (var w = 0; w < item.maxWidth && isBreak; w++)
        {
            for (var h = 0; h < item.maxHeight && isBreak; h++)
                if (item.item.points.Contains(new Vector2Int(w, h)))
                {
                    var index = startActor.charInventory[firstPoint.x + w, firstPoint.y + h].indexOfMovingPanel;
                    var cell = panelForMoving.GetChild(index);
                    var cellRectTransform = cell.GetComponent<RectTransform>();
                    if (cellRectTransform.anchoredPosition.x < topLeftPosition.x)
                        resultFirstPoint = new Vector2Int(resultFirstPoint.x - 1, resultFirstPoint.y);
                    if (cellRectTransform.anchoredPosition.y > topLeftPosition.y)
                        resultFirstPoint = new Vector2Int(resultFirstPoint.x, resultFirstPoint.y - 1);
                    isBreak = false;
                }
        }

        if (resultFirstPoint.x < 0 || resultFirstPoint.x + item.maxWidth > charInventory.GetLength(0) ||
            resultFirstPoint.y < 0 || resultFirstPoint.y + item.maxHeight > charInventory.GetLength(1))
                canMove = false;

        if (canMove)
            for (var i = 0; i < item.maxWidth; i++)
                for (var j = 0; j < item.maxHeight; j++)
                    if (item.item.points.Contains(new Vector2Int(i, j)) && charInventory[resultFirstPoint.x + i, resultFirstPoint.y + j].occupiedCell)
                        canMove = false;

        ColorCells();
    }

    // Color(1, 1, 1, 1) - Свободные ячейки
    // Color(0.5f, 0.5f, 0.5f, 1) - Занятые ячейки или те ячейки на которые возможно перемещение
    // Color(0, 1, 0, 1) - Начальная ячейка предмета
    // Color(1, 0, 0, 1) - Ячейки на которые нельзя переместить
    private void ColorCells()
    {
        var inventory = player.charInventory;
        for (var i = 0; i < inventory.GetLength(0); i++)
            for (var j = 0; j < inventory.GetLength(1); j++)  
                playerItemImages.transform.GetChild(i + j * inventory.GetLength(0)).GetComponent<Image>().color = new Color(1, 1, 1, 1);

        if (containerItemImages.activeInHierarchy)
        {
            inventory = container.charInventory;
            for (var i = 0; i < inventory.GetLength(0); i++)
                for (var j = 0; j < inventory.GetLength(1); j++)   
                    containerItemImages.transform.GetChild(i + j * inventory.GetLength(0)).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        
        inventory = currentActor.charInventory;
        if (canMove)
        {
            for (var w = 0; w < item.maxWidth; w++)
                for (var h = 0; h < item.maxHeight; h++)
                    if (item.item.points.Contains(new Vector2Int(w, h)))
                        currentItemImages.transform.GetChild(resultFirstPoint.x + w + (resultFirstPoint.y + h) * inventory.GetLength(0))
                            .GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
        else
        {
           for (var w = 0; w < item.maxWidth; w++)
                for (var h = 0; h < item.maxHeight; h++)
                    if (item.item.points.Contains(new Vector2Int(w, h)) && 
                    resultFirstPoint.x + w >= 0 && resultFirstPoint.x + w < inventory.GetLength(0) &&
                    resultFirstPoint.y + h >= 0 && resultFirstPoint.y + h < inventory.GetLength(1))
                        currentItemImages.transform.GetChild(resultFirstPoint.x + w + (resultFirstPoint.y + h) * inventory.GetLength(0))
                            .GetComponent<Image>().color = new Color(1, 0, 0, 1);
        }

        inventory = player.charInventory;
        for (var i = 0; i < inventory.GetLength(0); i++)
            for (var j = 0; j < inventory.GetLength(1); j++)   
                if (inventory[i, j].occupiedCell)
                    playerItemImages.transform.GetChild(i + j * inventory.GetLength(0)).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1);

        if (containerItemImages.activeInHierarchy)
        {
            inventory = container.charInventory;
            for (var i = 0; i < inventory.GetLength(0); i++)
                for (var j = 0; j < inventory.GetLength(1); j++)   
                    if (inventory[i, j].occupiedCell)
                        containerItemImages.transform.GetChild(i + j * inventory.GetLength(0)).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1);
        }
        
        for (var w = 0; w < item.maxWidth; w++)
            for (var h = 0; h < item.maxHeight; h++)
                if (item.item.points.Contains(new Vector2Int(w, h)))
                    startInventoryPanel.transform.GetChild(firstPoint.x + w + (firstPoint.y + h) * startInventoryPanel.GetComponent<GridLayoutGroup>().constraintCount)
                        .GetComponent<Image>().color = new Color(0, 1, 0, 1);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canMove)
        {
            var firstPointCenterPosition = currentItemImages.transform.GetChild(resultFirstPoint.x + resultFirstPoint.y * charInventory.GetLength(0))
                .GetComponent<RectTransform>().anchoredPosition;
            var items = new List<ItemInventory>();

            for (var w = 0; w < item.maxWidth; w++)
                for (var h = 0; h < item.maxHeight; h++)
                    if (item.item.points.Contains(new Vector2Int(w, h)))
                        items.Add(CopyElement(startActor.charInventory[firstPoint.x + w, firstPoint.y + h]));
            
            var count = 0;
            var copyPoint = new Vector2Int(item.point.x, item.point.y);
            for (var w = 0; w < item.maxWidth; w++)
                for (var h = 0; h < item.maxHeight; h++)
                    if (item.item.points.Contains(new Vector2Int(w, h)))
                    {
                        var temp = items[count];

                        charInventory[resultFirstPoint.x + w, resultFirstPoint.y + h].item = temp.item;
                        charInventory[resultFirstPoint.x + w, resultFirstPoint.y + h].occupiedCell = true;
                        charInventory[resultFirstPoint.x + w, resultFirstPoint.y + h].maxWidth = temp.maxWidth;
                        charInventory[resultFirstPoint.x + w, resultFirstPoint.y + h].maxHeight = temp.maxHeight;
                        charInventory[resultFirstPoint.x + w, resultFirstPoint.y + h].drawn = true;
                        charInventory[resultFirstPoint.x + w, resultFirstPoint.y + h].point = temp.point;
                        charInventory[resultFirstPoint.x + w, resultFirstPoint.y + h].sprite = temp.sprite;
                        charInventory[resultFirstPoint.x + w, resultFirstPoint.y + h].firstPointCenterPosition = firstPointCenterPosition;
                        charInventory[resultFirstPoint.x + w, resultFirstPoint.y + h].firstPoint = new Vector2Int(resultFirstPoint.x, resultFirstPoint.y);
                        charInventory[resultFirstPoint.x + w, resultFirstPoint.y + h].indexOfMovingPanel = temp.indexOfMovingPanel;

                        var resultItem  = charInventory[resultFirstPoint.x + w, resultFirstPoint.y + h];
                        var imageRT = panelForMoving.transform.GetChild(resultItem.indexOfMovingPanel).gameObject.GetComponent<RectTransform>();
                        imageRT.anchoredPosition = new Vector2(w * 30 + firstPointCenterPosition.x, -h * 30 + firstPointCenterPosition.y) + offset;

                        panelForMoving.transform.GetChild(charInventory[resultFirstPoint.x + w, resultFirstPoint.y + h].indexOfMovingPanel).GetComponent<CellMoving>()
                            .Init(mainPanel, player, container, currentInventoryType, playerItemImages, containerItemImages, resultFirstPoint.x + w, resultFirstPoint.y + h);

                        count++;
                    }
        }
        else
        {
            charInventory = startActor.charInventory;
            var delta = startPosition - panelForMoving.GetChild(item.indexOfMovingPanel).position;
            for (var w = 0; w < item.maxWidth; w++)
                for (var h = 0; h < item.maxHeight; h++)
                    if (item.item.points.Contains(new Vector2Int(w, h)))
                    {
                        charInventory[firstPoint.x + w, firstPoint.y + h].occupiedCell = true;
                        var index = charInventory[firstPoint.x + w, firstPoint.y + h].indexOfMovingPanel;
                        var cell = panelForMoving.GetChild(index);
                        cell.transform.position += delta;
                    }
        }
        StartCoroutine(mainPanel.transform.parent.GetComponent<UIInventory>().UpdateInventory(player));
        if(!(container is null)) StartCoroutine(mainPanel.transform.parent.GetComponent<UIInventory>().UpdateInventory(container));
        player.gameObject.GetComponent<Control>().PlayerControls.PlayerOther.Enable();
    }

    private ItemInventory CopyElement(ItemInventory item)
    {
        var result = new ItemInventory();
        result.item = item.item;
        result.occupiedCell = item.occupiedCell;
        result.maxHeight = item.maxHeight;
        result.maxWidth = item.maxWidth;
        result.drawn = item.drawn;
        result.point = item.point;
        result.sprite = item.sprite;
        result.firstPoint = new Vector2Int(item.point.x, item.point.y);
        result.firstPointCenterPosition = new Vector2(item.firstPointCenterPosition.x, item.firstPointCenterPosition.y);
        result.indexOfMovingPanel = item.indexOfMovingPanel;
        return result;
    }
}
