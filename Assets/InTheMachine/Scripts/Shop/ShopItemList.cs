using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Shop Inventory",menuName = "ScriptableObjects/ShopItemList")]
public class ShopItemList : ScriptableObject
{
    public ShopItem[] inventory;
}
