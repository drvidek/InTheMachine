using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Door : MonoBehaviour, IActivate
{
    [SerializeField] private int doorLength = 2;
    [SerializeField] private SpriteRenderer[] doorParts;
    [SerializeField] private float timeToClose;
    [SerializeField] private bool startOpen;
    [SerializeField] private Sprite[] doorSprites;
    [SerializeField] private ParticleSystem psysActive;

    private Collider2D[] colliders;

    private bool stayOpen;
    private float currentOpen;

    private bool open;
    private bool everOpened;

    private void OnValidate()
    {
        Initialise();
    }

    private void Start()
    {
        Initialise();

        currentOpen = doorLength;
        colliders = new Collider2D[doorParts.Length];
        var i = 0;
        foreach (var part in doorParts)
        {
            colliders[i] = part.GetComponent<Collider2D>();
            i++;
        }
        ToggleActive(false);
    }

    public void SetValues(int length, float rotation)
    {
#if UNITY_EDITOR
        doorLength = length;
        transform.localEulerAngles = new(0, 0, rotation);
#endif
    }

    public void Initialise()
    {
        doorParts = transform.GetComponentsInChildren<SpriteRenderer>();

        ///Future Aeon -
        ///DestroyImmediate cannot be called by OnValidate
        ///So annoying and boring

        while (doorParts.Length < doorLength)
        {
            Transform door = Instantiate(transform.GetChild(0), transform);
            door.localPosition = Vector3.down * (doorParts.Length);
            doorParts = transform.GetComponentsInChildren<SpriteRenderer>();
        }
    }

    private void Update()
    {
        float target = open ? 0 : doorLength;
        currentOpen = Mathf.MoveTowards(currentOpen, target, doorLength / timeToClose * Time.deltaTime);
        for (int i = 0; i < doorParts.Length; i++)
        {
            if (currentOpen <= i)
            {
                doorParts[i].sprite = null;
                colliders[i].enabled = false;
            }
            if (currentOpen > i)
            {

                if (!Physics2D.OverlapBox(colliders[i].transform.position, colliders[i].bounds.size, 0, 1 << 6))
                    colliders[i].enabled = true;

                if (!open)
                {
                    if (currentOpen - 1 >= i)
                        doorParts[i].sprite = doorSprites[0];
                    else
                        if (currentOpen - 0.5f >= i)
                        doorParts[i].sprite = doorSprites[1];
                }
                else
                {
                    if (currentOpen < i + 0.5f)
                        doorParts[i].sprite = null;
                    else
                    if (currentOpen < i + 1)
                        doorParts[i].sprite = doorSprites[1];
                }
            }
        }
    }

    public void ToggleActive(bool active)
    {
        if (stayOpen && everOpened)
            return;

        open = active;
        if (startOpen)
            open = !active;

        if (open)
            everOpened = true;

        psysActive.Play();
    }

    public void ToggleActiveAndLock(bool active)
    {
        ToggleActive(active);
        stayOpen = true;
    }
}
