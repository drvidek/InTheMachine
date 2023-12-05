using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] protected Grid roomGrid;
    #region Singleton + Awake
    private static RoomManager _singleton;
    public static RoomManager main
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.LogWarning("RoomGridManager instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        main = this;
    }
    #endregion

    public Vector2 currentRoom => GetRoom(CameraController.main.transform);

    public Action<Vector2> onPlayerMovedRoom;

    private Vector2 lastRoom;

    /// <summary>
    /// Returns the current room coordinates of the provided transform.
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public Vector2 GetRoom(Transform transform)
    {
        Vector3Int cell = roomGrid.WorldToCell(transform.position);
        return roomGrid.CellToWorld(cell);

    }

    private void Start()
    {
        lastRoom = GetRoom(Player.main.transform);
    }

    private void FixedUpdate()
    {
        if (currentRoom != lastRoom)
            onPlayerMovedRoom?.Invoke(currentRoom);

        lastRoom = currentRoom;
    }
}
