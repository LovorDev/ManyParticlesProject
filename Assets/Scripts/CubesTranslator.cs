using Cysharp.Threading.Tasks.Linq;

using UnityEngine;

namespace DefaultNamespace
{
    public class CubesTranslator : IStart
    {
        private readonly BoxesModel _boxesModel;
        private readonly Transform _cube;

        public CubesTranslator(BoxesModel boxesModel, Transform cube)
        {
            _boxesModel = boxesModel;
            _cube = cube;
        }

        public void Start()
        {
            _boxesModel.CubePosition.WithoutCurrent().Subscribe(OnNext);
        }

        private void OnNext(Vector3 position)
        {
            _cube.position = position;
        }
    }

    public interface IStart
    {
        public void Start();
    }
}