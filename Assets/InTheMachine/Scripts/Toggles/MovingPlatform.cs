using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector2 direction;
    [SerializeField] private float ascendSpeed;
    [SerializeField] private float descendSpeed;
    private Rigidbody2D rb;
    private bool active;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.velocity = direction * (active ? ascendSpeed : descendSpeed);
    }

    public void ToggleActive(bool active)
    {
        this.active = active;
    }
}
