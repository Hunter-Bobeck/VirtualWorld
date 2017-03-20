using System.Collections;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Fire;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.Utils;
using Improbable.Fire;
using Improbable.Grass;
using UnityEngine;

namespace Assets.Gamelogic.Grass
{
    public class GrassBurntState : FsmBaseState<GrassStateMachine, GrassFSMState>
    {
        private readonly GrassBehaviour parentBehaviour;
        private readonly Flammable.Writer flammable;
        private readonly FlammableBehaviour flammableInterface;

        private Coroutine regrowingCoroutine;

        public GrassBurntState(GrassStateMachine owner, GrassBehaviour inParentBehaviour, Flammable.Writer inFlammable, FlammableBehaviour inFlammableInterface) : base(owner)
        {
            parentBehaviour = inParentBehaviour;
            flammable = inFlammable;
            flammableInterface = inFlammableInterface;
        }

        public override void Enter()
        {
            flammableInterface.SelfExtinguish(flammable, false);
            if (regrowingCoroutine == null)
            {
                regrowingCoroutine = parentBehaviour.StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.BurntGrassRegrowthTimeSecs, Regrow));
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
