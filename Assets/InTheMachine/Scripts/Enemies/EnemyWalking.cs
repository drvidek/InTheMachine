using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyWalking : EnemyMachine
{
    Vector2 gravityDirection;

    public abstract Rigidbody2D GetStandingOnRigidbody();

}
