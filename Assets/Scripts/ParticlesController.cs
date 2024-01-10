using UnityEngine;

namespace DefaultNamespace
{
    public class ParticlesController : MonoBehaviour
    {
        [SerializeField]
        private Mesh _mesh;

        [SerializeField]
        private Material _material;

        [SerializeField]
        private int _count;

        [SerializeField]
        private float _timeDelay = 0.1f;

        [SerializeField]
        private Transform _startPoint;

        [SerializeField]
        private Transform _endPoint;

        [SerializeField]
        private Vector2 _randomScale;

        [SerializeField]
        private ComputeShader _compute;

        private static readonly int StartPosition = Shader.PropertyToID("start_position");
        private static readonly int EndPosition = Shader.PropertyToID("end_position");
        private readonly uint[] _args = { 0, 0, 0, 0, 0 };

        private ComputeBuffer _argsBuffer;
        private ComputeBuffer _meshPropertiesBuffer;

        private int _kernel;
        private int _threadGroupsX;
        private static readonly int Data = Shader.PropertyToID("data");

        private void Start()
        {
            _threadGroupsX = Mathf.CeilToInt(_count / 64f);
            _kernel = _compute.FindKernel("cs_main");
            _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            UpdateBuffers();
        }

        private void Update()
        {
            var start = _startPoint.position;
            var end = _endPoint.position;
            _compute.SetVector(StartPosition, start);
            _compute.SetVector(EndPosition, end);

            _compute.Dispatch(_kernel, _threadGroupsX, 1, 1);

            Graphics.DrawMeshInstancedIndirect(_mesh, 0, _material, new Bounds(Vector3.zero, Vector3.one * 1000),
                _argsBuffer);
        }

        private void OnDisable()
        {
            _argsBuffer?.Release();
            _argsBuffer = null;

            _meshPropertiesBuffer?.Release();
            _meshPropertiesBuffer = null;
        }

        private void UpdateBuffers()
        {
            var data = new MeshData[_count];
            for (var i = 0; i < _count; i++)
            {
                var range = Random.Range(.9f, 1.1f);
                data[i] = new MeshData
                {
                    start = _startPoint.position,
                    end = _endPoint.position,
                    random = Random.insideUnitSphere,
                    time = new Vector2(range * i * _timeDelay, Random.Range(_randomScale.x, _randomScale.y)),
                };
            }

            _meshPropertiesBuffer = new ComputeBuffer(_count, 44);
            _meshPropertiesBuffer.SetData(data);

            _compute.SetBuffer(_kernel, Data, _meshPropertiesBuffer);
            _material.SetBuffer(Data, _meshPropertiesBuffer);

            _args[0] = _mesh.GetIndexCount(0);
            _args[1] = (uint)_count;
            _args[2] = _mesh.GetIndexStart(0);
            _args[3] = _mesh.GetBaseVertex(0);

            _argsBuffer.SetData(_args);
        }

        private struct MeshData
        {
            public Vector3 start;
            public Vector3 end;
            public Vector3 random;
            public Vector2 time;
        }
    }
}