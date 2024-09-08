using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnObject : MonoBehaviour, IActivate
{
    [SerializeField] private GameObject respawnPrefab;
    [SerializeField] private bool startSpawned;
    private GameObject objectInScene = null;
    private ParticleSystem psysActivate;

    public bool ObjectCurrentlyInScene => objectInScene != null;

    private void Start()
    {
        psysActivate = GetComponentInChildren<ParticleSystem>();
        ToggleActive(startSpawned);
    }

    public void ToggleActive(bool active)
    {
        if (!active)
            return;

        if (objectInScene == null)
            objectInScene = Instantiate(respawnPrefab);
        objectInScene.transform.position = transform.position;
        psysActivate.Play();
    }

    public void ToggleActiveAndLock(bool active)
    {
        ToggleActive(active);
    }

    
}
