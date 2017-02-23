using Improbable.Unity;
using Improbable.Unity.Configuration;
using Improbable.Unity.Core;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    /// <summary>
    /// Manages the lifecycle of the connection to SpatialOS as a worker, such as connection and disconnection.
    /// </summary>
    public class Bootstrap : MonoBehaviour
    {
        public WorkerConfigurationData Configuration = new WorkerConfigurationData();

        private void Start()
        {
            SpatialOS.ApplyConfiguration(Configuration);

            Time.fixedDeltaTime = 1.0f / SimulationSettings.FixedFramerate;

            switch (SpatialOS.Configuration.EnginePlatform)
            {
                case EnginePlatform.FSim:
                    Application.targetFrameRate = SimulationSettings.TargetFramerateFSim;
                    SpatialOS.OnDisconnected += reason => Application.Quit();
                    break;
                case EnginePlatform.Client:
                    Application.targetFrameRate = SimulationSettings.TargetFramerate;
                    SpatialOS.OnConnected += ClientPlayerSpawner.SpawnPlayer;
                    break;
            }
            SpatialOS.Connect(gameObject);
        }

        private void OnApplicationQuit()
        {
            if (SpatialOS.IsConnected)
            {
                SpatialOS.Disconnect();
            }
        }
    }
}
