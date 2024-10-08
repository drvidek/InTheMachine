using System;
using System.Collections.Generic;
using UnityEngine;
using QKit;
using System.Net.Sockets;

public class RoomManager : MonoBehaviour
{
    [SerializeField] protected Grid roomGrid;
    [SerializeField] public Grid environmentGrid;
    [SerializeField] public Grid interactiblesGrid;

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

    private static int maxRoomDistance = 3;

    private static List<Vector3Int> secretRooms = new();

    private Vector3Int lastRoom = new();

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

    /// <summary>
    /// Return the current room coordinates of the provided world position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector3Int GetRoom(Vector3 position)
    {
        Vector3Int cell = roomGrid.WorldToCell(position);
        return cell; //roomGrid.CellToWorld(cell);
    }

    public Vector3 WorldToCellCentre(Vector3 position)
    {
        Vector3Int cell = environmentGrid.WorldToCell(position);
        return environmentGrid.GetCellCenterWorld(cell);
    }

    public Vector3 WorldToCellHalfCentre(Vector3 position)
    {
        Vector3Int cell = interactiblesGrid.WorldToCell(position);
        return interactiblesGrid.GetCellCenterWorld(cell);
    }

    public bool InSameRoom(params Transform[] transforms)
    {
        Vector3Int room = GetRoom(transforms[0]);
        foreach (var item in transforms)
        {
            Vector3Int itemRoom = GetRoom(item);
            if (!IsSameRoom(room, itemRoom))
                return false;
        }
        return true;
    }

    public bool InSameRoom(params Vector3[] positions)
    {
        Vector3Int room = GetRoom(positions[0]);
        foreach (var item in positions)
        {
            Vector3Int itemRoom = GetRoom(item);
            if (!IsSameRoom(room, itemRoom))
                return false;
        }
        return true;
    }
    
    public bool PlayerWithinRoomDistance(Transform transform)
    {
        Vector3Int room = GetRoom(transform);

        if (QMath.Difference(room.x, currentRoom.x) > maxRoomDistance || QMath.Difference(room.y, currentRoom.y) > maxRoomDistance)
            return false;

        return true;
    }

    public bool PlayerWithinRoomDistance(Transform transform, int distance)
    {
        Vector3Int room = GetRoom(transform);

        if (QMath.Difference(room.x, currentRoom.x) > distance || QMath.Difference(room.y, currentRoom.y) > distance)
            return false;

        return true;
    }

    public static bool IsRoomSecret(Vector3Int room)
    {
        foreach (var secret in secretRooms)
        {
            if (IsSameRoom(room, secret))
                return true;
        }
        return false;
    }

    public static void AddSecretRoom(Vector3Int room)
    {
        if (!secretRooms.Contains(room))
        secretRooms.Add(room);
    }

    private void Start()
    {
        lastRoom = GetRoom(Player.main.transform);
        onPlayerMovedRoom?.Invoke(currentRoom);
    }

    private void FixedUpdate()
    {
        if (currentRoom != lastRoom)
            onPlayerMovedRoom?.Invoke(currentRoom);

        lastRoom = currentRoom;

    }

    private static bool IsSameRoom(Vector3Int roomA, Vector3Int roomB)
    {
        return (roomA.x == roomB.x && roomA.y == roomB.y);
    }
}
