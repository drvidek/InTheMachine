using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineTilemap : MonoBehaviour
{
    [SerializeField] private Sprite tileSprite;
    [SerializeField] private Color tileColor = Color.white;
    [SerializeField] private Vector2Int gridSize = Vector2Int.one;
    [SerializeField] private Vector2 tileSize = Vector2.one;
    [SerializeField] private SpriteDrawMode drawMode;

    private GameObject tile;
    private SpriteRenderer[,] tilemap;

    private void Awake()
    {
        Initialise();
    }

    private void Initialise()
    {
        tile = Resources.Load("MachineTile") as GameObject;
        tilemap = new SpriteRenderer[gridSize.x, gridSize.y];

        Vector2 currentPosition = transform.position;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                SpriteRenderer currentTile = Instantiate(tile, transform).GetComponent<SpriteRenderer>();
                currentTile.transform.position = currentPosition;
                currentTile.size = tileSize;
                currentTile.color = tileColor;
                currentTile.sprite = tileSprite;
                tilemap[x, y] = currentTile;
                currentPosition -= new Vector2(0, tileSize.y);

            }
            currentPosition = new Vector2(currentPosition.x + tileSize.x, transform.position.y);
        }
    }


    public void SetTileActive(Vector2 tile, bool active)
    {
        if (ValidateTile(tile))
            tilemap[(int)tile.x, (int)tile.y].enabled = active;
    }

    public void SetTileSprite(Vector2 tile, Sprite sprite)
    {
        if (ValidateTile(tile))
            tilemap[(int)tile.x, (int)tile.y].sprite = sprite;
    }

    public bool ValidateTile(Vector2 tile)
    {
        return (tile.x < gridSize.x && tile.y < gridSize.y);
    }
}
