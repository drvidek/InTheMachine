using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEditor;

[CustomEditor(typeof(RoomManager), true)]
[CanEditMultipleObjects]
public class RoomManagerEditor : Editor
{
    private void OnEnable()
    {
        //initialise fields

        //property = serializedObject.FindProperty("property");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        if (GUILayout.Button("Organise Scene Objects"))
        {
            OrganiseScene();
        }

        if (target)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void OrganiseScene()
    {
        RoomManager rm = target as RoomManager;

        for (int i = 0; i < rm.interactiblesGrid.childCount; i++)
        {
            Transform currentChild = rm.interactiblesGrid.GetChild(i);
            if (currentChild.tag == "Room")
                continue;

            Vector3Int roomCode = rm.GetRoom(currentChild);

            string roomName = $"Room {roomCode.x},{roomCode.y}";
            GameObject roomObject = GameObject.Find(roomName);
            if (!roomObject)
                roomObject = CreateNewRoomParent(roomName, roomCode);
            currentChild.parent = roomObject.transform;

            i--;
        }

    }

    public GameObject CreateNewRoomParent(string name, Vector3Int roomCode)
    {
        GameObject go = new();
        go.name = name;
        go.tag = "Room";
        go.transform.position = roomCode;
        go.transform.parent = (target as RoomManager).interactiblesGrid;

        return go;
    }

}


