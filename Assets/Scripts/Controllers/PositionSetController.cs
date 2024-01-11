using Cysharp.Threading.Tasks.Linq;

using UnityEngine;

namespace DefaultNamespace
{
    public class PositionSetController : IStart
    {
        private readonly InputController _inputController;
        private readonly BoxesModel _boxesModel;
        private readonly float _speed;

        public PositionSetController(InputController inputController, BoxesModel boxesModel, float speed)
        {
            _inputController = inputController;
            _boxesModel = boxesModel;
            _speed = speed;
        }

        public void Start()
        {
            _inputController.AxisTriggered.Subscribe(OnNext);
        }

        private void OnNext(Vector3 obj)
        {
            _boxesModel.CubePosition.Value += obj * _speed;
        }
    }
}