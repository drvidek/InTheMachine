using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shop : MonoBehaviour
{
    public static ShopItem[] inventory;
    [SerializeField] private GameObject defaultSelection;
    [SerializeField] private EventSystem eventSystem;

    private GameObject shop;

    public bool IsOpen => shop.activeInHierarchy;

    #region Singleton + Awake
    private static Shop _singleton;
    public static Shop main
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.LogWarning("Shop instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        main = this;
    }

    private void OnDisable()
    {
        if (main == this)
            _singleton = null;
    }
    #endregion


    private void Start()
    {
        shop = transform.GetChild(0).gameObject;
        inventory = (Resources.Load("ShopInventory") as ShopItemList).inventory;
    }

    public void OpenShop()
    {
        shop.SetActive(true);
        eventSystem.SetSelectedGameObject(defaultSelection);
    }

    public void CloseShop()
    {
        shop.SetActive(false);

    }

    public void SetDefaultSelection(GameObject defaultSelection)
    {
        this.defaultSelection = defaultSelection;
    }
}
