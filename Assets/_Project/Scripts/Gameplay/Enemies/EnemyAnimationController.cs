using UnityEngine;

namespace Project.Gameplay.Enemies
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimationController : MonoBehaviour
    {
        private Animator _animator;

        private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
        private static readonly int HitTriggerHash = Animator.StringToHash("Hit");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetSpeed(float speed)
        {
            _animator.speed = speed;
        }

        public void SetRunningState(bool isRunning)
        {
            _animator.SetBool(IsRunningHash, isRunning);
        }

        public void PlayHitAnimation()
        {
            _animator.SetTrigger(HitTriggerHash);
        }
    }
}