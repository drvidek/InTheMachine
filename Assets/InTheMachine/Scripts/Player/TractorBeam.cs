using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class TractorBeam : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float beamPower;
    [SerializeField] private Transform beamOrigin;
    [SerializeField] private ParticleSystem psysBeam;
    [SerializeField] private float tractorForgiveness;
    private Collider2D beam;
    private bool active = false;
    private bool snapped = false;
    private Alarm tractorForgivenessAlarm;

    public bool Active => active;

    [SerializeField] private Rigidbody2D currentBody = null;
    private List<ContactPoint2D> currentBodyContactCheck = new List<ContactPoint2D>();

    // Start is called before the first frame update
    void Start()
    {
        tractorForgivenessAlarm = Alarm.Get(tractorForgiveness, false, false);
        beam = transform.GetChild(0).GetComponent<Collider2D>();
        Player.main.onFlyExit += () => ToggleBeam(false);
        Player.main.onShootPress += () => ToggleBeam(true);
        Player.main.onShootRelease += () => ToggleBeam(false);
        ToggleBeam(false);
    }

    public void Update()
    {
        if (currentBody == null)
            return;

        float z = currentBody.transform.position.z;
        if (!snapped && QMath.MoveTowardsAndSnap(currentBody.transform, beamOrigin.position, beamPower * Time.deltaTime, 0.05f))
            snapped = true;

        if (snapped)
            currentBody.transform.position = beamOrigin.position;

        currentBody.transform.position = QMath.ReplaceVectorValue(currentBody.transform.position, VectorValue.z, z);
        //currentBody.transform.position = Vector2.SmoothDamp(currentBody.transform.position, beamOrigin.position, ref dampVel, 0.1f);
    }

    private void FixedUpdate()
    {
        if (!active)
        {
            if (currentBody != null)
                DetatchConnectedBody();
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + (Vector3)beam.offset, (beam as BoxCollider2D).size, 0, 1 << 8);

        if (colliders.Length == 0)
            return;

        if (currentBody)
        {
            currentBody.gravityScale = 0;
            currentBody.velocity = Vector2.zero;

            if (tractorForgivenessAlarm.IsStopped)
            {
                currentBodyContactCheck.Clear();

                if (currentBody.GetContacts(currentBodyContactCheck) > 0)
                {
                    DetatchConnectedBody();
                    ToggleBeam(false);
                }
            }
            return;
        }

        float smallestDistance = float.PositiveInfinity;

        foreach (var collider in colliders)
        {
            float newDistance = Vector3.Distance(collider.ClosestPoint(beamOrigin.position), beamOrigin.position);
            if (newDistance < smallestDistance)
            {
                smallestDistance = newDistance;
                currentBody = collider.attachedRigidbody;
            }

        }
        if (currentBody)
            tractorForgivenessAlarm.ResetAndPlay();
    }

    public void ToggleBeam(bool active)
    {
        if (!Player.main.IsFlying)
            active = false;

        this.active = active;

        psysBeam.Play();

        if (active)
        {
            return;
        }
        tractorForgivenessAlarm.Stop();
        psysBeam.Stop();
        psysBeam.Clear();
        DetatchConnectedBody();
    }

    private void DetatchConnectedBody()
    {
        if (currentBody != null)
            currentBody.gravityScale = 1;
        currentBody = null;
        snapped = false;
    }
}
