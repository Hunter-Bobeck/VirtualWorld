using Assets.Gamelogic.FSM;
using Improbable.Fire;
using Improbable.Life;
using Improbable.Grass;

namespace Assets.Gamelogic.Grass
{
    public class GrassBurningState : FsmBaseState<GrassStateMachine, GrassFSMState>
    {
        private readonly Flammable.Writer flammable;
        private readonly Health.Writer health;

        public GrassBurningState(GrassStateMachine owner, Flammable.Writer inFlammable, Health.Writer inHealth) 
            : base(owner)
        {
            flammable = inFlammable;
            health = inHealth;
        }

        public override void Enter()
        {
            flammable.Send(new Flammable.Update().SetCanBeIgnited(false));

            flammable.ComponentUpdated += OnFlammableUpdated;
            health.ComponentUpdated += OnHealthUpdated;
        }

        public override void Tick()
        {

        }

        public override void Exit(bool disabled)
        {
            health.ComponentUpdated -= OnHealthUpdated;
            flammable.ComponentUpdated -= OnFlammableUpdated;
        }

        private void OnHealthUpdated(Health.Update update)
        {
            if (update.currentHealth.HasValue && update.currentHealth.Value <= 0)
            {
                Owner.TriggerTransition(GrassFSMState.BURNT);
            }
        }

        private void OnFlammableUpdated(Flammable.Update update)
        {
            if (HasBeenExtinguished(update))
            {
                Owner.TriggerTransition(GrassFSMState.UNEATEN);
            }
        }

        private bool HasBeenExtinguished(Flammable.Update flammableUpdate)
        {
            return flammableUpdate.isOnFire.HasValue && !flammableUpdate.isOnFire.Value;
        }
    }
}
