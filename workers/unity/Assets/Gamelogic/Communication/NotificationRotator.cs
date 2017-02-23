using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Communication
{
    [EngineType(EnginePlatform.Client)]
    public class NotificationRotator : MonoBehaviour
    {
        private void LateUpdate()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                transform.forward = Camera.main.transform.forward;
            }
        }
    }
}