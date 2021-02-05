using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory
{
    public Item item;
    public bool occupiedCell;
    public int maxWidth;
    public int maxHeight;
    public bool drawn;
    public Vector2Int point;
    public Sprite sprite;
    public Vector2 firstPointCenterPosition;
    public Vector2Int firstPoint;
    public int indexOfMovingPanel;
}