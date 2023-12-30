using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunUnlock : Collectible
{
    [SerializeField] private GunProfileType gun;

    public GunProfileType Gun => gun;

    public void SetType(GunProfileType type)
    {
#if UNITY_EDITOR
        gun = type;
#endif
    }

}
