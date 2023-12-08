using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] protected Grid roomGrid;
    [SerializeField] public Transform interactiblesGrid;

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

    public Grid RoomGrid => roomGrid;

    /// <summary>
    /// Returns the room coordinates of the player
    /// </summary>
    public Vector3Int currentRoom => GetRoom(Player.main.transform);

    /// <summary>
    /// Called when the player changes room, with the room coordinates
    /// </summary>
    public Action<Vector3Int> onPlayerMovedRoom;

    private Vector3Int lastRoom;

    /// <summary>
    /// Returns the current room coordinates of the provided transform.
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public Vector3Int GetRoom(Transform transform)
    {
        Vector3Int cell = roomGrid.WorldToCell(transform.position);
        return cell; //roomGrid.CellToWorld(cell);

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

        Debug.Log(currentRoom);
    }

    
}
