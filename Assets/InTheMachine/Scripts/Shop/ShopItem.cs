using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ShopItem
{
    public enum ID
    {
        RepairCharge,
        HealthUpgrade,
        Shield,
        AirUpgrade = 10,
        AirAltUpgrade,
        FireUpgrade,
        FireAltUpgrade,
        ElecUpgrade,
        ElecAltUpgrade,
        SlimeUpgrade,
        SlimeAltUpgrade,
        mapPip = 20,
        mapHint
    }
    public ID id;

    public int intId => (int)id;

    public string name;
    public Sprite image;
    public Color color;
    public int count;
    public float[] cost;
    public string description;

    private int currentCount;
    public int CurrentCount => currentCount;
    public float CurrentCost => currentCount > 0 ? cost[cost.Length - currentCount] : 0;

    public void Initialise()
    {
        currentCount = count;
    }

    public void TryToBuy()
    {
        if (currentCount == 0)
            return;

        if (!CashManager.main.TryToBuy(CurrentCost))
            return;

        currentCount--;

        switch (id)
        {
            case ID.RepairCharge:
                Player.main.IncreaseRepairCharge();
                break;
            case ID.HealthUpgrade:
                Player.main.HealthMeter.SetNewBounds(0, Player.main.HealthMeter.Max + 1);
                Player.main.HealthMeter.Fill();
                break;
            case ID.Shield:
                break;
            case ID.AirUpgrade:
                PlayerGun.main.UpgradeGun(GunProfileType.Air);
                break;
            case ID.AirAltUpgrade:
                break;
            case ID.FireUpgrade:
                PlayerGun.main.UpgradeGun(GunProfileType.Fire);
                break;
            case ID.FireAltUpgrade:
                break;
            case ID.ElecUpgrade:
                break;
            case ID.ElecAltUpgrade:
                break;
            case ID.SlimeUpgrade:
                break;
            case ID.SlimeAltUpgrade:
                break;
            case ID.mapPip:
                FogOfWar.AddTileToClear();
                break;
            case ID.mapHint:
                break;
            default:
                break;
        }

    }
}
