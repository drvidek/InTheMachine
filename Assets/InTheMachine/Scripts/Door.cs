using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Door : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] doorParts;
    [SerializeField] private float timeToClose;
    [SerializeField] private bool startOpen;
    [SerializeField] private bool stayOpen;
    [SerializeField] private Sprite[] doorSprites;
    [SerializeField] private ParticleSystem psysActive;

    private Collider2D[] colliders;
    private int partCount;

    private float currentOpen;

    private bool open;
    private bool everOpened;


    private void Start()
    {
        doorParts = transform.GetComponentsInChildren<SpriteRenderer>();

        partCount = doorParts.Length;
        currentOpen = partCount;
        colliders = new Collider2D[doorParts.Length];
        var i = 0;
        foreach (var part in doorParts)
        {
            colliders[i] = part.GetComponent<Collider2D>();
            i++;
        }
        ChangeActiveState(false);
    }

    private void Update()
    {
        float target = open ? 0 : partCount;
        currentOpen = Mathf.MoveTowards(currentOpen, target, partCount / timeToClose * Time.deltaTime);
        for (int i = 0; i < doorParts.Length; i++)
        {
            if (currentOpen <= i)
            {
                doorParts[i].sprite = null;
                colliders[i].enabled = false;
            }
            if (currentOpen > i)
            {
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

    public void ChangeActiveState(bool active)
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
}
