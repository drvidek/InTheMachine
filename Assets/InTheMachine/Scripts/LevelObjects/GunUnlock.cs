using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunUnlock : Collectible
{
    [SerializeField] private GunProfileType gun;

    public GunProfileType Gun => gun;

#if UNITY_EDITOR
    public void SetType(GunProfileType type)
    {
        gun = type;
    }
#endif

}
