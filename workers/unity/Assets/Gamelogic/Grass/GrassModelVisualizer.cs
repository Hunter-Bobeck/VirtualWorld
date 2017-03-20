using System.Collections;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable.Grass;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Grass
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class GrassModelVisualizer : MonoBehaviour
    {
        [Require] private GrassState.Reader grassState;

        [SerializeField] private GameObject UneatenGrass;
        [SerializeField] private GameObject Eaten;
        [SerializeField] private GameObject BurntGrass;
        [SerializeField] private Mesh[] meshes;

        private void OnEnable()
        {
            /*SetupGrassModel();*/
            grassState.ComponentUpdated += UpdateVisualization;
            ShowGrassModel(grassState.Data.currentState);
        }

        private void OnDisable()
        {
            grassState.ComponentUpdated -= UpdateVisualization;
        }

        private void SetupGrassModel()
        {
            var grassModel = meshes[(int)grassState.Data.grassType];
            UneatenGrass.GetComponent<MeshFilter>().mesh = grassModel;
        }

        private void UpdateVisualization(GrassState.Update newState)
        {
            ShowGrassModel(newState.currentState.Value);
        }

        private void ShowGrassModel(GrassFSMState currentState)
        {
            switch (currentState)
            {
                case GrassFSMState.UNEATEN:
                    StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.GrassExtinguishTimeBuffer, () =>
                    {
                        TransitionTo(UneatenGrass);
                    }));
                    break;
                case GrassFSMState.EATEN:
                    StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.GrassCutDownTimeBuffer, () =>
                    {
                        TransitionTo(Eaten); 
                    }));
                    break;
                case GrassFSMState.BURNING:
                    StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.GrassIgnitionTimeBuffer, () =>
                    {
                        TransitionTo(UneatenGrass);
                    }));
                    break;
                case GrassFSMState.BURNT:
                    TransitionTo(BurntGrass);
                    break;
            }
        }

        private void TransitionTo(GameObject newModel)
        {
            HideAllModels();
            newModel.SetActive(true);
        }

        private void HideAllModels()
        {
            UneatenGrass.SetActive(false);
            Eaten.SetActive(false);
            BurntGrass.SetActive(false);
        }
    }
}
