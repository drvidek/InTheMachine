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
    [SerializeField] private Transform powerPelletContainerA;
    [SerializeField] private Transform powerPelletContainerB;
    [SerializeField] private Transform healthContainer;

    [SerializeField] private GameObject[] gunIcons;

    private Image[] powerPelletsA;
    private Image[] powerPelletsB;
    private Image[] healthPellets;

    private void Start()
    {
        player = GetComponent<Player>();
        playerGun = GetComponent<PlayerGun>();

        powerPelletsA = new Image[powerPelletContainerA.childCount];
        for (int i = 0; i < powerPelletContainerA.childCount; i++)
        {
            powerPelletsA[i] = powerPelletContainerA.GetChild(i).GetChild(0).GetComponent<Image>();
        }
        powerPelletsB = new Image[powerPelletContainerB.childCount];
        for (int i = 0; i < powerPelletContainerB.childCount; i++)
        {
            powerPelletsB[i] = powerPelletContainerB.GetChild(i).GetChild(0).GetComponent<Image>();
        }

        healthPellets = new Image[healthContainer.childCount];
        for (int i = 0; i < healthContainer.childCount; i++)
        {
            healthPellets[i] = healthContainer.GetChild(i).GetChild(0).GetComponent<Image>();
        }

        player.PowerMeter.onMax += () => { SetPowerColor(player.PowerMeter.CurrentColor); };
        player.PowerMeter.onMin += () => { SetPowerColor(player.PowerMeter.BackgroundColor); };
        playerGun.onProfileChange += SetActiveGunIcon;
        playerGun.onProfileUnlock += EnableGunProfileIcon;

        CashManager.main.onCashChange += UpdateCashDisplay;
        UpdateCashDisplay(0);
    }

    private void Update()
    {
        powerPelletContainerB.gameObject.SetActive(player.PowerMeter.Max > powerPelletWorth * powerPelletsA.Length);
        for (int i = powerPelletsA.Length - 1; i >= 0; i--)
        {
            powerPelletsA[i].transform.parent.gameObject.SetActive(Player.main.PowerMeter.Max / powerPelletWorth > i);
            powerPelletsA[i].enabled = Player.main.PowerMeter.Value / powerPelletWorth > i;
        }

        float newMax = Player.main.PowerMeter.Max - (powerPelletWorth * powerPelletsA.Length);
        float newValue = Player.main.PowerMeter.Value - (powerPelletWorth * powerPelletsA.Length);

        for (int i = powerPelletsB.Length - 1; i >= 0; i--)
        {
            powerPelletsB[i].transform.parent.gameObject.SetActive(newMax / powerPelletWorth > i);
            powerPelletsB[i].enabled = newValue / powerPelletWorth > i;
        }

        for (int i = healthPellets.Length - 1; i >= 0; i--)
        {
            healthPellets[i].transform.parent.gameObject.SetActive(Player.main.MaxHealth > i);
            healthPellets[i].enabled = Player.main.CurrentHealth > i;
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
