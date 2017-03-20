using Assets.Gamelogic.Fire;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.Life;
using Improbable.Fire;
using Improbable.Grass;
using System.Collections.Generic;
using Improbable.Life;
using UnityEngine;

namespace Assets.Gamelogic.Grass
{
    public class GrassStateMachine : FiniteStateMachine<GrassFSMState>
    {
        private readonly GrassState.Writer grass;
        public GrassStateData Data;

        public GrassStateMachine(
              GrassBehaviour owner,
              GrassState.Writer inGrass,
              Health.Writer health,
              FlammableBehaviour flammableInterface,
              Flammable.Writer flammable
        )
        {
            grass = inGrass;

            var uneatenState = new GrassUneatenState(this, flammable, health);
            var burningState = new GrassBurningState(this, flammable, health);
            var burntState = new GrassBurntState(this, owner, flammable, flammableInterface);
            var eatenState = new GrassEatenState(this, owner, flammable);

            var stateList = new Dictionary<GrassFSMState, IFsmState>();
            stateList.Add(GrassFSMState.UNEATEN, uneatenState);
            stateList.Add(GrassFSMState.BURNING, burningState);
            stateList.Add(GrassFSMState.BURNT, burntState);
            stateList.Add(GrassFSMState.EATEN, eatenState);

            SetStates(stateList);

            var allowedTransitions = new Dictionary<GrassFSMState, IList<GrassFSMState>>();

            allowedTransitions.Add(GrassFSMState.UNEATEN, new List<GrassFSMState>()
            {
                GrassFSMState.BURNING,
                GrassFSMState.EATEN
            });

            allowedTransitions.Add(GrassFSMState.BURNING, new List<GrassFSMState>()
            {
                GrassFSMState.UNEATEN,
                GrassFSMState.BURNT
            });

            allowedTransitions.Add(GrassFSMState.BURNT, new List<GrassFSMState>()
            {
                GrassFSMState.UNEATEN
            });

            allowedTransitions.Add(GrassFSMState.EATEN, new List<GrassFSMState>()
            {
                GrassFSMState.UNEATEN
            });

            SetTransitions(allowedTransitions);
        }

        protected override void OnEnableImpl()
        {
            Data = grass.Data.DeepCopy();
        }

        public void TriggerTransition(GrassFSMState newState)
        {
            if (grass == null)
            {
                Debug.LogError("Trying to change state without authority.");
                return;
            }

            if (IsValidTransition(newState))
            {
                Data.currentState = newState;

                var update = new GrassState.Update();
                update.SetCurrentState(Data.currentState);
                grass.Send(update);

                TransitionTo(newState);
            }
            else
            {
                Debug.LogErrorFormat("Grass: Invalid transition from {0} to {1} detected.", Data.currentState, newState);
            }
        }
    }

}
