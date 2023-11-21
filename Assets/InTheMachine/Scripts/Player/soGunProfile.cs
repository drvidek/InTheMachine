using QKit;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum GunProfile
{
    Air,
    Fire,
    Elec,
    Slime
}

[CreateAssetMenu(fileName = "Gun Profile", menuName = "ScriptableObjects")]
public class soGunProfile : ScriptableObject
{
    public float size, speed, power, lifetime;
    public int count;
    public float spread;
    public LayerMask collidingLayer;
    public LayerMask piercingLayer;
    public Projectile projectilePrefab;
    public float cost;
}
