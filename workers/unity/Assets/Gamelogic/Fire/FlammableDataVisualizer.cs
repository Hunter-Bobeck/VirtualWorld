using System;
using Improbable.Fire;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Fire
{
    [EngineType(EnginePlatform.FSim)]
    public class FlammableDataVisualizer : MonoBehaviour
    {
        [Require] private Flammable.Reader flammable;
        public bool canBeIgnited { get; private set; }

        void OnEnable()
        {
            flammable.ComponentUpdated += FlammableOnComponentUpdated;
            canBeIgnited = flammable.Data.canBeIgnited;
        }

        void OnDisable()
        {
            flammable.ComponentUpdated -= FlammableOnComponentUpdated;
            canBeIgnited = false;
        }

        private void FlammableOnComponentUpdated(Flammable.Update update)
        {
            canBeIgnited = flammable.Data.canBeIgnited;
        }

        public void SetLocalCanBeIgnited(bool ignitable)
        {
            canBeIgnited = ignitable;
        }
    }
}
