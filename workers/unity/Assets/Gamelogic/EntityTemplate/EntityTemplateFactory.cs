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
using Improbable.Grass;
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

            var specificClientPredicate = CommonRequirementSets.SpecificClientOnly(clientWorkerId);

            var permissions = Acl.Build()
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<ClientAuthorityCheck>(specificClientPredicate)
                .SetWriteAccess<FSimAuthorityCheck>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<TransformComponent>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<PlayerInfo>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<PlayerControls>(specificClientPredicate)
                .SetWriteAccess<Health>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Spells>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Inventory>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Chat>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Heartbeat>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<TeamAssignment>(CommonRequirementSets.PhysicsOnly);

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
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<FSimAuthorityCheck>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<TransformComponent>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<BarracksInfo>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Health>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<StockpileDepository>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<NPCSpawner>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<TeamAssignment>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonRequirementSets.PhysicsOnly);

            template.SetAcl(permissions);

            return template;
        }

        public static SnapshotEntity CreateGrassTemplate(Coordinates initialPosition, uint initialRotation)
        {
            var template = new SnapshotEntity { Prefab = SimulationSettings.GrassPrefabName };
            template.Add(new FSimAuthorityCheck.Data());
            template.Add(new TransformComponent.Data(initialPosition, initialRotation));
            template.Add(new Harvestable.Data());
            template.Add(new Health.Data(SimulationSettings.GrassMaxHealth, SimulationSettings.GrassMaxHealth, true));
            template.Add(new Flammable.Data(false, true, FireEffectType.BIG));
            template.Add(new GrassState.Data((GrassType)UnityEngine.Random.Range(0, 2), GrassFSMState.UNEATEN));

            var permissions = Acl.Build()
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<FSimAuthorityCheck>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<TransformComponent>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Harvestable>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Health>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<GrassState>(CommonRequirementSets.PhysicsOnly);

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
            template.Add(new TargetNavigation.Data(NavigationState.INACTIVE, Vector3f.ZERO, new EntityId(), 0f));
            template.Add(new Inventory.Data(0));
            template.Add(new NPCLumberjack.Data(LumberjackFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition.ToVector3f()));
			template.Add(new TeamAssignment.Data(teamId));

            var permissions = Acl.Build()
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<TransformComponent>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<FSimAuthorityCheck>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Health>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<TargetNavigation>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Inventory>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<NPCLumberjack>(CommonRequirementSets.PhysicsOnly)
				.SetWriteAccess<TeamAssignment>(CommonRequirementSets.PhysicsOnly);

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
            template.Add(new TargetNavigation.Data(NavigationState.INACTIVE, Vector3f.ZERO, new EntityId(), 0f));
            template.Add(new Spells.Data(new Map<SpellType, float> { { SpellType.LIGHTNING, 0f }, { SpellType.RAIN, 0f } }, true));
            template.Add(new NPCWizard.Data(WizardFSMState.StateEnum.IDLE, new EntityId(), SimulationSettings.InvalidPosition.ToVector3f()));
            template.Add(new TeamAssignment.Data(teamId));

            var permissions = Acl.Build()
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<TransformComponent>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<FSimAuthorityCheck>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Health>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<TargetNavigation>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Spells>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<NPCWizard>(CommonRequirementSets.PhysicsOnly)
				.SetWriteAccess<TeamAssignment>(CommonRequirementSets.PhysicsOnly);

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
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<FSimAuthorityCheck>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<HQInfo>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<TransformComponent>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Health>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<TeamAssignment>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Flammable>(CommonRequirementSets.PhysicsOnly);

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
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<TransformComponent>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<FSimAuthorityCheck>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<PlayerLifeCycle>(CommonRequirementSets.PhysicsOnly);

            template.SetAcl(permissions);

            return template;
        }
    }
}
