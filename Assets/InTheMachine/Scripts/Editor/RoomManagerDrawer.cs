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
        if (GUILayout.Button("Move Player to Camera"))
        {
            MovePlayerToCurrentRoom();
        }

        if (target)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void OnSceneGUI()
    {
        Handles.BeginGUI();

        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

        Vector3Int position = (target as RoomManager).GetRoom(ray.origin);

        GUILayout.Label($"Room: {position.x},{position.y}", GUILayout.Width(100f));
        Handles.EndGUI();
    }

    private void OrganiseScene()
    {
        RoomManager rm = target as RoomManager;

        for (int i = 0; i < rm.interactiblesGrid.transform.childCount; i++)
        {
            Transform currentChild = rm.interactiblesGrid.transform.GetChild(i);
            if (currentChild.tag == "Room")
            {
                for (int ii = 0; ii < currentChild.childCount; ii++)
                {
                    Transform roomChild = currentChild.GetChild(ii);
                    roomChild.parent = rm.interactiblesGrid.transform;
                    ii--;
                }
                continue;
            }

            MoveChildToRoom(rm, currentChild);

            i--;
        }

    }

    private void MoveChildToRoom(RoomManager rm, Transform currentChild)
    {
        Vector3Int roomCode = rm.GetRoom(currentChild);

        string roomName = $"Room {roomCode.x},{roomCode.y}";
        GameObject roomObject = GameObject.Find(roomName);
        if (!roomObject)
            roomObject = CreateNewRoomParent(roomName, roomCode);
        currentChild.parent = roomObject.transform;
    }

    public GameObject CreateNewRoomParent(string name, Vector3Int roomCode)
    {
        RoomManager rm = (target as RoomManager);
        GameObject go = new();
        go.name = name;
        go.tag = "Room";
        go.transform.position = rm.RoomGrid.CellToWorld(roomCode);
        go.transform.parent = rm.interactiblesGrid.transform;

        return go;
    }


    public void MovePlayerToCurrentRoom()
    {
        Transform cam = SceneView.lastActiveSceneView.camera.transform;
        Player p = FindObjectOfType<Player>();
        p.transform.position = new(cam.position.x, cam.position.y, p.transform.position.z);
    }
}


