using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Abilities;
using Improbable.Building;
using Improbable.Collections;
using Improbable.Communication;
using Improbable.Core;
using Improbable.Fire;
using Improbable.Global;
using Improbable.Life;
using Improbable.Math;
using Improbable.Npc;
using Improbable.Player;
using Improbable.Team;
using Improbable.Tree;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;

namespace Assets.Gamelogic.EntityTemplate
{
    public static class EntityTemplateFactory
    {
        public static Entity CreatePlayerTemplate(string clientWorkerId, Coordinates initialPosition, uint teamId)
        {
            var template = new Entity();
            template.Add(new ClientAuthorityCheck.Data());
            template.Add(new FSimAuthorityCheck.Data());
            template.Add(new TransformComponent.Data(initialPosition, 0));
            template.Add(new PlayerInfo.Data(true, initialPosition));
            template.Add(new PlayerControls.Data(initialPosition));
            template.Add(new Health.Data(SimulationSettings.PlayerMaxHealth, SimulationSettings.PlayerMaxHealth, true));
            template.Add(new Flammable.Data(false, true, FireEffectType.SMALL));
            template.Add(new Spells.Data(new Map<SpellType, float> { { SpellType.LIGHTNING, 0f }, { SpellType.RAIN, 0f } }, true));
            template.Add(new Inventory.Data(0));
            template.Add(new Chat.Data());
            template.Add(new Heartbeat.Data(SimulationSettings.HeartbeatMax));
            template.Add(new TeamAssignment.Data(teamId));
            template.Add(new Flammable.Data(false, true, FireEffectType.SMALL));

            var specificClientPredicate = CommonPredicates.SpecificClientOnly(clientWorkerId);

            var permissions = Acl.Build()
                .SetReadAccess(CommonPredicates.PhysicsOrVisual)
                .SetWriteAccess<ClientAuthorityCheck>(specificClientPredicate)
                .SetWriteAccess<FSimAuthorityCheck>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<TransformComponent>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<PlayerInfo>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<PlayerControls>(specificClientPredicate)
                .SetWriteAccess<Health>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Spells>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Inventory>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Chat>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Heartbeat>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<TeamAssignment>(CommonPredicates.PhysicsOnly);

            template.SetAcl(permissions);

            return template;
        }
        
        public static SnapshotEntity CreateBarracksTemplate(Coordinates initialPosition, BarracksState barracksState, uint teamId)
        {
            var template = new SnapshotEntity { Prefab = SimulationSettings.BarracksPrefabName };
            template.Add(new FSimAuthorityCheck.Data());
            template.Add(new TransformComponent.Data(initialPosition, (uint)(UnityEngine.Random.value * 360)));
            template.Add(new BarracksInfo.Data(barracksState));
            template.Add(new Health.Data(barracksState == BarracksState.CONSTRUCTION_FINISHED ? SimulationSettings.BarracksMaxHealth : 0, SimulationSettings.BarracksMaxHealth, true));
            template.Add(new Flammable.Data(false, false, FireEffectType.BIG));
            template.Add(new StockpileDepository.Data(barracksState == BarracksState.UNDER_CONSTRUCTION));
            template.Add(new NPCSpawner.Data(barracksState == BarracksState.CONSTRUCTION_FINISHED, new Map<NPCRole, float> { { NPCRole.LUMBERJACK, 0f }, { NPCRole.WIZARD, 0f } }));
            template.Add(new TeamAssignment.Data(teamId));

            var permissions = Acl.Build()
                .SetReadAccess(CommonPredicates.PhysicsOrVisual)
                .SetWriteAccess<FSimAuthorityCheck>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<TransformComponent>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<BarracksInfo>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Health>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<StockpileDepository>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<NPCSpawner>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<TeamAssignment>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonPredicates.PhysicsOnly);
            
            template.SetAcl(permissions);

            return template;
        }

        public static SnapshotEntity CreateTreeTemplate(Coordinates initialPosition, uint initialRotation)
        {
            var template = new SnapshotEntity { Prefab = SimulationSettings.TreePrefabName };
            template.Add(new FSimAuthorityCheck.Data());
            template.Add(new TransformComponent.Data(initialPosition, initialRotation));
            template.Add(new Harvestable.Data());
            template.Add(new Health.Data(SimulationSettings.TreeMaxHealth, SimulationSettings.TreeMaxHealth, true));
            template.Add(new Flammable.Data(false, true, FireEffectType.BIG));
            template.Add(new TreeState.Data((TreeType)UnityEngine.Random.Range(0, 2), TreeFSMState.HEALTHY));

            var permissions = Acl.Build()
                .SetReadAccess(CommonPredicates.PhysicsOrVisual)
                .SetWriteAccess<FSimAuthorityCheck>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<TransformComponent>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Harvestable>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Health>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<TreeState>(CommonPredicates.PhysicsOnly);

            template.SetAcl(permissions);

            return template;
        }

