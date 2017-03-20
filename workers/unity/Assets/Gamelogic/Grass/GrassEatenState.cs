using Assets.Gamelogic.Core;
using Assets.Gamelogic.Fire;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.Utils;
using Improbable.Fire;
using Improbable.Grass;
using UnityEngine;

namespace Assets.Gamelogic.Grass
{
    public class GrassEatenState : FsmBaseState<GrassStateMachine, GrassFSMState>
    {
        private readonly GrassBehaviour parentBehaviour;
        private readonly Flammable.Writer flammable;
        private readonly FlammableBehaviour flammableInterface;

        private Coroutine regrowingCoroutine;

        public GrassEatenState(GrassStateMachine owner, GrassBehaviour inParentBehaviour, Flammable.Writer inFlammable) : base(owner)
        {
            parentBehaviour = inParentBehaviour;
            flammable = inFlammable;
        }

        public override void Enter()
        {
            flammable.Send(new Flammable.Update().SetCanBeIgnited(false));

            if (regrowingCoroutine == null)
            {
                regrowingCoroutine = parentBehaviour.StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.GrassEatenRegrowthTimeSecs, Regrow));
            }
        }

        private void Regrow()
        {
            Owner.TriggerTransition(GrassFSMState.UNEATEN);
        }

        public override void Tick()
        {

        }

        public override void Exit(bool disabled)
        {
            if (regrowingCoroutine != null)
            {
                parentBehaviour.StopCoroutine(regrowingCoroutine);
                regrowingCoroutine = null;
            }
        }
    }
}
