using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PixelAligner : MonoBehaviour
{
    private Vector2 offset;
    [SerializeField] private bool objectRotates;
    [HideInInspector]
    [SerializeField] private Vector2 animOffset = new(0, 0);
    [SerializeField] private Transform parent;
    public static float OnePixel => 1f / GameManager.pixelsPerUnit;
    public Vector2 Offset => offset;

    private SpriteRenderer sprite;

    public static Vector3 RoundPositionToPixel(Vector3 position)
    {
        return new(
            RoundFloatToPixel(position.x, GameManager.pixelsPerUnit),
            RoundFloatToPixel(position.y, GameManager.pixelsPerUnit),
            position.z
            );
    }

    public static float PixelsToWidth(float pixels)
    {
        return OnePixel * (float)pixels;
    }

    public void SetOffset(Vector2 offset)
    {
        this.offset = offset;
    }

    private void Start()
    {
        offset = transform.localPosition;
        sprite = GetComponent<SpriteRenderer>();

        if (parent == null)
            parent = transform.parent;
    }

    protected virtual void LateUpdate()
    {
        Vector2 realPosition = parent.position;
        Vector3 PixelPosition = new(
            RoundFloatToPixel(realPosition.x, GameManager.pixelsPerUnit),
            RoundFloatToPixel(realPosition.y, GameManager.pixelsPerUnit),
            transform.position.z
            );

        Vector3 animOffsetConverted = new(PixelsToWidth(animOffset.x), PixelsToWidth(animOffset.y));

        Vector3 addedOffset = animOffsetConverted + new Vector3(offset.x, offset.y);
        Vector3 finalOffset = new Vector3(addedOffset.x * (!objectRotates && sprite.flipX ? -1 : 1), addedOffset.y * (!objectRotates && sprite.flipY ? -1 : 1));

        transform.position = PixelPosition + finalOffset;
    }

    private static float RoundFloatToPixel(float position, float pixelSize)
    {
        float remainder = position % 1;
        float multiplied = remainder * pixelSize;

        float roundedRemainder = Mathf.Round(multiplied) / pixelSize;
        return (position - remainder) + roundedRemainder;
    }
}
