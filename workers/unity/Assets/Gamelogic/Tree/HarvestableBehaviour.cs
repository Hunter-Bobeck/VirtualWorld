using Assets.Gamelogic.ComponentExtensions;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Life;
using Improbable.Entity.Component;
using Improbable.Life;
using Improbable.Tree;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Tree
{
    public class HarvestableBehaviour : MonoBehaviour
    {
        [Require] private Harvestable.Writer harvestable;
        [Require] private Health.Writer health;

        private void OnEnable()
        {
            harvestable.CommandReceiver.OnHarvest += OnHarvest;
        }

        private void OnDisable()
        {
            harvestable.CommandReceiver.OnHarvest -= OnHarvest;
        }

        private void OnHarvest(ResponseHandle<Harvestable.Commands.Harvest, HarvestRequest, HarvestResponse> request)
        {
            var resourcesToGive = Mathf.Min(SimulationSettings.HarvestReturnQuantity, health.Data.currentHealth);
            health.AddCurrentHealthDelta(-resourcesToGive);
            request.Respond(new HarvestResponse(resourcesToGive));
        }
    }
}
