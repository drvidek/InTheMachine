using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType
{
    Actor,
    World,
    Button,
    ButtonLoose,
    Physics,
    Debris,
    Air,
    Fire,
    Elec,
    Slime,
    Liquid,
    BurnIt,
    ZapIt,

    _max
}

[CreateAssetMenu(fileName = "NewPalette", menuName = "Scriptable Objects/Color Palette")]
public class ColorPalette : ScriptableObject
{
    public Color[] color;

}
