using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType
{
    Actor,
    Door,
    Button,
    Physics,
    Debris,
    Air,
    Fire,
    Elec,
    Goo,
    Liquid,
    BurnIt,
    ZapIt

}

[CreateAssetMenu(fileName = "NewPalette", menuName = "Scriptable Objects/Color Palette")]
public class ColorPalette : ScriptableObject
{
    public Color[] color;

    public static int Length => Enum.GetNames(typeof(ColorType)).Length;

}

