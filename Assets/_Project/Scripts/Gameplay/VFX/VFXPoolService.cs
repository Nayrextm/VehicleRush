using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Project.Gameplay
{
    public class VFXPoolService : IDisposable
    {
        private readonly ObjectPool<PooledVFX> _deathEffectPool;
        private readonly PooledVFX _prefab;

        public VFXPoolService(PooledVFX deathEffectPrefab, int initialCapacity)
        {
            _prefab = deathEffectPrefab;

            _deathEffectPool = new ObjectPool<PooledVFX>(
                createFunc: () => UnityEngine.Object.Instantiate(_prefab),
                actionOnGet: vfx => vfx.gameObject.SetActive(true),
                actionOnRelease: vfx => vfx.gameObject.SetActive(false),
                actionOnDestroy: vfx => UnityEngine.Object.Destroy(vfx.gameObject),
                collectionCheck: false,
                defaultCapacity: initialCapacity,
                maxSize: 50
            );
        }

        public void PlayDeathEffect(Vector3 position)
        {
            if (_deathEffectPool == null) return;

            PooledVFX effect = _deathEffectPool.Get();
            effect.transform.position = position;
            effect.InitAndPlay(_deathEffectPool);
        }

        public void PreWarm(int count)
        {
            PooledVFX[] preWarm = new PooledVFX[count];
            for (int i = 0; i < count; i++) preWarm[i] = _deathEffectPool.Get();
            for (int i = 0; i < count; i++) _deathEffectPool.Release(preWarm[i]);
        }

        public void Dispose()
        {
            _deathEffectPool?.Clear();
        }
    }
}