using UnityEngine;
using UnityEngine.Pool;

namespace Project.Gameplay.VFX
{
    [RequireComponent(typeof(Rigidbody))]
    public class ShellCasing : MonoBehaviour
    {
        [SerializeField] private float _lifetime = 2f;

        private IObjectPool<ShellCasing> _pool;
        private Rigidbody _rb;
        private float _currentLifeTime;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Init(IObjectPool<ShellCasing> pool, Vector3 ejectDirection, float ejectForce)
        {
            _pool = pool;
            _currentLifeTime = 0f;

            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;

            Vector3 randomDirection = ejectDirection + Random.insideUnitSphere * 0.15f;
            
            _rb.AddForce(randomDirection.normalized * ejectForce, ForceMode.Impulse);
            _rb.AddTorque(Random.insideUnitSphere * ejectForce, ForceMode.Impulse);
        }

        private void Update()
        {
            _currentLifeTime += Time.deltaTime;
            if (_currentLifeTime >= _lifetime)
            {
                ReturnToPool();
            }
        }
        private void ReturnToPool()
        {
            if (gameObject.activeSelf && _pool != null)
            {
                _pool.Release(this);
            }
        }
    }
}