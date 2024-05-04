using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using QKit;

public class RevealSecret : MonoBehaviour
{
    private Color currentColor;
    private Color homeColor;
    private bool revealed;
    private Tilemap map;
    private Collider2D _collider;

    private Meter fade = new(0, 0.5f, 0.5f);

    private void Start()
    {
        map = GetComponent<Tilemap>();
        _collider = GetComponent<CompositeCollider2D>();
        homeColor = map.color;
    }

    private void Update()
    {
        if (revealed)
            fade.FillOver(0.5f);
        else
            fade.EmptyOver(0.5f);

        currentColor = Color.Lerp(homeColor, Color.clear, fade.Percent);
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
