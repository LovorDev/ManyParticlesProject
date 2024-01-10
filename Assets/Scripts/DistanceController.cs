using System;

using Cysharp.Threading.Tasks.Linq;

using UnityEngine;

namespace DefaultNamespace
{
    public class DistanceController : IStart
    {
        public event Action<float> DistanceChanged = _ => { }; 
        
        private readonly BoxesModel _firstBoxesModel;
        private readonly BoxesModel _secondBoxesModel;
        private readonly DistanceShowView _showView;

        public DistanceController(BoxesModel firstBoxesModel, BoxesModel secondBoxesModel, DistanceShowView showView)
        {
            _firstBoxesModel = firstBoxesModel;
            _secondBoxesModel = secondBoxesModel;
            _showView = showView;
        }


        public void Start()
        {
            _firstBoxesModel.CubePosition
                .CombineLatest(_secondBoxesModel.CubePosition, Vector3.Distance)
                .Subscribe(OnDistanceChanged);
        }

        private void OnDistanceChanged(float distance)
        {
            _showView.SetText(distance.ToString("F1"));
            DistanceChanged(distance);
        }
    }
}