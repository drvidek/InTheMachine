using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Player))]
public class PlayerUI : MonoBehaviour
{
    private Player player;
    private PlayerGun playerGun;
    [SerializeField] private float powerPelletWorth = 2f;
    [SerializeField] private TextMeshProUGUI cashDisplay;
    [SerializeField] private TextMeshProUGUI navMemDisplay;
    [SerializeField] private Transform powerPelletContainerA;
    [SerializeField] private Transform powerPelletContainerB;
    [SerializeField] private Transform healthContainer;
    [SerializeField] private Transform repairContainer;

    [SerializeField] private GameObject[] gunIcons;

    private Image[] powerPelletsA;
    private Image[] powerPelletsB;
    private Image[] healthPellets;
    private Image[] repairPellets;

    private void Start()
    {
        player = GetComponent<Player>();
        playerGun = GetComponent<PlayerGun>();

        ScrapeContainer(powerPelletContainerA, ref powerPelletsA);
        ScrapeContainer(powerPelletContainerB, ref powerPelletsB);
        ScrapeContainer(healthContainer, ref healthPellets);
        ScrapeContainer(repairContainer, ref repairPellets);

        player.PowerMeter.onMax += () => { SetPowerColor(player.PowerMeter.CurrentColor); };
        player.PowerMeter.onMin += () => { SetPowerColor(player.PowerMeter.BackgroundColor); };
        playerGun.onProfileChange += SetActiveGunIcon;
        playerGun.onProfileUnlock += EnableGunProfileIcon;

        CashManager.main.onCashChange += UpdateCashDisplay;
        UpdateCashDisplay(0);
    }

    private void ScrapeContainer(Transform container, ref Image[] array)
    {
        int count = container.childCount;
        array = new Image[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = container.GetChild(i).GetChild(0).GetComponent<Image>();
        }
    }

    private void Update()
    {
        powerPelletContainerB.gameObject.SetActive(player.PowerMeter.Max > powerPelletWorth * powerPelletsA.Length);

        UpdateDisplay(powerPelletsA, Player.main.PowerMeter.Max / powerPelletWorth, Player.main.PowerMeter.Value / powerPelletWorth);
        float newMax = Player.main.PowerMeter.Max - (powerPelletWorth * powerPelletsA.Length);
        float newValue = Player.main.PowerMeter.Value - (powerPelletWorth * powerPelletsA.Length);
        UpdateDisplay(powerPelletsB, newMax / powerPelletWorth, newValue / powerPelletWorth);
        UpdateDisplay(healthPellets, Player.main.MaxHealth, Player.main.CurrentHealth);
        UpdateDisplay(repairPellets, Player.main.RepairMax, Player.main.RepairCurrent);
        navMemDisplay.text = $"{FogOfWar.TilesToClear}x NAV MEMORY";
    }

    private void UpdateDisplay(Image[] collection, float activeValue, float filledValue)
    {
        for (int i = collection.Length - 1; i >= 0; i--)
        {
            collection[i].transform.parent.gameObject.SetActive(activeValue > i);
            collection[i].enabled = filledValue > i;
        }
    }

    private void SetActiveGunIcon(GunProfileType type)
    {
        for (int i = 0; i < gunIcons.Length; i++)
        {
            gunIcons[i].transform.GetChild(0).gameObject.SetActive(i == (int)type);
        }
    }

    private void EnableGunProfileIcon(GunProfileType type)
    {
        gunIcons[(int)type].gameObject.SetActive(true);
    }

    private void SetPowerColor(Color color)
    {
        foreach (var pellet in powerPelletsA)
        {
            pellet.color = color;
        }

        foreach (var pellet in powerPelletsB)
        {
            pellet.color = color;
        }
    }

    private void UpdateCashDisplay(int cash)
    {
        cashDisplay.text = $"{cash}c";
    }
}
