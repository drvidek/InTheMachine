using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IActivate
{
    public abstract void ToggleActive(bool active);
    public abstract void ToggleActiveAndLock(bool active);
}
