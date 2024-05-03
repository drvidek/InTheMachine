using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    private Player player;
    private PlayerGun playerGun;
    private PlayerSpecialGun playerSpecialGun;
    [SerializeField] private float powerPelletWorth = 2f;
    [SerializeField] private TextMeshProUGUI cashDisplay;
    [SerializeField] private TextMeshProUGUI navMemDisplay;
    [SerializeField] private Transform powerPelletContainerA;
    [SerializeField] private Transform powerPelletContainerB;
    [SerializeField] private Transform healthContainer;
    [SerializeField] private Transform repairContainer;
    [SerializeField] private Transform specialChargeContainer;
    [SerializeField] private Transform specialCooldownContainer;
    [SerializeField] private GameObject RepairLabel;
    [SerializeField] private GameObject GunLabel;
    [SerializeField] private GameObject SpecialLabel;

    [SerializeField] private GameObject[] gunIcons;

    private Image[] powerPelletsA;
    private Image[] powerPelletsB;
    private Image[] healthPellets;
    private Image[] repairPellets;
    private Image[] chargePellets;
    private Image[] cooldownPellets;


    private void Start()
    {
        FindPlayer();

        ScrapeContainer(powerPelletContainerA, ref powerPelletsA);
        ScrapeContainer(powerPelletContainerB, ref powerPelletsB);
        ScrapeContainer(healthContainer, ref healthPellets);
        ScrapeContainer(repairContainer, ref repairPellets);
        ScrapeContainer(specialChargeContainer, ref chargePellets);
        ScrapeContainer(specialCooldownContainer, ref cooldownPellets);

        player.PowerMeter.onMax += () => { SetPowerColor(player.PowerMeter.CurrentColor); };
        player.PowerMeter.onMin += () => { SetPowerColor(player.PowerMeter.BackgroundColor); };
        playerGun.onProfileChange += SetActiveGunIcon;
        playerGun.onProfileUnlock += EnableGunProfileIcon;


        CashManager.main.onCashChange += UpdateCashDisplay;
        UpdateCashDisplay(0);
    }

    private void FindPlayer()
    {
        player = FindObjectOfType<Player>();
        playerGun = player.GetComponent<PlayerGun>();
        playerSpecialGun = player.GetComponent<PlayerSpecialGun>();
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
        GunLabel.SetActive(Player.main.HasAbility(Player.Ability.Gun));
        SpecialLabel.SetActive(Player.main.HasAbility(Player.Ability.Special));
        specialChargeContainer.gameObject.SetActive(Player.main.HasAbility(Player.Ability.Special));
        specialCooldownContainer.gameObject.SetActive(Player.main.HasAbility(Player.Ability.Special));

        RepairLabel.SetActive(player.RepairMax > 0);
        repairContainer.gameObject.SetActive(player.RepairMax > 0);

        UpdateDisplay(powerPelletsA, Player.main.PowerMeter.Max / powerPelletWorth, Player.main.PowerMeter.Value / powerPelletWorth);
        float newMax = Player.main.PowerMeter.Max - (powerPelletWorth * powerPelletsA.Length);
        float newValue = Player.main.PowerMeter.Value - (powerPelletWorth * powerPelletsA.Length);
        UpdateDisplay(powerPelletsB, newMax / powerPelletWorth, newValue / powerPelletWorth);
        UpdateDisplay(healthPellets, Player.main.MaxHealth, Player.main.CurrentHealth);
        UpdateDisplay(repairPellets, Player.main.RepairMax, Player.main.RepairCurrent);
        UpdateDisplay(chargePellets, playerSpecialGun.ChargesMax, playerSpecialGun.ChargesAvailable);
        UpdateDisplay(cooldownPellets, 1f*cooldownPellets.Length, playerSpecialGun.CooldownPercent * cooldownPellets.Length);
        navMemDisplay.text = $"{FogOfWar.TilesToClear}x NAV CHIPS";
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
