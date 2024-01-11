using System;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class SceneSwitchController : IStart, IDisposable
    {
        private const float DistanceToSwitch = 1;
        private readonly DistanceController _distanceController;
        private AsyncOperation _loadSceneAsync;

        public SceneSwitchController(DistanceController distanceController)
        {
            _distanceController = distanceController;
        }

        public void Start()
        {
            _distanceController.DistanceChanged += OnDistanceChanged;
        }

        private void OnDistanceChanged(float obj)
        {
            if (obj < DistanceToSwitch && _loadSceneAsync == null)
            {
                LoadNewSceneAsync().Forget();
            }
        }

        private async UniTaskVoid LoadNewSceneAsync()
        {
            _loadSceneAsync = SceneManager.LoadSceneAsync("SecondScene", LoadSceneMode.Additive);
            await SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("FirstScene"));
            await _loadSceneAsync;
        }

        public void Dispose()
        {
            _distanceController.DistanceChanged -= OnDistanceChanged;
        }
    }
}