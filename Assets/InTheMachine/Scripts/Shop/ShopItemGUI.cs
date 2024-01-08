using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemGUI : MonoBehaviour
{
    [SerializeField] private ShopItem.ID id;
    [SerializeField] private TextMeshProUGUI nameLabel, costLabel, descriptionLabel;
    [SerializeField] private Image image;
    [SerializeField] private Button button;

    public int intId => (int)id;

    public ShopItem Item
    {
        get
        {
#if UNITY_EDITOR
            foreach (ShopItem item in (Resources.Load("ShopInventory") as ShopItemList).inventory)
            {
                if (item.id == id)
                    return item;
            }
            Debug.LogError("Invalid item ID! Returning default entry");
            return (Resources.Load("ShopInventory") as ShopItemList).inventory[0];
#endif
            foreach (ShopItem item in Shop.inventory)
            {
                if (item.id == id)
                    return item;
            }
            Debug.LogError("Invalid item ID! Returning default entry");
            return Shop.inventory[0];
        }
    }

    private void OnValidate()
    {
        ApplyProperties();
    }

    void Start()
    {
        ApplyProperties();
    }

    private void ApplyProperties()
    {
        name = Item.name + " Parent";
        nameLabel.text = Item.name;
        costLabel.text = $"{Item.cost}c";
        string rawDescription = Item.description;
        string formattedDescription = rawDescription.Replace("|", "\n");
        descriptionLabel.text = formattedDescription;
        image.sprite = Item.image;
        image.color = Item.color;
        button = GetComponent<Button>();
    }

    private void Update()
    {
        button.interactable = true;
        if (CashManager.main.Cash < Item.cost)
        {
            if (Shop.main.ESys.currentSelectedGameObject == gameObject)
                Shop.main.ESys.SetSelectedGameObject(Shop.main.DefaultSelection);
            button.interactable = false;
        }
    }

    public void TryToBuy()
    {
        Item.TryToBuy();
    }
}
