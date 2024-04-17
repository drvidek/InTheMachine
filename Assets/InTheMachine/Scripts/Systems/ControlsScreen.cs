using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsScreen : MonoBehaviour
{
    [SerializeField] private GameObject powerPakGroup, flightPair, tractorPair, specialPair, dodgePair, ultraboostPair, swapPair, repairPair;

    private void Start()
    {
        Player.main.onAbilityUnlock += EnableAbilityPair;
        PlayerGun.main.onProfileUnlock += EnableGunPair;
    }

    private void Update()
    {
        repairPair.SetActive(Player.main.RepairMax > 0);
    }

    private void EnableAbilityPair(Player.Ability ability)
    {
        switch (ability)
        {
            case Player.Ability.Gun:
                powerPakGroup.SetActive(true);
                break;
            case Player.Ability.Flight:
                flightPair.SetActive(true);
                break;
            case Player.Ability.Tractor:
                tractorPair.SetActive(true);
                break;
            case Player.Ability.Special:
                specialPair.SetActive(true);
                break;
            case Player.Ability.Boost:
                dodgePair.SetActive(true);
                break;
            case Player.Ability.UltraBoost:
                ultraboostPair.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void EnableGunPair(GunProfileType type)
    {
        if (type != GunProfileType.Air)
            swapPair.SetActive(true);
    }

}
