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

#if UNITY_EDITOR
    public void SetType(Type type)
    {
        typeHeld = type;
    }    
#endif
    
}
