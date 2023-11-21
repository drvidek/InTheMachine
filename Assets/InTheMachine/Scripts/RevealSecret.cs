using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RevealSecret : MonoBehaviour
{
    private Color currentColor = Color.white;
    private bool revealed;
    private Tilemap map;

    private void Start()
    {
        map = GetComponent<Tilemap>();

    }

    private void Update()
    {
        currentColor = Color.Lerp(currentColor, revealed ? Color.clear : Color.white, 0.5f);
        map.color = currentColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
            revealed = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
            revealed = false;
    }
}
