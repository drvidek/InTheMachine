using System;
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

    public static Action<Vector3Int> onActivate;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ToggleActive(active);

        Player.main.interact.onPress += OpenShop;
    }

    public void ToggleActive(bool active)
    {
        this.active = active;
        animator.SetBool("Active", active);
        if (active)
        {
            onActivate?.Invoke(RoomManager.main.GetRoom(transform));
            psysActivate.Play();
            QuestManager.main.CompleteQuest(QuestID.Terminal);
        }
    }

    public void ToggleActiveAndLock(bool active)
    {
        this.active = active;
        animator.SetBool("Active", active);
        if (active)
        {
            psysActivate.Play();
            QuestManager.main.CompleteQuest(QuestID.Terminal);
        }
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

    private void OpenShop()
    {
        if (!active || !Physics2D.OverlapBox(transform.position, Vector2.one, 0,1<< 6))
            return;

        Shop.main.OpenShop();
        GameManager.main.TogglePause();
    }
}
