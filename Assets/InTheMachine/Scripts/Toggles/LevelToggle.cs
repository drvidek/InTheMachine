using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class LevelToggle : MonoBehaviour
{
    protected bool active;
    [SerializeField] public UnityEvent<bool> onActiveChanged;

    protected virtual void TriggerChange(bool active, bool force = false)
    {
        this.active = active;
        onActiveChanged?.Invoke(active);
    }
}
