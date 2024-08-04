using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SetPlayProfile
{
    private static GameObject prefabPlayerNewGame => Resources.Load("PlayerNewGame") as GameObject;
    private static GameObject prefabPlayerFullUnlock => Resources.Load("PlayerFullUnlock") as GameObject;
    private static GameObject prefabPlayerGodMode => Resources.Load("PlayerGodMode") as GameObject;

    [MenuItem("Machine/Play Profiles/New Game",priority = 0)]
    public static void NewGame()
    {
        GameObject currentPlayer = Object.FindObjectOfType<Player>().gameObject;
        GameObject playerPrefab = PrefabUtility.InstantiatePrefab(prefabPlayerNewGame, null) as GameObject;
        playerPrefab.transform.position = GameObject.Find("StartPosition").transform.position;
        MonoBehaviour.DestroyImmediate(currentPlayer);
    }
    [MenuItem("Machine/Play Profiles/Full Unlock", priority = 1)]
    public static void FullUnlock()
    {
        GameObject currentPlayer = Object.FindObjectOfType<Player>().gameObject;
        GameObject playerPrefab = PrefabUtility.InstantiatePrefab(prefabPlayerFullUnlock, null) as GameObject;
        playerPrefab.transform.position = currentPlayer.transform.position;
        MonoBehaviour.DestroyImmediate(currentPlayer);
    }
    [MenuItem("Machine/Play Profiles/God Mode", priority = 2)]
    public static void GodMode()
    {
        GameObject currentPlayer = Object.FindObjectOfType<Player>().gameObject;
        GameObject playerPrefab = PrefabUtility.InstantiatePrefab(prefabPlayerGodMode, null) as GameObject;
        playerPrefab.transform.position = currentPlayer.transform.position;
        MonoBehaviour.DestroyImmediate(currentPlayer);
    }
}
