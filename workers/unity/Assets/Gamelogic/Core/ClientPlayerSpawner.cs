using Improbable;
using Improbable.Core;
using Improbable.Global;
using Improbable.Unity.Core;
using Improbable.Unity.Core.EntityQueries;
using Improbable.Worker;
using Improbable.Worker.Query;
using System;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    /// <summary>
    /// Helper class for handling spawning and deleting of the player character
    /// </summary>
    public static class ClientPlayerSpawner
    {
        public static EntityId SimulationManagerEntityId = EntityId.InvalidEntityId;

        public static void SpawnPlayer()
        {
            FindSimulationManagerEntityId(RequestPlayerSpawn);
        }

        public static void DeletePlayer()
        {
            if (EntityId.IsValidEntityId(SimulationManagerEntityId))
            {
                SpatialOS.Connection.SendCommandRequest(SimulationManagerEntityId, new PlayerLifeCycle.Commands.DeletePlayer.Request(new DeletePlayerRequest()), null);
            }
        }

        private static void FindSimulationManagerEntityId(Action<EntityId> callback)
        {
		    if (EntityId.IsValidEntityId(SimulationManagerEntityId))
		    {
                callback.Invoke(SimulationManagerEntityId);
		        return;
		    }

            var entityQuery = Query.HasComponent<PlayerLifeCycle>().ReturnOnlyEntityIds();
            SpatialOS.WorkerCommands.SendQuery(entityQuery, response => OnSearchResult(callback, response));
        }

        private static void OnSearchResult(Action<EntityId> callback,
                                           ICommandCallbackResponse<EntityQueryResult> response)
        {
            if (!response.Response.HasValue || response.StatusCode != StatusCode.Success)
            {
                Debug.LogError("Find player spawner query failed with error: " + response.ErrorMessage);
                return;
            }

            var result = response.Response.Value;
            if (result.EntityCount < 1)
            {
                Debug.LogError("Failed to find player spawner: no entities found with the PlayerSpawner component.");
                return;
            }

            SimulationManagerEntityId = result.Entities.First.Value.Key;
            callback(SimulationManagerEntityId);
        }

        private static void RequestPlayerSpawn(EntityId simulationManagerEntityId)
        {
            SpatialOS.WorkerCommands.SendCommand(PlayerLifeCycle.Commands.SpawnPlayer.Descriptor, new SpawnPlayerRequest(), simulationManagerEntityId, 
                response => OnSpawnPlayerResponse(simulationManagerEntityId, response));
        }

        private static void OnSpawnPlayerResponse(EntityId simulationManagerEntityId, ICommandCallbackResponse<Nothing> response)
        {
            if (!response.Response.HasValue || response.StatusCode != StatusCode.Success)
            {
                Debug.LogError("SpawnPlayer command failed: " + response.ErrorMessage);
                RequestPlayerSpawn(simulationManagerEntityId);
                return;
            }
        }
    }
}
