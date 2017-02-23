using Improbable.Core;
using Improbable.Entity.Component;
using Improbable.Fire;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable.Unity.Core;
using System.Collections;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable.Unity;

namespace Assets.Gamelogic.Fire
{
    [EngineType(EnginePlatform.FSim)]
    public class FlammableBehaviour : MonoBehaviour
    {
        [Require] private Flammable.Writer flammable;

        public bool IsOnFire { get { return flammable != null && flammable.Data.isOnFire; } }
        private Collider[] nearbyColliders = new Collider[8];
        private Coroutine spreadFlamesCoroutine;

        private IFlammable[] flammableInterfaces;

        private void Awake()
        {
            flammableInterfaces = gameObject.GetComponents<IFlammable>();
        }

        private void OnEnable()
        {
            flammable.CommandReceiver.OnIgnite += OnIgnite;
            flammable.CommandReceiver.OnExtinguish += OnExtinguish;
            flammable.CommandReceiver.OnSetCanBeIgnited += OnSetCanBeIgnited;

            if (flammable.Data.isOnFire)
            {
                StartFlameSpread();
            }
        }

        private void OnDisable()
        {
            flammable.CommandReceiver.OnIgnite -= OnIgnite;
            flammable.CommandReceiver.OnExtinguish -= OnExtinguish;
            flammable.CommandReceiver.OnSetCanBeIgnited -= OnSetCanBeIgnited;

            StopFlameSpread();
        }

        private void OnIgnite(ResponseHandle<Flammable.Commands.Ignite, Nothing, Nothing> request)
        {
            Ignite();
            request.Respond(new Nothing());
        }

        private void OnExtinguish(ResponseHandle<Flammable.Commands.Extinguish, ExtinguishRequest, Nothing> request)
        {
            Extinguish(request.Request.canBeIgnited);
            request.Respond(new Nothing());
        }

        private void OnSetCanBeIgnited(ResponseHandle<Flammable.Commands.SetCanBeIgnited, SetCanBeIgnitedRequest, Nothing> request)
        {
            SetCanBeIgnited(request.Request.canBeIgnited);
            request.Respond(new Nothing());
        }

        private void Ignite()
        {
            if (!flammable.Data.isOnFire && flammable.Data.canBeIgnited)
            {
                IgniteUpdate();
                StartFlameSpread();
                for (var i = 0; i < flammableInterfaces.Length; i++)
                {
                    flammableInterfaces[i].OnIgnite();
                }
            }
        }

        private void Extinguish(bool canBeIgnited)
        {
            if (flammable.Data.isOnFire)
            {
                ExtinguishUpdate(canBeIgnited);
                StopFlameSpread();
                for (var i = 0; i < flammableInterfaces.Length; i++)
                {
                    flammableInterfaces[i].OnExtinguish();
                }
            }
        }

        private void SetCanBeIgnited(bool canBeIgnited)
        {
            if (flammable.Data.canBeIgnited != canBeIgnited)
            {
                flammable.Send(new Flammable.Update().SetCanBeIgnited(canBeIgnited));
            }
        }

        public void SelfIgnite(IComponentWriter writer)
        {
            if (flammable == null)
            {
                SpatialOS.Commands.SendCommand(writer, Flammable.Commands.Ignite.Descriptor, new Nothing(), 
                    gameObject.EntityId(), _ => { });
                return;
            }
            Ignite();
        }

        public void SelfExtinguish(IComponentWriter writer, bool canBeIgnited)
        {
            if (flammable == null)
            {
                SpatialOS.Commands.SendCommand(writer, Flammable.Commands.Extinguish.Descriptor, new ExtinguishRequest(canBeIgnited), 
                    gameObject.EntityId(), _ => { });
                return;
            }
            Extinguish(canBeIgnited);
        }

        public void SelfSetCanBeIgnited(IComponentWriter writer, bool canBeIgnited)
        {
            if (flammable == null)
            {
                SpatialOS.Commands.SendCommand(writer, Flammable.Commands.SetCanBeIgnited.Descriptor, new SetCanBeIgnitedRequest(canBeIgnited), 
                    gameObject.EntityId(), _ => { });
                return;
            }
            SetCanBeIgnited(canBeIgnited);
        }

        private void StartFlameSpread()
        {
            spreadFlamesCoroutine = StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.FireSpreadInterval, SpreadFlame));
        }

        private void StopFlameSpread()
        {
            if (spreadFlamesCoroutine != null)
            {
                StopCoroutine(spreadFlamesCoroutine);
            }
        }

        private void SpreadFlame()
        {
            if (flammable == null)
            {
                Debug.LogError("flammable component was set to null in the flammable behaviour");
                return;
            }

            var count = Physics.OverlapSphereNonAlloc(transform.position, SimulationSettings.FireSpreadRadius, nearbyColliders);
            for (var i = 0; i < count; i++)
            {
                var otherFlammable = nearbyColliders[i].transform.GetComponentInParent<FlammableDataVisualizer>();
                if (otherFlammable == null || !otherFlammable.canBeIgnited)
                {
                    continue;
                }

                // Cache local ignitable value, to avoid duplicated ignitions within 1 frame on an FSim
                otherFlammable.SetLocalCanBeIgnited(false);
                otherFlammable.GetComponent<FlammableBehaviour>().SelfIgnite(flammable);
            }
        }

        private void IgniteUpdate()
        {
            if (!flammable.Data.isOnFire)
            {
                var update = new Flammable.Update();
                update.SetIsOnFire(true).SetCanBeIgnited(false);
                flammable.Send(update);
            }
        }

        private void ExtinguishUpdate(bool canBeIgnited)
        {
            if (flammable.Data.isOnFire)
            {
                var update = new Flammable.Update();
                update.SetIsOnFire(false).SetCanBeIgnited(canBeIgnited);
                flammable.Send(update);
            }
        }
    }
}
