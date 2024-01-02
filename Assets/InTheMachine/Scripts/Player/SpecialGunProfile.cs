using QKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialGunProfile", menuName = "Scriptable Objects/SpecialGun Profile")]
public class SpecialGunProfile : ScriptableObject
{
    public float size, speed, power, lifetime;
    public int count;
    public float spread;
    public LayerMask collidingLayer;
    public LayerMask piercingLayer;
    public LayerMask pinpointLayer;
    public Projectile projectilePrefab;
}