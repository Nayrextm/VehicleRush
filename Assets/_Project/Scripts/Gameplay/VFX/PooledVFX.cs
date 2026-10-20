using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace Project.Gameplay.VFX
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledVFX : MonoBehaviour
    {
        private IObjectPool<PooledVFX> _pool;
        private ParticleSystem _particleSystem;
        private float _duration;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _duration = _particleSystem.main.duration;
        }

        public void InitAndPlay(IObjectPool<PooledVFX> pool)
        {
            _pool = pool;
            PlayAndForgetAsync().Forget();
        }

        private async UniTaskVoid PlayAndForgetAsync()
        {
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _particleSystem.Play();

            await UniTask.Delay(
                (int)(_duration * 1000),
                delayType: DelayType.DeltaTime, 
                cancellationToken: this.GetCancellationTokenOnDestroy()
            );

            if (gameObject.activeSelf && _pool != null)
            {
                _pool.Release(this);
            }
        }
    }
}