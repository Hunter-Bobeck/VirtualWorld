using Assets.Gamelogic.ComponentExtensions;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Life;
using Improbable.Building;
using Improbable.Core;
using Improbable.Life;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Building
{
    public class StockpileDepositoryBehaviour : MonoBehaviour
    {
        [Require] private StockpileDepository.Writer stockpileDepository;
        [Require] private Health.Writer health;

        private void OnEnable ()
        { 
            stockpileDepository.CommandReceiver.OnAddResource += OnAddResource;
        }

        private void OnDisable()
        {
            stockpileDepository.CommandReceiver.OnAddResource -= OnAddResource;
        }

        private void OnAddResource(Improbable.Entity.Component.ResponseHandle<StockpileDepository.Commands.AddResource, AddResource, Improbable.Core.Nothing> request)
        {
            if (stockpileDepository.Data.canAcceptResources)
            {
                health.AddCurrentHealthDelta(request.Request.quantity);
            }
            request.Respond(new Nothing());
        }
    }
}
