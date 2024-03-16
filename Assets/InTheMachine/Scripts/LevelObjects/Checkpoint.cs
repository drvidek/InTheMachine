using System;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour, IActivate
{
    [SerializeField] private bool active;
    [SerializeField] private ParticleSystem psysActivate;
    [SerializeField] private SpriteRenderer iconSprite;
    [SerializeField] private PixelAligner radarAligner;

    private Animator animator;

    private static Checkpoint currentCheckpoint = null;

    public static Checkpoint Current => currentCheckpoint;

    public Vector3 Position => new(transform.position.x, transform.position.y, Player.main.Z);

    public static Action<Vector3Int> onActivate;

    private float radarTop = 10;
    private float radarCurrentYOffset;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ToggleActive(active);
        iconSprite.enabled = false;

        Player.main.interact.onPress += OpenShop;
    }

    private void FixedUpdate()
    {
        if (active && GameManager.IsPlaying)
        {
            radarCurrentYOffset = Mathf.MoveTowards(radarCurrentYOffset, radarTop, Time.fixedDeltaTime * 16f);
            radarAligner.AddTempOffset(new(0, radarCurrentYOffset));
        }
    }

    public void ToggleActive(bool active)
    {
        this.active = active;
        animator.SetBool("Active", active);
        if (active)
        {
            psysActivate.Play();
            QKit.Alarm alarm = QKit.AlarmPool.GetAndPlay(1f);
            alarm.onComplete += () =>
            {
                onActivate?.Invoke(RoomManager.main.GetRoom(transform));
                GameManager.main.TogglePause();
                PauseMenu.main.OpenMap();
                QuestManager.main.CompleteQuest(QuestID.Terminal);
            };
        }
    }

    public void ToggleActiveAndLock(bool active)
    {
        ToggleActive(active);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active && collision.GetComponent<Player>())
        {
            if (currentCheckpoint)
                currentCheckpoint.animator.SetBool("CurrentCheckpoint", false);
            animator.SetBool("CurrentCheckpoint", true);
            currentCheckpoint = this;
            iconSprite.enabled = true;
            Player.main.RefillRepairCharges();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (active && collision.GetComponent<Player>())
            iconSprite.enabled = false;
    }

    private void OpenShop()
    {
        if (!active || !Physics2D.OverlapBox(transform.position, Vector2.one, 0, 1 << 6))
            return;

        Shop.main.OpenShop();
        GameManager.main.TogglePause();
    }
}
