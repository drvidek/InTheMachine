using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    private Tilemap fogMap;

    private static int tilesToClear;

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
        if (!clearingArea && tilesToClear <= 0)
            return;

        Vector3 roomPos = RoomManager.main.RoomGrid.CellToWorld(room);
        Vector3Int fogTile = fogMap.WorldToCell(roomPos);
        fogTile.z = 0;

        if (fogMap.GetTile(fogTile) == null)
            return;

        fogMap.SetTile(fogTile, null);

        if (!clearingArea)
            tilesToClear--;
    }

    private void ClearArea(Vector3Int room)
    {
        clearingArea = true;
        for (int x = room.x - 1; x < room.x + 2; x++)
        {
            for (int y = room.y - 1; y < room.y + 2; y++)
            {
                ClearFog(new(x, y));
            }
        }
        clearingArea = false;
    }

    public static void AddTileToClear()
    {
        tilesToClear++;
    }
}
