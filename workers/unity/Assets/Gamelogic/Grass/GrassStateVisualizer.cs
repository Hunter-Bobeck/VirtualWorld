using Improbable.Grass;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Grass
{
    public class GrassStateVisualizer : MonoBehaviour
    {
        [Require] private GrassState.Reader grassState;
        public GrassFSMState CurrentState { get { return grassState.Data.currentState; } }
    }
}
