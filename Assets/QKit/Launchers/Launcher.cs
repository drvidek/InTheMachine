using System;
using UnityEngine;

namespace QKit
{
    public abstract class Launcher : MonoBehaviour
    {
        //These references exist to prevent runtime crashes, per advice from the below
        //https://docs.unity3d.com/ScriptReference/GameObject.CreatePrimitive.html
        private MeshFilter _mfCrashPrevent;
        private MeshRenderer _mrCrashPrevent;
        private SphereCollider _scCrashPrevent;
        //projectile properties
        [SerializeField] protected float _size, _speed, _power, _lifetime;
        //shot count
        [SerializeField] protected int _count;
        //shot spread
        [SerializeField] protected float _spread;
        //launcher position
        [SerializeField] protected Transform _spawn;
        //layer to collide with
        [SerializeField] protected LayerMask _collidingLayer;
        //layers to pierce
        [SerializeField] protected LayerMask _piercingLayer;
        [SerializeField] protected Projectile _projectilePrefab;

        public Action onShoot;

        /// <summary>
        /// Define the direction the launcher should shoot
        /// </summary>
        /// <returns></returns>
        protected abstract Vector3 GetDirection();
        /// <summary>
        /// Checks if this launcher CanShoot, and if so, calls Shoot then Reload and returns true. Else returns false.
        /// </summary>
        /// <returns></returns>
        public bool TryToShoot()
        {
            if (CanShoot())
            {
                Shoot();
                Reload();
                onShoot?.Invoke();
                return true;
            }

            return false;
        }
        /// <summary>
        /// Define the conditions under which this launcher is allowed to shoot.
        /// </summary>
        /// <returns></returns>
        protected abstract bool CanShoot();
        /// <summary>
        /// Override to define how the launcher instantiates its projectile and applies properties.
        /// </summary>
        protected virtual void Shoot()
        {
            Vector3 direction = GetDirection();
            if (!_projectilePrefab)
            {
                Debug.LogError($"No projectile prefab is installed in {gameObject.name}'s launcher.");
                return;
               
            }

            Projectile projectile = Instantiate(_projectilePrefab, _spawn.position, Quaternion.identity, null);
            ApplyPropertiesToProjectile(projectile, direction);
        }
        /// <summary>
        /// Override to define how the launcher applies properties to the given projectile
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="direction"></param>
        protected virtual void ApplyPropertiesToProjectile(Projectile projectile, Vector3 direction)
        {
            projectile.ApplyProjectileProperties(direction, _size, _speed, _lifetime, _power, _collidingLayer, _piercingLayer);
        }
        /// <summary>
        /// Define what action the launcher should take after shooting
        /// </summary>
        protected abstract void Reload();
    }
}
