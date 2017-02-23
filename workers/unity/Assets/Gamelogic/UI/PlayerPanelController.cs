using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class PlayerPanelController : MonoBehaviour
    {
        [SerializeField] private Image playerHealthIcon;
        private static PlayerPanelController instance;
        
        private void Awake()
        {
            instance = this;
        }

        public static void SetPlayerHealth(float fill)
        {
            instance.playerHealthIcon.fillAmount = fill;
        }
    }
}
