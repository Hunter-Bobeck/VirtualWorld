using Assets.Gamelogic.Core;
using Improbable.Communication;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Communication
{
    [EngineType(EnginePlatform.Client)]
    public class ChatVisualizer : MonoBehaviour
    {
        [Require] private Chat.Reader chat;

        [SerializeField] private NotificationController notification;

        private void Awake()
        {
            notification = gameObject.GetComponentCachedInChildren(notification);
            if (notification == null)
            {
                Debug.LogWarning("No notification controller!");
            }
            else
            {
                notification.HideNotification();
            }
        }

        private void OnEnable()
        {
            chat.ComponentUpdated += ComponentUpdated;
        }

        private void OnDisable()
        {
            chat.ComponentUpdated -= ComponentUpdated;

            if (notification != null)
            {
                notification.HideNotification();
            }
        }

        private void ComponentUpdated(Chat.Update update)
        {
            var lastIndex = update.chatSent.Count - 1;
            notification.ShowNotification(update.chatSent[lastIndex].message);
        }
    }
}
