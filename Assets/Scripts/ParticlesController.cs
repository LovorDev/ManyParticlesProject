using System.Threading.Tasks;

using UnityEngine;

namespace DefaultNamespace
{
    public class ParticlesController : MonoBehaviour
    {
        [SerializeField] private Mesh _mesh;
        [SerializeField] private Material _material;
        
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
        
        private ComputeBuffer _argsBuffer;
        private readonly uint[] _args = { 0, 0, 0, 0, 0 };
        private ComputeBuffer _startPositionBuffer, _endPositionBuffer, _scaleBuffer;
        private int _cachedMultiplier = 1;
        private Vector4[] _positions1;
        private Vector4[] _positions2;

        private void Start()
        {
            _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            UpdateBuffers();
        }

        private void Update()
        {
            //todo - optimize it
            // var start = _startPoint.position;
            // var end = _endPoint.position;
            // Parallel.For(0, _count, i =>
            // {
            //     _positions1[i].Set(start.x, start.y, start.z, _positions1[i].w);
            //     _positions2[i].Set(end.x, end.y, end.z, _positions2[i].w);
            // });
            //
            // _startPositionBuffer.SetData(_positions1);
            // _endPositionBuffer.SetData(_positions2);
            // _material.SetBuffer("position_buffer_1", _startPositionBuffer);
            // _material.SetBuffer("position_buffer_2", _endPositionBuffer);

            Graphics.DrawMeshInstancedIndirect(_mesh, 0, _material, new Bounds(Vector3.zero, Vector3.one * 1000), _argsBuffer);
        }
        
        private void UpdateBuffers()
        {
            // Positions
            _startPositionBuffer?.Release();
            _endPositionBuffer?.Release();
            _scaleBuffer?.Release();
            _startPositionBuffer = new ComputeBuffer(_count, 16);
            _endPositionBuffer = new ComputeBuffer(_count, 16);
            _scaleBuffer = new ComputeBuffer(_count, 12);

            _positions1 = new Vector4[_count];
            _positions2 = new Vector4[_count];
            var scale = new Vector3[_count];

            for (var i = 0; i < _count; i++)
            {
                _positions1[i]= _startPoint.position;
                _positions2[i] = _endPoint.position;

                var range = Random.Range(.9f, 1.1f);
                _positions1[i].w = range * i * _timeDelay;
                _positions2[i].w = Random.Range(_randomScale.x, _randomScale.y);
                scale[i] = Random.insideUnitSphere;
            }
            

            _startPositionBuffer.SetData(_positions1);
            _endPositionBuffer.SetData(_positions2);
            _scaleBuffer.SetData(scale);
            _material.SetBuffer("position_buffer_1", _startPositionBuffer);
            _material.SetBuffer("position_buffer_2", _endPositionBuffer);
            _material.SetBuffer("scale_buffer", _scaleBuffer);

            // Verts
            _args[0] = _mesh.GetIndexCount(0);
            _args[1] = (uint)_count;
            _args[2] = _mesh.GetIndexStart(0);
            _args[3] = _mesh.GetBaseVertex(0);

            _argsBuffer.SetData(_args);
        }
        
        private void OnDisable()
        {
            _startPositionBuffer?.Release();
            _startPositionBuffer = null;

            _endPositionBuffer?.Release();
            _endPositionBuffer = null;

            _argsBuffer?.Release();
            _argsBuffer = null;
        }
    }
}