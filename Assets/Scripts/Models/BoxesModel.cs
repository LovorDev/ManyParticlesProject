using Cysharp.Threading.Tasks;

using UnityEngine;

namespace DefaultNamespace
{
    public class BoxesModel
    {
        public AsyncReactiveProperty<Vector3> CubePosition { get; }

        public BoxesModel(Vector3 initValue = default)
        {
            CubePosition = new AsyncReactiveProperty<Vector3>(initValue);
        }
    }
}