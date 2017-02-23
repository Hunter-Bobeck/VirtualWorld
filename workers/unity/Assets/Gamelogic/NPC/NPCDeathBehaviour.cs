﻿using Assets.Gamelogic.Core;
using Improbable.Core;
using Improbable.Life;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC
{
    [EngineType(EnginePlatform.FSim)]
    public class NPCDeathBehaviour : MonoBehaviour
    {
        [Require] private FSimAuthorityCheck.Writer fSimAuthorityCheck;
        [Require] private Health.Reader health;

        private bool npcDeathActive;

        private void OnEnable()
        {
            npcDeathActive = SimulationSettings.NPCDeathActive;
            health.ComponentUpdated += OnHealthUpdated;
        }

        private void OnDisable()
        {
            health.ComponentUpdated -= OnHealthUpdated;
        }

        private void OnHealthUpdated(Health.Update update)
        {
            if (update.currentHealth.HasValue)
            {
                DieUponHealthDepletion(update);
            }
        }

        private void DieUponHealthDepletion(Health.Update update)
        {
            if (npcDeathActive && update.currentHealth.Value <= 0)
            {
                SpatialOS.Commands.DeleteEntity(fSimAuthorityCheck, gameObject.EntityId(), result => { });
            }
        }
    }
}