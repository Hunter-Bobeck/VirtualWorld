using Assets.Gamelogic.Core;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.Life;
using Improbable.Grass;
using Improbable.Fire;
using Improbable.Life;
using Assets.Gamelogic.ComponentExtensions;

namespace Assets.Gamelogic.Grass
{
    public class GrassUneatenState : FsmBaseState<GrassStateMachine, GrassFSMState>
    {
        private readonly Flammable.Writer flammable;
        private readonly Health.Writer health;

        public GrassUneatenState(GrassStateMachine owner, Flammable.Writer inFlammable, Health.Writer inHealth) 
            : base(owner)
        {
            flammable = inFlammable;
            health = inHealth;
        }

        public override void Enter()
        {
            health.SetCurrentHealth(SimulationSettings.GrassMaxHealth);
            flammable.Send(new Flammable.Update().SetCanBeIgnited(true));

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
                Owner.TriggerTransition(GrassFSMState.EATEN);
            }
        }

        private void OnFlammableUpdated(Flammable.Update update)
        {
            if (HasBeenIgnited(update))
            {
                Owner.TriggerTransition(GrassFSMState.BURNING);
            }
        }

        private bool HasBeenIgnited(Flammable.Update flammableUpdate)
        {
            return flammableUpdate.isOnFire.HasValue && flammableUpdate.isOnFire.Value;
        }
    }
}
