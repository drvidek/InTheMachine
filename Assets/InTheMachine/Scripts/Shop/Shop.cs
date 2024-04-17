using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shop : MonoBehaviour
{
    private static ShopItem[] _inventory;
    public static ShopItem[] inventory
    {
        get
        {
            if (_inventory == null)
            {
               _inventory = (Resources.Load("ShopInventory") as ShopItemList).inventory;
            }
            return _inventory;
        }
    }

    [SerializeField] private GameObject defaultSelection;
    [SerializeField] private GameObject weaponGroup, specialGroup;
    [SerializeField] private GameObject[] airItems, fireItems, elecItems, gooItems;
    [SerializeField] private EventSystem eventSystem;

    private GameObject shop;

    public GameObject DefaultSelection => defaultSelection;
    public EventSystem ESys => eventSystem;
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
        shop.SetActive(false);
        weaponGroup.SetActive(false);
        specialGroup.SetActive(false);
        Player.main.onAbilityUnlock += CheckShopUnlock;
        PlayerGun.main.onProfileUnlock += CheckShopUnlock;
    }

    private void CheckShopUnlock(Player.Ability ability)
    {
        switch (ability)
        {
            case Player.Ability.Gun:
                weaponGroup.SetActive(true);
                break;
            case Player.Ability.Special:
                specialGroup.SetActive(true);
                break;
            case Player.Ability.Flight:
            case Player.Ability.Tractor:
            case Player.Ability.Boost:
            case Player.Ability.UltraBoost:
            default:
                break;
        }
    }

    private void CheckShopUnlock(GunProfileType gunType)
    {
        switch (gunType)
        {
            case GunProfileType.Air:
                foreach (var item in airItems)
                {
                    item.SetActive(true);
                }
                break;
            case GunProfileType.Fire:
                foreach (var item in fireItems)
                {
                    item.SetActive(true);
                }
                break;
            case GunProfileType.Elec:
                foreach (var item in elecItems)
                {
                    item.SetActive(true);
                }
                break;
            case GunProfileType.Goo:
                foreach (var item in gooItems)
                {
                    item.SetActive(true);
                }
                break;
            default:
                break;
        }
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
