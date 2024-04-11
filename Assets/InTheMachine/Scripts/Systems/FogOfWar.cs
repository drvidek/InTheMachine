using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    [SerializeField] private bool chargePoint;

    private Tilemap fogMap;

    private static int tilesToClear;

    public static Action onMapReveal;

    public static int TilesToClear => tilesToClear;

    private bool clearingArea = false;

    // Start is called before the first frame update
    void Start()
    {
        fogMap = GetComponent<Tilemap>();
        RoomManager.main.onPlayerMovedRoom += ClearFog;
        Checkpoint.onActivate += ClearArea;
    }

    private void ClearFog(Vector3Int room)
    {
        Debug.Log($"Trying to clear {room}");
        if (!clearingArea && (tilesToClear <= 0 && chargePoint))
        {
            Debug.LogAssertion($"Failed to clear: Area?{clearingArea} Pips?{tilesToClear} Cost?{chargePoint}");
            return;
        }
        
        Vector3 roomPos = RoomManager.main.RoomGrid.CellToWorld(room);
        Vector3Int fogTile = fogMap.WorldToCell(roomPos);
        fogTile.z = 0;
        
        if (fogMap.GetTile(fogTile) == null)
        {
            Debug.LogAssertion($"Did not find tile on {fogMap} at {roomPos}");
        
            return;
        }
        
        fogMap.SetTile(fogTile, null);
        
        if (!clearingArea)
        {
            fogMap.RefreshAllTiles();
            if (chargePoint)
            {
                onMapReveal?.Invoke();
                tilesToClear--;
            }
        }
        Debug.LogAssertion($"Cleared tile");
    }

    private void ClearArea(Vector3Int room)
    {
        Debug.LogAssertion($"Trying to clear area");
        clearingArea = true;
        for (int x = room.x - 1; x < room.x + 2; x++)
        {
            for (int y = room.y - 1; y < room.y + 2; y++)
            {
                Vector3Int currentRoom = new(x, y);
                if (RoomManager.IsRoomSecret(currentRoom))
                {
                    Debug.LogAssertion($"Secret room at {x}/{y}");
                    continue;
                }
                ClearFog(currentRoom);
            }
        }
        //fogMap.RefreshAllTiles();
        clearingArea = false;
    }

    public static void AddTileToClear()
    {
        tilesToClear++;

    }
}
