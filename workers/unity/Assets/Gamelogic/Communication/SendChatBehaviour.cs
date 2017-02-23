using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Communication
{
    [EngineType(EnginePlatform.Client)]
    public class SendChatBehaviour : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer authCheck;
        
        public void SayChat(string message)
        {
			//Todo: Send a chat message here!
        }
    }
}