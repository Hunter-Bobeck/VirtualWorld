using Assets.Gamelogic.Building;
using Assets.Gamelogic.Fire;
using Assets.Gamelogic.Life;
using Assets.Gamelogic.Team;
using Assets.Gamelogic.Tree;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Building;
using Improbable.Tree;
using Improbable.Unity.Core;
using System;
using UnityEngine;

namespace Assets.Gamelogic.NPC
{
    public static class NPCUtils
    {
        private static Collider[] nearbyColliders = new Collider[32];

        public static bool TargetExistsLocally(EntityId targetEntityId)
        {
            return SpatialOS.Universe.ContainsEntity(targetEntityId);
        }

        public static GameObject GetTargetGameObject(EntityId targetEntityId)
        {
            if (!TargetExistsLocally(targetEntityId))
            {
                return null;
            }
            return SpatialOS.Universe.Get(targetEntityId).UnderlyingGameObject;
        }

        public static bool IsWithinInteractionRange(Vector3 currentPosition, Vector3 targetPosition, float interactionSqrDistance)
        {
            return MathUtils.SqrDistance(currentPosition, targetPosition) <= interactionSqrDistance;
        }

        public static GameObject FindNearestTarget(GameObject referenceGameObject, float radius, Func<GameObject, GameObject, bool> conditionForSuccess, int layerMask)
        {
            var currentPosition = referenceGameObject.transform.position;
            var gameObjectCount = Physics.OverlapSphereNonAlloc(currentPosition, radius, nearbyColliders, layerMask);

            GameObject closestTarget = null;
            var minimumDistanceFound = Mathf.Infinity;

            for (var nearbyColliderIndex = 0; nearbyColliderIndex < gameObjectCount; nearbyColliderIndex++)
            {
                var targetObject = nearbyColliders[nearbyColliderIndex].gameObject;
                if (!targetObject.IsEntityObject())
                {
                    continue;
                }

                var distance = (targetObject.transform.position - currentPosition).sqrMagnitude;
                if (distance < minimumDistanceFound && conditionForSuccess(referenceGameObject, targetObject))
                {
                    minimumDistanceFound = distance;
                    closestTarget = targetObject;
                }
            }
            return closestTarget;
        }

        public static bool IsTargetAttackable(GameObject reference, GameObject target)
        {
            var teamAssignment = reference.GetComponent<TeamAssignmentVisualizerFSim>();
            if (teamAssignment == null)
            {
                Debug.LogError("Failed to find TeamAssignmentVisualizerFSim in IsTargetAttackable.");
                return false;
            }
            var targetTeamAssignment = target.GetComponent<TeamAssignmentVisualizerFSim>();
            var targetFlammable = target.GetComponent<FlammableBehaviour>();
            var targetHealth = target.GetComponent<HealthVisualizer>();

            return targetTeamAssignment != null &&
                   teamAssignment.TeamId != targetTeamAssignment.TeamId &&
                   targetFlammable != null &&
                   !targetFlammable.IsOnFire &&
                   targetHealth != null && 
                   targetHealth.CurrentHealth > 0;
        }

        public static bool IsTargetDefendable(GameObject reference, GameObject target)
        {
            var teamAssignment = reference.GetComponent<TeamAssignmentVisualizerFSim>();
            if (teamAssignment == null)
            {
                Debug.LogError("Failed to find TeamAssignmentVisualizerFSim in IsTargetDefendable.");
                return false;
            }
            var targetTeamAssignment = target.GetComponent<TeamAssignmentVisualizerFSim>();
            var targetFlammable = target.GetComponent<FlammableBehaviour>();
            var targetHealth = target.GetComponent<HealthVisualizer>();
            
            return targetTeamAssignment != null &&
                   teamAssignment.TeamId == targetTeamAssignment.TeamId &&
                   targetFlammable != null && 
                   targetFlammable.IsOnFire &&
                   targetHealth != null && 
                   targetHealth.CurrentHealth > 0;
        }

        public static bool IsTargetATeamStockpile(GameObject reference, GameObject target)
        {
            var teamAssignment = reference.GetComponent<TeamAssignmentVisualizerFSim>();
            if (teamAssignment == null)
            {
                Debug.LogError("Failed to find TeamAssignmentVisualizerFSim in IsTargetATeamStockpile.");
                return false;
            }
            var targetBarracksInfoVisualizer = target.GetComponent<BarracksInfoVisualizer>();
            var targetTeamAssignmentVisualizer = target.GetComponent<TeamAssignmentVisualizerFSim>();

            return targetBarracksInfoVisualizer != null && 
                   targetTeamAssignmentVisualizer != null && 
                   targetBarracksInfoVisualizer.BarracksState == BarracksState.UNDER_CONSTRUCTION &&
                   teamAssignment.TeamId == targetTeamAssignmentVisualizer.TeamId;
        }

        public static bool IsTargetAHealthyTree(GameObject reference, GameObject target)
        {
            var targetTreeStateVisualizer = target.GetComponent<TreeStateVisualizer>();
            var targetHealthVisualizer = target.GetComponent<HealthVisualizer>();
            return targetTreeStateVisualizer != null && 
                   targetHealthVisualizer != null &&
                   targetTreeStateVisualizer.CurrentState == TreeFSMState.HEALTHY &&
                   targetHealthVisualizer.CurrentHealth > 0;
        }

        public static void NavigateToRandomNearbyPosition(TargetNavigationBehaviour navigation, Vector3 currentPosition, float maxDistance, float interactionSqrDistance)
        {
            var targetPosition = currentPosition + (UnityEngine.Random.insideUnitSphere * maxDistance).FlattenVector();
            navigation.StartNavigation(targetPosition, interactionSqrDistance);
        }
    }
}
