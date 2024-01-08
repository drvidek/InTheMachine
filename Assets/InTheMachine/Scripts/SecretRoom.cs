using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretRoom : MonoBehaviour
{
    private void Start()
    {
        RoomManager.AddSecretRoom(RoomManager.main.GetRoom(transform));
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new(.2f, .2f, 1, 0.1f);
        Gizmos.DrawCube(transform.position, Vector3.one * 8f);
    }
}
