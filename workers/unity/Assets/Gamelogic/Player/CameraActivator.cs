using Assets.Gamelogic.Core;
using Improbable.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    public class CameraActivator : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer authCheck;

        private void OnEnable()
        {
            MainCameraController.SetTarget(gameObject);
        }
    }
}
