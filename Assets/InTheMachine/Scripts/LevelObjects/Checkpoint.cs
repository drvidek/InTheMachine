using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour, IActivate
{
    [SerializeField] private bool active;
    [SerializeField] private ParticleSystem psysActivate;

    private Animator animator;

    private static Checkpoint currentCheckpoint = null;

    public static Checkpoint Current => currentCheckpoint;

    public Vector3 Position => new(transform.position.x, transform.position.y, Player.main.Z);

    private void Start()
    {
        animator = GetComponent<Animator>();
        ToggleActive(active);
    }

    public void ToggleActive(bool active)
    {
        this.active = active;
        animator.SetBool("Active", active);
        psysActivate.Play();
        QuestManager.main.CompleteQuest(QuestID.Terminal);
    }

    public void ToggleActiveAndLock(bool active)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active && collision.GetComponent<Player>())
        {
            if (currentCheckpoint)
                currentCheckpoint.animator.SetBool("CurrentCheckpoint", false);
            animator.SetBool("CurrentCheckpoint", true);
            currentCheckpoint = this;
            Player.main.RefillRepairCharges();
        }
    }
}
