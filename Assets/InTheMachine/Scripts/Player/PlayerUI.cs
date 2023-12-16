using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Player))]
public class PlayerUI : MonoBehaviour
{
    private Player player;
    private PlayerGun playerGun;
    [SerializeField] private TextMeshProUGUI cashDisplay;
    [SerializeField] private Transform powerPelletContainer;

    [SerializeField] private GameObject[] gunIcons;

    private Image[] powerPellets;

    private void Start()
    {
        player = GetComponent<Player>();
        playerGun = GetComponent<PlayerGun>();
        powerPellets = new Image[powerPelletContainer.childCount];
        for (int i = 0; i < powerPelletContainer.childCount; i++)
        {
            powerPellets[i] = powerPelletContainer.GetChild(i).GetChild(0).GetComponent<Image>();
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
        for (int i = powerPellets.Length-1; i >= 0; i--)
        {
            powerPellets[i].transform.parent.gameObject.SetActive(Player.main.PowerMeter.Max / 5f > i);
            powerPellets[i].enabled = Player.main.PowerMeter.Value / 5f > i;
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
        foreach (var pellet in powerPellets)
        {
            pellet.color = color;
        }
    }

    private void UpdateCashDisplay(int cash)
    {
        cashDisplay.text = $"{cash}c";
    }
}
