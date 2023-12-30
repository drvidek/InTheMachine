using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
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
    public float cost;
    public string description;
    public int count;
    public float costIncrease;

    public void TryToBuy()
    {
        if (!CashManager.main.TryToBuy(cost))
            return;

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
                break;
            case ID.AirAltUpgrade:
                break;
            case ID.FireUpgrade:
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
