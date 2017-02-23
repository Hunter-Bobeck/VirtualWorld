using Assets.Gamelogic.Core;
using Assets.Gamelogic.EntityTemplate;
using Assets.Gamelogic.Utils;
using Improbable.Building;
using Improbable.Collections;
using Improbable.Math;
using Improbable.Npc;
using Improbable.Team;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Gamelogic.Building
{
    public class NPCSpawnerBehaviour : MonoBehaviour
    {
        [Require] private NPCSpawner.Writer npcSpawner;
        [Require] private TeamAssignment.Reader teamAssignment;

        private Coroutine spawnNPCsReduceCooldownCoroutine;

        private static readonly IDictionary<NPCRole, float> npcRolesToCooldownDictionary = new Dictionary<NPCRole, float>
        {
            { NPCRole.LUMBERJACK, SimulationSettings.LumberjackSpawningCooldown },
            { NPCRole.WIZARD, SimulationSettings.WizardSpawningCooldown }
        };

        private void OnEnable()
        {
            var npcRoles = new System.Collections.Generic.List<NPCRole>(npcRolesToCooldownDictionary.Keys);

            spawnNPCsReduceCooldownCoroutine = StartCoroutine(TimerUtils.CallRepeatedly(SimulationSettings.SimulationTickInterval, () =>
            {
                ReduceSpawnCooldown(npcRoles, SimulationSettings.SimulationTickInterval);
            }));
        }

        private void OnDisable()
        {
            CancelSpawnNPCsReduceCooldownCoroutine();
        }

        private void CancelSpawnNPCsReduceCooldownCoroutine()
        {
            if (spawnNPCsReduceCooldownCoroutine != null)
            {
                StopCoroutine(spawnNPCsReduceCooldownCoroutine);
                spawnNPCsReduceCooldownCoroutine = null;
            }
        }

        private void ReduceSpawnCooldown(IList<NPCRole> npcRoles, float interval)
        {
            if (!npcSpawner.Data.spawningEnabled)
            {
                return;
            }

            var newCooldowns = new Map<NPCRole, float>(npcSpawner.Data.cooldowns);

            for (var i = 0; i < npcRoles.Count; i++)
            {
                var role = npcRoles[i];
                if (newCooldowns[role] <= 0f) // todo: this is a workaround for WIT-1374
                {
                    var spawningOffset = new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f) * SimulationSettings.SpawnOffsetFactor;
                    var spawnPosition = (gameObject.transform.position + spawningOffset).ToCoordinates();
                    SpawnNpc(role, spawnPosition);
                    newCooldowns[role] = npcRolesToCooldownDictionary[role];
                }
                else
                {
                    newCooldowns[role] = Mathf.Max(newCooldowns[role] - interval, 0f);
                }
            }
            npcSpawner.Send(new NPCSpawner.Update().SetCooldowns(newCooldowns));
        }

        public void SetSpawningEnabled(bool spawningEnabled)
        {
            if (spawningEnabled != npcSpawner.Data.spawningEnabled)
            {
                npcSpawner.Send(new NPCSpawner.Update().SetSpawningEnabled(spawningEnabled));
            }
        }

        private void SpawnNpc(NPCRole npcRoleEnum, Coordinates position)
        {
            switch (npcRoleEnum)
            {
                case NPCRole.LUMBERJACK:
                    SpawnLumberjack(position);
                    break;
                case NPCRole.WIZARD:
                    SpawnWizard(position);
                    break;
            }
        }

        private void SpawnLumberjack(Coordinates position)
        {
            var template = EntityTemplateFactory.CreateNPCLumberjackTemplate(position, teamAssignment.Data.teamId);
            SpatialOS.Commands.CreateEntity(npcSpawner, SimulationSettings.NPCPrefabName, template, result => { });
        }

        private void SpawnWizard(Coordinates position)
        {
            var template = EntityTemplateFactory.CreateNPCWizardTemplate(position, teamAssignment.Data.teamId);
            SpatialOS.Commands.CreateEntity(npcSpawner, SimulationSettings.NPCWizardPrefabName, template, result => { });
        }

        public void ResetCooldowns()
        {
            npcSpawner.Send(new NPCSpawner.Update().SetCooldowns(new Map<NPCRole, float> { { NPCRole.LUMBERJACK, 0f }, { NPCRole.WIZARD, 0f } }));
        }
    }
}
