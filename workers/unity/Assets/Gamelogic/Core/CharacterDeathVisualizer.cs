using Improbable.Life;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [EngineType(EnginePlatform.Client)]
    public class CharacterDeathVisualizer : MonoBehaviour
    {
        [Require] private Health.Reader health;

        [SerializeField] private CharacterModelVisualizer characterModelVisualizer;

        private void Awake()
        {
            characterModelVisualizer = gameObject.GetComponentIfUnassigned(characterModelVisualizer);
        }

        private void OnEnable()
        {
            characterModelVisualizer.SetModelVisibility(true);
            health.ComponentUpdated += HealthUpdated;
        }

        private void OnDisable()
        {
            health.ComponentUpdated -= HealthUpdated;
        }

        private void HealthUpdated(Health.Update update)
        {
            if (update.currentHealth.HasValue && update.currentHealth.Value <= 0)
            {
                PlayDeathAnimation();
            }
        }

        private void PlayDeathAnimation()
        {
            DeathAnimVisualizerPool.ShowEffect(transform.position);
            characterModelVisualizer.SetModelVisibility(false);
        }
    }
}
