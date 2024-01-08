using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EndScreenText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    private int totalPowerups, foundPowerups;

    // Start is called before the first frame update
    void Start()
    {
        totalPowerups = FindObjectsOfType<PowerUp>().Length;
        Player.main.onPowerUpObtained += (type) => foundPowerups++;
    }

    // Update is called once per frame
    void Update()
    {
        label.text = $"End of Demo! Thanks for playing :) <3\r\nHave this end-game ability!\r\nPower upgrades found: {foundPowerups}/{totalPowerups}";
    }
}
