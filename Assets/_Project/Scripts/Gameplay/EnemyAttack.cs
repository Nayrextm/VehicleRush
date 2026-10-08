using UnityEngine;

namespace Project.Gameplay
{
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private int _damage = 25;
        [SerializeField] private EnemyEntity _enemyEntity;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Health playerHealth = other.GetComponentInParent<Health>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(_damage);

                    if (_enemyEntity != null)
                    {
                        _enemyEntity.Explode();
                    }
                }
            }
        }
    }
}