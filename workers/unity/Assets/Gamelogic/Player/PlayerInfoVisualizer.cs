using Assets.Gamelogic.Core;
using Assets.Gamelogic.UI;
using Improbable.Core;
using Improbable.Fire;
using Improbable.Life;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    [EngineType(EnginePlatform.Client)]
    public class PlayerInfoVisualizer : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer clientAuthorityCheck;
        [Require] private PlayerInfo.Reader playerInfo;
        [Require] private Health.Reader health;
        [Require] private Flammable.Reader flammable;

        private float healthLocalCopy;

        [SerializeField] private CharacterModelVisualizer characterModelVisualizer;

        private void Awake()
        {
            characterModelVisualizer = gameObject.GetComponentIfUnassigned(characterModelVisualizer);
        }

        private void OnEnable()
        {
            playerInfo.ComponentUpdated += OnPlayerInfoUpdated;
            health.ComponentUpdated += OnHealthUpdated;
            healthLocalCopy = health.Data.currentHealth;
            UpdatePlayerPanelHealthBar(healthLocalCopy);
        }

        private void OnDisable()
        {
            playerInfo.ComponentUpdated -= OnPlayerInfoUpdated;
            health.ComponentUpdated -= OnHealthUpdated;
        }

        private void OnHealthUpdated(Health.Update update)
        {
            if (update.currentHealth.HasValue)
            {
                healthLocalCopy = update.currentHealth.Value;
                UpdatePlayerPanelHealthBar(healthLocalCopy);
            }
        }

        private void Update()
        {
            if (flammable.Data.isOnFire)
            {
                healthLocalCopy -= Time.deltaTime;
                UpdatePlayerPanelHealthBar(healthLocalCopy);
            }
        }

        private void UpdatePlayerPanelHealthBar(float h)
        {
            PlayerPanelController.SetPlayerHealth(h / SimulationSettings.PlayerMaxHealth);
        }

        private void OnPlayerInfoUpdated(PlayerInfo.Update update)
        {
            if (update.isAlive.HasValue)
            {
                switch (update.isAlive.Value)
                {
                    case true:
                        Resurrect();
                        break;
                    case false:
                        Die();
                        break;
                }
            }
        }

        private void Resurrect()
        {
            GameNotificationsPanelController.SetText("");
            characterModelVisualizer.SetModelVisibility(true);
        }

        private void Die()
        {
            GameNotificationsPanelController.SetText("You have died.");
        }
    }
}
