using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUnlock : Collectible
{
    [SerializeField] private Player.Ability ability;

    public Player.Ability Ability => ability;

    public void SetType(Player.Ability type)
    {
#if UNITY_EDITOR
        ability = type;
#endif
    }

}
