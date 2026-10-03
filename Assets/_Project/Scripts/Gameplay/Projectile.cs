using UnityEngine;
using UnityEngine.Pool;

namespace Project.Gameplay
{
    public class Projectile : MonoBehaviour
    {
        private IObjectPool<Projectile> _pool;
        private float _speed;
        private float _lifetime;
        private float _currentLifeTime;

        private TrailRenderer _trailRenderer;

        private void Awake()
        {
            _trailRenderer = GetComponent<TrailRenderer>();
        }

        public void Init(IObjectPool<Projectile> pool, float speed, float lifetime)
        {
            _pool = pool;
            _speed = speed;
            _lifetime = lifetime;
            _currentLifeTime = 0f;

            if (_trailRenderer != null)
            {
                _trailRenderer.Clear();
            }
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * (_speed * Time.deltaTime), Space.Self);

            _currentLifeTime += Time.deltaTime;
            if (_currentLifeTime >= _lifetime)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (gameObject.activeSelf)
            {
                _pool.Release(this);
            }
        }
    }
}