        public static SnapshotEntity CreateNPCLumberjackTemplate(Coordinates initialPosition, uint teamId)
        {
            var template = new SnapshotEntity { Prefab = SimulationSettings.NPCPrefabName };
            template.Add(new TransformComponent.Data(initialPosition, 0));
            template.Add(new FSimAuthorityCheck.Data());
            template.Add(new Health.Data(SimulationSettings.LumberjackMaxHealth, SimulationSettings.LumberjackMaxHealth, true));
            template.Add(new Flammable.Data(false, true, FireEffectType.SMALL));
            template.Add(new TargetNavigation.Data(NavigationState.INACTIVE, Vector3f.ZERO, EntityId.InvalidEntityId, 0f));
            template.Add(new Inventory.Data(0));
            template.Add(new NPCLumberjack.Data(LumberjackFSMState.StateEnum.IDLE, EntityId.InvalidEntityId, SimulationSettings.InvalidPosition.ToVector3f()));
			template.Add(new TeamAssignment.Data(teamId));
            
            var permissions = Acl.Build()
                .SetReadAccess(CommonPredicates.PhysicsOrVisual)
                .SetWriteAccess<TransformComponent>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<FSimAuthorityCheck>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Health>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<TargetNavigation>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Inventory>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<NPCLumberjack>(CommonPredicates.PhysicsOnly)
				.SetWriteAccess<TeamAssignment>(CommonPredicates.PhysicsOnly);

            template.SetAcl(permissions);

            return template;
        }

        public static SnapshotEntity CreateNPCWizardTemplate(Coordinates initialPosition, uint teamId)
        {
            var template = new SnapshotEntity { Prefab = SimulationSettings.NPCWizardPrefabName };
            template.Add(new TransformComponent.Data(initialPosition, 0));
            template.Add(new FSimAuthorityCheck.Data());
            template.Add(new Health.Data(SimulationSettings.WizardMaxHealth, SimulationSettings.WizardMaxHealth, true));
            template.Add(new Flammable.Data(false, true, FireEffectType.SMALL));
            template.Add(new TargetNavigation.Data(NavigationState.INACTIVE, Vector3f.ZERO, EntityId.InvalidEntityId, 0f));
            template.Add(new Spells.Data(new Map<SpellType, float> { { SpellType.LIGHTNING, 0f }, { SpellType.RAIN, 0f } }, true));
            template.Add(new NPCWizard.Data(WizardFSMState.StateEnum.IDLE, EntityId.InvalidEntityId, SimulationSettings.InvalidPosition.ToVector3f()));
            template.Add(new TeamAssignment.Data(teamId));
            
            var permissions = Acl.Build()
                .SetReadAccess(CommonPredicates.PhysicsOrVisual)
                .SetWriteAccess<TransformComponent>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<FSimAuthorityCheck>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Health>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<TargetNavigation>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Spells>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<NPCWizard>(CommonPredicates.PhysicsOnly)
				.SetWriteAccess<TeamAssignment>(CommonPredicates.PhysicsOnly);

            template.SetAcl(permissions);

            return template;
        }

        public static SnapshotEntity CreateHQTemplate(Coordinates initialPosition, uint initialRotation, uint teamId)
        {
            var template = new SnapshotEntity { Prefab = SimulationSettings.HQPrefabName };
            template.Add(new FSimAuthorityCheck.Data());
            template.Add(new HQInfo.Data(new List<EntityId>()));
            template.Add(new TransformComponent.Data(initialPosition, initialRotation));
            template.Add(new Health.Data(SimulationSettings.HQMaxHealth, SimulationSettings.HQMaxHealth, true));
            template.Add(new TeamAssignment.Data(teamId));
            template.Add(new Flammable.Data(false, true, FireEffectType.BIG));

            var permissions = Acl.Build()
                .SetReadAccess(CommonPredicates.PhysicsOrVisual)
                .SetWriteAccess<FSimAuthorityCheck>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<HQInfo>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<TransformComponent>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Health>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<TeamAssignment>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonPredicates.PhysicsOnly);

            template.SetAcl(permissions);

            return template;
        }

        public static SnapshotEntity CreateSimulationManagerTemplate()
        {
            var template = new SnapshotEntity { Prefab = SimulationSettings.SimulationManagerEntityName };
            template.Add(new TransformComponent.Data(Coordinates.ZERO, 0));
            template.Add(new FSimAuthorityCheck.Data());
            template.Add(new PlayerLifeCycle.Data(new Map<string, EntityId>()));

            var permissions = Acl.Build()
                .SetReadAccess(CommonPredicates.PhysicsOrVisual)
                .SetWriteAccess<TransformComponent>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<FSimAuthorityCheck>(CommonPredicates.PhysicsOnly)
                .SetWriteAccess<PlayerLifeCycle>(CommonPredicates.PhysicsOnly);
				
            template.SetAcl(permissions);

            return template;
        }
    }
}
