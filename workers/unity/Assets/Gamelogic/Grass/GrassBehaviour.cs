using Assets.Gamelogic.Core;
using Assets.Gamelogic.Fire;
using Assets.Gamelogic.Life;
using Improbable.Fire;
using Improbable.Life;
using Improbable.Grass;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Grass
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class GrassBehaviour : MonoBehaviour
    {
        [Require] private GrassState.Writer grass;
        [Require] private Flammable.Writer flammable;
        [Require] private Health.Writer health;

        [SerializeField] private FlammableBehaviour flammableInterface;

        private GrassStateMachine stateMachine;

        private void Awake()
        {
            flammableInterface = gameObject.GetComponentIfUnassigned(flammableInterface);
        }

        private void OnEnable()
        {
            stateMachine = new GrassStateMachine(this, 
                grass,
                health,
                flammableInterface,
                flammable);

            stateMachine.OnEnable(grass.Data.currentState);
        }

        private void OnDisable()
        {
            stateMachine.OnDisable();
        }

    }
}
