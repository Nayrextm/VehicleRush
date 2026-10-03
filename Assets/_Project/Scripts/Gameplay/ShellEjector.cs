using UnityEngine;
using UnityEngine.Pool;

namespace Project.Gameplay
{
    public class ShellEjector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShellCasing _shellPrefab;
        [SerializeField] private Transform _ejectionPort;

        [Header("Settings")]
        [SerializeField] private float _ejectForce = 5f;
        [SerializeField] private int _defaultCapacity = 15;
        [SerializeField] private int _maxSize = 30;

        private IObjectPool<ShellCasing> _shellPool;

        private void Awake()
        {
            _shellPool = new ObjectPool<ShellCasing>(
                createFunc: () => Instantiate(_shellPrefab),
                actionOnGet: shell => shell.gameObject.SetActive(true),
                actionOnRelease: shell => shell.gameObject.SetActive(false),
                actionOnDestroy: shell => Destroy(shell.gameObject),
                collectionCheck: false,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize
            );
        }

        private void Start()
        {
            ShellCasing[] preWarm = new ShellCasing[_defaultCapacity];
            for (int i = 0; i < _defaultCapacity; i++) preWarm[i] = _shellPool.Get();
            for (int i = 0; i < _defaultCapacity; i++) _shellPool.Release(preWarm[i]);
        }

        public void Eject()
        {
            if (_shellPool == null || _ejectionPort == null) return;

            ShellCasing shell = _shellPool.Get();

            shell.transform.SetPositionAndRotation(_ejectionPort.position, _ejectionPort.rotation);
            Vector3 ejectDirection = _ejectionPort.right + _ejectionPort.up * 0.4f;
            shell.Init(_shellPool, ejectDirection, _ejectForce);
        }
    }
}