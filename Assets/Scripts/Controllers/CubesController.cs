using Cysharp.Threading.Tasks.Linq;

using UnityEngine;

namespace DefaultNamespace
{
    public class CubesController : IStart
    {
        private static readonly int ColorKey = Shader.PropertyToID("_Color");
        
        private readonly BoxesModel _boxesModel;
        private readonly Renderer _cube;
        private readonly Color _color;

        public CubesController(BoxesModel boxesModel, Renderer cube, Color color)
        {
            _boxesModel = boxesModel;
            _cube = cube;
            _color = color;
        }

        public void Start()
        {
            _boxesModel.CubePosition.WithoutCurrent().Subscribe(OnNext);
            var block = new MaterialPropertyBlock();
            block.SetColor(ColorKey, _color);
            _cube.SetPropertyBlock(block);
        }

        private void OnNext(Vector3 position)
        {
            _cube.transform.position = position;
        }
    }
}