using DG.Tweening;
using UnityEngine;

namespace Project.Gameplay
{
    [RequireComponent(typeof(Health))]
    public class DamageFlasher : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Add here all objects renders (enemy- 1, player(car) - 6 or more)")]
        [SerializeField] private Renderer[] _renderers;

        [Header("Settings")]
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _flashDuration = 0.15f;

        private Health _health;
        private Material[] _instancedMaterials;
        private Color[] _originalColors;

        private void Awake()
        {
            _health = GetComponent<Health>();

            if (_renderers == null || _renderers.Length == 0) return;

            _instancedMaterials = new Material[_renderers.Length];
            _originalColors = new Color[_renderers.Length];

            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] == null) continue;

                _instancedMaterials[i] = _renderers[i].material;

                _originalColors[i] = _instancedMaterials[i].HasProperty("_BaseColor")
                    ? _instancedMaterials[i].GetColor("_BaseColor")
                    : _instancedMaterials[i].color;
            }
        }

        private void OnEnable()
        {
            if (_health != null)
                _health.OnDamaged += HandleDamageFlash;
        }

        private void OnDisable()
        {
            if (_health != null)
                _health.OnDamaged -= HandleDamageFlash;

            if (_instancedMaterials == null) return;

#pragma warning disable CS4014
            for (int i = 0; i < _instancedMaterials.Length; i++)
            {
                if (_instancedMaterials[i] == null) continue;

                _instancedMaterials[i].DOKill();

                if (_instancedMaterials[i].HasProperty("_EmissionColor"))
                {
                    _instancedMaterials[i].SetColor("_EmissionColor", Color.black);
                }
            }
#pragma warning restore CS4014
        }

        private void HandleDamageFlash(int damageAmount)
        {
            if (_instancedMaterials == null) return;

#pragma warning disable CS4014
            for (int i = 0; i < _instancedMaterials.Length; i++)
            {
                Material mat = _instancedMaterials[i];
                if (mat == null) continue;

                mat.DOKill();

                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.EnableKeyword("_EMISSION");

                    Color intenseFlash = _flashColor * 4f;
                    mat.SetColor("_EmissionColor", intenseFlash);

                    mat.DOColor(Color.black, "_EmissionColor", _flashDuration)
                        .SetLink(gameObject);
                }
                else
                {
                    mat.color = _flashColor;
                    mat.DOColor(_originalColors[i], _flashDuration)
                        .SetLink(gameObject);
                }
            }
#pragma warning restore CS4014
        }

        private void OnDestroy()
        {
            if (_instancedMaterials == null) return;

            foreach (var mat in _instancedMaterials)
            {
                if (mat != null) Destroy(mat);
            }
        }
    }
}