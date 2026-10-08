using DG.Tweening;
using UnityEngine;

namespace Project.Gameplay
{
    [RequireComponent(typeof(Health))]
    public class EnemyVisuals : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _flashDuration = 0.15f;

        private Health _health;
        private Material _instancedMaterial;
        private Color _originalColor;

        private void Awake()
        {
            _health = GetComponent<Health>();

            _instancedMaterial = _renderer.material;

            _originalColor = _instancedMaterial.HasProperty("_BaseColor")
                ? _instancedMaterial.GetColor("_BaseColor")
                : _instancedMaterial.color;
        }

        private void OnEnable() => _health.OnDamaged += HandleDamageFlash;

        private void OnDisable()
        {
            _health.OnDamaged -= HandleDamageFlash;

            _instancedMaterial.DOKill();

            if (_instancedMaterial.HasProperty("_EmissionColor"))
            {
                _instancedMaterial.SetColor("_EmissionColor", Color.black);
            }
        }

        private void HandleDamageFlash(int damage)
        {
            _instancedMaterial.DOKill();

            if (_instancedMaterial.HasProperty("_EmissionColor"))
            {
                _instancedMaterial.EnableKeyword("_EMISSION");

                Color intenseFlash = _flashColor * 4f;
                _instancedMaterial.SetColor("_EmissionColor", intenseFlash);

                _instancedMaterial.DOColor(Color.black, "_EmissionColor", _flashDuration);
            }
            else
            {
                _instancedMaterial.color = _flashColor;
                _instancedMaterial.DOColor(_originalColor, _flashDuration);
            }
        }

        private void OnDestroy()
        {
            if (_instancedMaterial != null)
            {
                Destroy(_instancedMaterial);
            }
        }
    }
}