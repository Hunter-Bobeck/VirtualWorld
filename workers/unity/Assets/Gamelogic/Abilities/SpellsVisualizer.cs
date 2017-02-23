using Assets.Gamelogic.Utils;
using Improbable.Abilities;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Abilities
{
    [EngineType(EnginePlatform.Client)]
    public class SpellsVisualizer : MonoBehaviour
    {
        [Require] private Spells.Reader spells;

        private void OnEnable()
        {
            spells.ComponentUpdated += OnComponentUpdated;
        }

        private void OnDisable()
        {
            spells.ComponentUpdated -= OnComponentUpdated;
        }

        private void OnComponentUpdated(Spells.Update update)
        {
            for (var i = 0; i < update.spellAnimationEvent.Count; i++)
            {
                SpellsVisualizerPool.ShowSpellEffect(update.spellAnimationEvent[i].position.ToVector3(), update.spellAnimationEvent[i].spellType);
            }
        }
    }
}
