using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : Collectible
{
    public enum Type
    {
        Power
    }

    [SerializeField] private Type typeHeld;

    public Type TypeHeld => typeHeld;

    public void SetType(Type type)
    {
#if UNITY_EDITOR
        typeHeld = type;
#endif
    }    
    
}
