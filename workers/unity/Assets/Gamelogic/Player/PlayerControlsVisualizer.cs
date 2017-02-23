using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    [EngineType(EnginePlatform.FSim)]
    public class PlayerControlsVisualizer : MonoBehaviour
    {
        [Require] private FSimAuthorityCheck.Writer fsimAuthorityCheck;
        [Require] private PlayerInfo.Reader playerInfo;
        [Require] private PlayerControls.Reader playerControls;

        public Vector3 TargetPosition { get { return playerControls.Data.targetPosition.ToVector3(); } }

        [SerializeField] private Rigidbody myRigidbody;
        [SerializeField] private TransformSender transformSender;

        private void Awake()
        {
            transformSender = gameObject.GetComponentIfUnassigned(transformSender);
            myRigidbody = gameObject.GetComponentIfUnassigned(myRigidbody);
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }
        
        private void MovePlayer()
        {
            if (ShouldMovePlayerFSim(TargetPosition, myRigidbody.position))
            {
                if (PlayerMovementCheatSafeguardPassedFSim(TargetPosition, myRigidbody.position))
                {
                    transform.LookAt(TargetPosition);
                    myRigidbody.MovePosition(TargetPosition);
                }
                else
                {
                    transformSender.TriggerTeleport(myRigidbody.position);
                }
            }
        }

        private bool ShouldMovePlayerFSim(Vector3 targetPosition, Vector3 currentPosition)
        {
            return playerInfo.Data.isAlive && (targetPosition - currentPosition).FlattenVector().sqrMagnitude > SimulationSettings.PlayerPositionUpdateMinSqrDistance;
        }

        private bool PlayerMovementCheatSafeguardPassedFSim(Vector3 targetPosition, Vector3 currentPosition)
        {
            var result = (targetPosition - currentPosition).sqrMagnitude < SimulationSettings.PlayerPositionUpdateMaxSqrDistance;
            if (!result)
            {
                Debug.LogError("Player movement cheat safeguard failed on FSim.");
            }
            return result;
        }
    }
}
