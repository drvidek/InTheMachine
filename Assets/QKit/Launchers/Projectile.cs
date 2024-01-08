using System.Collections.Generic;
using UnityEngine;
using QKit;
namespace QKit
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Projectile : MonoBehaviour
    {
        //speed,size
        protected float _speed, _size, _power, _lifetime;
        //layer to collide with
        protected LayerMask _collidingLayer;
        //layers to pierce
        protected LayerMask _piercingLayer;
        protected LayerMask _pinpointLayer;
        protected Vector3 _direction;
        protected Rigidbody2D rb;
        protected List<Collider2D> pierceList = new();

        public Vector3 Direction => _direction;
        public float Speed => _speed;
        public float Size => _size;
        public float Power => _power;

        protected virtual void OnValidate()
        {
            SetRigidbody();
        }

        protected virtual void Start()
        {
            SetRigidbody();
            CircleCollider2D myCollider = GetComponentInChildren<CircleCollider2D>();
            foreach (var other in Physics2D.OverlapCircleAll(transform.position, myCollider.radius))
            {
                Debug.Log(other.name);
                int otherLayer = other.gameObject.layer;
                bool collision = CheckForCollision(otherLayer);
                bool piercing = CheckForPierce(otherLayer);
                bool pinpoint = false;

                if (_pinpointLayer != 0)
                {
                    pinpoint = CheckForPinpoint(otherLayer);
                }


                if (!collision && !piercing && !pinpoint)
                   break;

                if (pinpoint)
                {
                    pinpoint = false;
                    foreach (var collider in Physics2D.OverlapCircleAll(transform.position, 0.2f, _pinpointLayer))
                    {
                        if (collider == other)
                        {
                            pinpoint = true;
                            break;
                        }
                    }
                    if (!pinpoint)
                        break;
                }

                TryToHitTarget(other);

                if (collision || pinpoint)
                {
                    DoCollision(other);
                }
            }

        }

        protected virtual void SetRigidbody()
        {
            if (!rb)
            {
                rb = GetComponent<Rigidbody2D>();
                Collider2D collider = rb.GetComponentInChildren<Collider2D>();
                collider.isTrigger = true;
            }
        }

        public virtual void ApplyProjectileProperties(Vector3 direction, float size, float speed, float lifetime, float power, LayerMask colliding, LayerMask piercing, LayerMask pinpoint)
        {
            _direction = direction;
            _size = size;
            transform.localScale = Vector3.one * size;
            _speed = speed;
            _lifetime = lifetime;
            _power = power;
            _collidingLayer = colliding;
            _piercingLayer = piercing;
            _pinpointLayer = pinpoint;

            SetRigidbody();
            rb.velocity = direction * speed;
        }

        /// <summary>
        /// Move in inherited direction at inherited speed per fixedDeltaTime
        /// </summary>
        protected virtual void MoveFixed()
        {
            rb.MovePosition(transform.position + (_direction * _speed * Time.fixedDeltaTime));
        }
        /// <summary>
        /// Move in inherited direction at inherited speed per deltaTime
        /// </summary>
        protected virtual void Move()
        {
            rb.MovePosition(transform.position + (_direction * _speed * Time.deltaTime));
        }
        /// <summary>
        /// Move in direction at speed, pass true/false for fixed/unfixed deltaTime
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        /// <param name="fixedTime"></param>
        protected virtual void Move(Vector3 direction, float speed, bool fixedTime = true)
        {
            float delta = fixedTime ? Time.fixedDeltaTime : Time.deltaTime;
            rb.MovePosition(transform.position + (direction * speed * delta));
        }

        /// <summary>
        /// Casts a ray from object's current position in Direction and checks for any collisions or pierces. Stops at a collision and returns the hit coordinates in world space. Else returns the last pierced point.
        /// </summary>
        /// <returns></returns>
        protected virtual Vector3 Hitscan()
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, _direction, float.PositiveInfinity, _collidingLayer | _piercingLayer);
            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                LayerMask hitLayer = hits[i].collider.gameObject.layer;
                TryToHitTarget(hits[i].collider);

                if (CheckForCollision(hitLayer))
                {
                    DoCollision(hits[i].collider);
                    return hits[i].point;
                }
            }
            return hits[^1].point;
        }

        /// <summary>
        /// Casts a ray from position in direction for length and checks for any collisions or pierces. Stops at a collision and returns the worldspace hit coordinates. Else returns the point reached with no collision.
        /// </summary>
        /// <returns></returns>
        protected virtual Vector3 Hitscan(Vector3 position, Vector3 direction, float length)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(position, direction, length, _collidingLayer | _piercingLayer);
            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                LayerMask hitLayer = hits[i].collider.gameObject.layer;
                TryToHitTarget(hits[i].collider);

                if (CheckForCollision(hitLayer))
                {
                    DoCollision(hits[i].collider);
                    return hits[i].point;
                }
            }
            return position + (direction * length);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            LayerMask otherLayer = other.gameObject.layer;
            bool collision = CheckForCollision(otherLayer);
            bool piercing = CheckForPierce(otherLayer);

            if (!collision && !piercing)
                return;
            TryToHitTarget(other);

            if (collision)
            {
                DoCollision(other);
            }
        }

        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            LayerMask otherLayer = other.gameObject.layer;
            bool collision = CheckForCollision(otherLayer);
            bool piercing = CheckForPierce(otherLayer);
            bool pinpoint = false;

            if (_pinpointLayer != 0)
            {
                pinpoint = CheckForPinpoint(otherLayer);
            }


            if (!collision && !piercing &&!pinpoint)
                return;

            if (pinpoint)
            {
                pinpoint = false;
                foreach (var collider in Physics2D.OverlapCircleAll(transform.position,0.2f,_pinpointLayer))
                {
                    if (collider == other)
                        pinpoint = true;
                }
                if (!pinpoint)
                    return;
            }

            TryToHitTarget(other);

            if (collision || pinpoint)
            {
                DoCollision(other);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (pierceList.Contains(collision))
                pierceList.Remove(collision);
        }


        /// <summary>
        /// Check a collider for a valid ProjectileTarget and trigger OnProjectileHit and return true if found
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        protected virtual bool TryToHitTarget(Collider2D collider)
        {
            if (!pierceList.Contains(collider) && TryGet<IProjectileTarget>(collider.transform, out IProjectileTarget target))
            {
                target.OnProjectileHit(this);
                pierceList.Add(collider);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Executes when a projectile hits a collider through Move or Hitscan
        /// </summary>
        protected virtual void DoCollision(Collider2D collider)
        {
            EndOfLife();
        }

        protected bool TryGet<T>(Transform getFrom, out T result)
        {
            if (getFrom.TryGetComponent<T>(out result))
                return true;
            if (getFrom.parent != null && getFrom.parent.TryGetComponent<T>(out result))
                return true;
            return (getFrom.root.TryGetComponent<T>(out result));
        }

        protected bool CheckForCollision(LayerMask layer)
        {
            return _collidingLayer == (_collidingLayer | (1 << layer));
        }

        protected bool CheckForPierce(LayerMask layer)
        {
            return _piercingLayer == (_piercingLayer | (1 << layer));
        }

        protected bool CheckForPinpoint(LayerMask layer)
        {
            return _pinpointLayer == (_pinpointLayer | (1 << layer));
        }

        protected virtual void EndOfLife()
        {
            Destroy(gameObject);
        }
    }
}
