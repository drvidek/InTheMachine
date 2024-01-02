using QKit;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum GunProfileType
{
    Air,
    Fire,
    Elec,
    Goo
}

[CreateAssetMenu(fileName = "GunProfile", menuName = "Scriptable Objects/Gun Profile")]
public class GunProfile : ScriptableObject
{
    public float size, speed, power, lifetime;
    public int count;
    public float spread;
    public LayerMask collidingLayer;
    public LayerMask piercingLayer;
    public LayerMask pinpointLayer;
    public Projectile projectilePrefab;
    public float cost;
    public bool costOnShot;
}
