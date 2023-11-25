using QKit;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum GunProfileType
{
    Air,
    Fire,
    Elec,
    Slime
}

[CreateAssetMenu(fileName = "GunProfile", menuName = "Scriptable Objects/Gun Profile")]
public class GunProfile : ScriptableObject
{
    public float size, speed, power, lifetime;
    public int count;
    public float spread;
    public LayerMask collidingLayer;
    public LayerMask piercingLayer;
    public Projectile projectilePrefab;
    public float cost;
    public bool costOnShot;
}
