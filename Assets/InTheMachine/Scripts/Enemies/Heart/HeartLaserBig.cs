using UnityEngine;
using QKit;

public class HeartLaserBig : EnemyStatic
{
    [SerializeField] protected float speed;
    [SerializeField] protected ParticleSystem[] psysWallContact; 

    private float currentSpeed;

    private void OnEnable()
    {
        foreach (var item in psysWallContact)
        {
            item.Play();
        }
    }

    private void OnDisable()
    {
        foreach (var item in psysWallContact)
        {
            item.Stop();
        }
    }

    protected override void FixedUpdate()
    {
        Vector2 dir = Player.main.Position - transform.position;
        dir.Normalize();

        Vector2 dirCurrent = transform.right;

        float angle = Vector2.SignedAngle(dirCurrent, dir);

        float sign = Mathf.Sign(angle);

        currentSpeed = Mathf.MoveTowards(currentSpeed, sign * speed, speed * Time.fixedDeltaTime);

        transform.Rotate(Vector3.forward, currentSpeed * Time.fixedDeltaTime);

        float scale = transform.localScale.y;

        Vector3 partStart = transform.up * 0.75f * scale;

        for (int i = 0; i < psysWallContact.Length; i++)
        {
            var hit = Physics2D.Raycast(transform.position + partStart - (transform.up * 0.5f * i * scale), transform.right, float.PositiveInfinity, groundedMask);
            if (hit)
            {
                psysWallContact[i].transform.position = hit.point;
                psysWallContact[i].transform.right = transform.right;
            }
        }

        base.FixedUpdate();
    }

    protected override void CheckPlayerInRangeForLogic(Vector3Int room)
    {
        //force the laser on no matter what room the player is in
        doingLogic = true;
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        
    }
}
