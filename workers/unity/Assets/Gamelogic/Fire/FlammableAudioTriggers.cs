using Assets.Gamelogic.Core;
using Improbable.Fire;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Fire
{
    [EngineType(EnginePlatform.Client)]
    public class FlammableAudioTriggers : MonoBehaviour
    {
        [Require] private Flammable.Reader flammable;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip Ignite;
        [SerializeField] private AudioClip Extinguish;
        [SerializeField] private AudioClip Fire;

        private void Awake()
        {
            audioSource = gameObject.GetComponentIfUnassigned(audioSource);
        }

        private void OnEnable()
        {
            flammable.ComponentUpdated += OnFireChange;
        }

        private void OnDisable()
        {
            flammable.ComponentUpdated -= OnFireChange;
        }

        private void OnFireChange(Flammable.Update fireChange)
        {
            if (fireChange.isOnFire.HasValue)
            {
                if (fireChange.isOnFire.Value)
                {
                    TriggerIgnitionSound();
                    StartFireAudio();
                }
                else
                {
                    StopFireAudio();
                    TriggerExtinguishSound();
                }
            }
        }

        public void TriggerIgnitionSound()
        {
            audioSource.volume = SimulationSettings.IgnitionVolume;
            audioSource.PlayOneShot(Ignite);
        }

        private void StartFireAudio()
        {
            audioSource.clip = Fire;
            audioSource.volume = SimulationSettings.FireVolume;
            audioSource.loop = true;
            audioSource.Play();
        }

        private void StopFireAudio()
        {
            audioSource.loop = false;
            audioSource.Stop();
        }

        public void TriggerExtinguishSound()
        {
            audioSource.volume = SimulationSettings.ExtinguishVolume;
            audioSource.PlayOneShot(Extinguish);
        }
    }
}
