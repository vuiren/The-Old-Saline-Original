using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity
{
    Common,
    Rare,
    ExtremelyRare,
    Unique,
    Unknown,
    Forbidden
}
public enum Category
{
    RawMaterials,
    Containers,
    AlchemicalCompounds,
    Tools,
    Consumable,
    QuestItems,
    Special
}

[System.Serializable, CreateAssetMenu(fileName = "New Item Data", menuName = "Item Data", order = 51)]
public class Item : ScriptableObject
{
    public List<Vector2Int> points;
    public List<Sprite> sprites;
    public string itemName;
    public Rarity itemRarity;
    public int barterPrice;
    public Category itemCategory;
    public int rawMaterials_Condition;
    public int consumable_NumberOfUses;

    
}