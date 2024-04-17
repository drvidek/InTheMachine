using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUnlock : Collectible
{
    [SerializeField] private Player.Ability ability;

    public Player.Ability Ability => ability;

#if UNITY_EDITOR
    public void SetType(Player.Ability type)
    {
        ability = type;
    }
#endif

}
