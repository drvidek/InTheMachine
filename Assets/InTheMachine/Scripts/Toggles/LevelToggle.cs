using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class LevelToggle : MonoBehaviour
{
    protected bool active;
    [SerializeField] public UnityEvent<bool> onActiveChanged;
    [SerializeField] protected Animator animator;

    private void Start()
    {
        RoomManager.main.onPlayerMovedRoom += RoomCheck;
    }

    protected virtual void ToggleActive(bool active, bool force = false)
    {
        this.active = active;
        onActiveChanged?.Invoke(active);
    }

    private void RoomCheck(Vector3Int room)
    {
        if (animator)
            animator.enabled = RoomManager.main.PlayerWithinRoomDistance(transform);
    }
}
