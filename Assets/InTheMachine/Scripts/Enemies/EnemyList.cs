using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyList", menuName = "ScriptableObjects/EnemyList")]
public class EnemyList : ScriptableObject
{
    public enum Type
    {
        Beetle,
        Fly,
        Buzzfly
    }

    public static int Length => Enum.GetNames(typeof(Type)).Length;

    public GameObject[] enemyPrefabs;

}