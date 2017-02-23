using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Communication
{
    [EngineType(EnginePlatform.FSim)]
    public class ChatBroadcasterBehaviour : MonoBehaviour
    {
        private void OnEnable()
        {
            //Todo: Handle sent chat messages here!
        }

        private void OnDisable()
        {
	        //Todo: Tidy up after chat implementation here!
        }
    }
}
