using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RevealSecret : MonoBehaviour
{
    private Color currentColor = Color.white;
    private bool revealed;
    private Tilemap map;
    private Collider2D _collider;

    private void Start()
    {
        map = GetComponent<Tilemap>();
        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        currentColor = Color.Lerp(currentColor, revealed ? Color.clear : Color.white, 0.5f);
        map.color = currentColor;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out Player p))
            revealed = _collider.OverlapPoint(p.transform.position);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
            revealed = false;
    }
}
