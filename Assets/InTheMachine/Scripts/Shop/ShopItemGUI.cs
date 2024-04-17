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
        Item.Initialise();
        ApplyProperties();
    }

    private void ApplyProperties()
    {
        name = Item.name + " Parent";
        nameLabel.text = Item.name;
        costLabel.text = $"{Item.CurrentCost}c";
        string rawDescription = Item.description;
        string formattedDescription = rawDescription.Replace("|", "\n");
        descriptionLabel.text = formattedDescription;
        image.sprite = Item.image;
        image.color = Item.color;
        button = GetComponent<Button>();
    }

    private void Update()
    {
        if (Item.CurrentCount == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        button.interactable = true;
        if (CashManager.main.Cash < Item.CurrentCost)
        {
            if (Shop.main.ESys.currentSelectedGameObject == gameObject)
                Shop.main.ESys.SetSelectedGameObject(Shop.main.DefaultSelection);
            button.interactable = false;
        }
    }

    public void TryToBuy()
    {
        Item.TryToBuy();
        ApplyProperties();
    }
}
