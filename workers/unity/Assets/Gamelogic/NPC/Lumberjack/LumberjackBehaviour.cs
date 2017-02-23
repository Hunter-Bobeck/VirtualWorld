using Assets.Gamelogic.Core;
using Assets.Gamelogic.Fire;
using Assets.Gamelogic.NPC.LumberJack;
using Improbable;
using Improbable.Core;
using Improbable.Npc;
using Improbable.Team;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Lumberjack
{
    [EngineType(EnginePlatform.FSim)]
    public class LumberjackBehaviour : MonoBehaviour, IFlammable
    {
        [Require] private NPCLumberjack.Writer npcLumberjack;
        [Require] private TargetNavigation.Writer targetNavigation;
        [Require] private Inventory.Writer inventory;
        [Require] private TeamAssignment.Reader teamAssignment;

        [SerializeField] private TargetNavigationBehaviour navigation;
        
        private LumberjackStateMachine stateMachine;

        private void Awake()
        {
            navigation = gameObject.GetComponentIfUnassigned(navigation);
        }

        private void OnEnable()
        {
            stateMachine = new LumberjackStateMachine(this, navigation, inventory, targetNavigation, npcLumberjack, teamAssignment);
            stateMachine.OnEnable(npcLumberjack.Data.currentState);
        }

        private void OnDisable()
        {
            stateMachine.OnDisable();
        }

        public void OnIgnite()
        {
            stateMachine.TriggerTransition(LumberjackFSMState.StateEnum.ON_FIRE, EntityId.InvalidEntityId, SimulationSettings.InvalidPosition);
        }

        public void OnExtinguish()
        {
            stateMachine.TriggerTransition(LumberjackFSMState.StateEnum.IDLE, EntityId.InvalidEntityId, SimulationSettings.InvalidPosition);
        }
    }
}
