using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
public class PlayerUI : MonoBehaviour
{
    private Player player;
    [SerializeField] private Image powerMeter;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        powerMeter.fillAmount = player.PowerMeter.Percent;
    }

}
