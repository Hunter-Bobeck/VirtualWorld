using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [EngineType(EnginePlatform.Client)]
    public class FootstepAudioTriggers : MonoBehaviour
    {
        [SerializeField] private AudioClip LeftFootStep;
        [SerializeField] private AudioClip RightFootStep;
        [SerializeField] private AudioSource audioSource;

        private void Awake()
        {
            audioSource = gameObject.GetComponentIfUnassigned(audioSource);
        }

        public void OnLeftFootStep()
        {
            audioSource.volume = SimulationSettings.FootstepVolume;
            audioSource.PlayOneShot(LeftFootStep);
        }

        public void OnRightFootStep()
        {
            audioSource.volume = SimulationSettings.FootstepVolume;
            audioSource.PlayOneShot(RightFootStep);
        }
    }
}
