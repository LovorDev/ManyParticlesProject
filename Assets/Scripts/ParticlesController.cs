using System;
using System.Linq;

using Cysharp.Threading.Tasks.Linq;

using UnityEngine;

using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class ParticlesController : IStart, ITick, IDisposable
    {
        private static readonly int StartPosition = Shader.PropertyToID("start_position");
        private static readonly int EndPosition = Shader.PropertyToID("end_position");
        private static readonly int Data = Shader.PropertyToID("data");

        private readonly uint[] _args = { 0, 0, 0, 0, 0 };
        private readonly ParticlesConfig _particlesConfig;
        private readonly BoxesModel _firstCubeModel;
        private readonly BoxesModel _secondCubeModel;

        private ComputeBuffer _argsBuffer;
        private ComputeBuffer _meshPropertiesBuffer;

        private int _kernel;
        private int _threadGroupsX;

        public ParticlesController(ParticlesConfig particlesConfig, BoxesModel firstCubeModel,
            BoxesModel secondCubeModel)
        {
            _particlesConfig = particlesConfig;
            _firstCubeModel = firstCubeModel;
            _secondCubeModel = secondCubeModel;
        }

        void IStart.Start()
        {
            _threadGroupsX = Mathf.CeilToInt(_particlesConfig.Count / 64f);
            _kernel = _particlesConfig.Compute.FindKernel("cs_main");
            _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            UpdateBuffers();
            _firstCubeModel.CubePosition
                .CombineLatest(_secondCubeModel.CubePosition, (vector3, vector4) => (vector3, vector4))
                .Subscribe(OnPositionChanged);
        }

        public void Tick()
        {
            Graphics.DrawMeshInstancedIndirect(_particlesConfig.Mesh, 0, _particlesConfig.Material,
                new Bounds(Vector3.zero, Vector3.one * 1000),
                _argsBuffer);
        }

        private void OnPositionChanged((Vector3 start, Vector3 end) position)
        {
            _particlesConfig.Compute.SetVector(StartPosition, position.start);
            _particlesConfig.Compute.SetVector(EndPosition, position.end);

            _particlesConfig.Compute.Dispatch(_kernel, _threadGroupsX, 1, 1);
        }

        public void Dispose()
        {
            _argsBuffer?.Release();
            _argsBuffer?.Dispose();
            _argsBuffer = null;

            _meshPropertiesBuffer?.Release();
            _meshPropertiesBuffer?.Dispose();
            _meshPropertiesBuffer = null;
        }

        private void UpdateBuffers()
        {
            var data = new MeshData[_particlesConfig.Count];
            for (var i = 0; i < _particlesConfig.Count / 2; i++)
            {
                var range = Random.Range(.9f, 1.1f);
                data[i] = new MeshData
                {
                    random = Random.insideUnitSphere,
                    time = new Vector2(range * i * _particlesConfig.TimeDelay,
                        Random.Range(_particlesConfig.RandomScale.x, _particlesConfig.RandomScale.y)),
                };
            }

            for (var i = _particlesConfig.Count / 2; i < _particlesConfig.Count; i++)
            {
                var range = Random.Range(.9f, 1.1f);
                data[i] = new MeshData
                {
                    random = Random.insideUnitSphere,
                    time = new Vector2(range * i * _particlesConfig.TimeDelay,
                        Random.Range(_particlesConfig.RandomScale.x, _particlesConfig.RandomScale.y)),
                    inverted = 1,
                };
            }

            _meshPropertiesBuffer = new ComputeBuffer(_particlesConfig.Count, 48);
            _meshPropertiesBuffer.SetData(data);

            _particlesConfig.Compute.SetBuffer(_kernel, Data, _meshPropertiesBuffer);
            _particlesConfig.Material.SetBuffer(Data, _meshPropertiesBuffer);

            _args[0] = _particlesConfig.Mesh.GetIndexCount(0);
            _args[1] = (uint)_particlesConfig.Count;
            _args[2] = _particlesConfig.Mesh.GetIndexStart(0);
            _args[3] = _particlesConfig.Mesh.GetBaseVertex(0);

            _argsBuffer.SetData(_args);
        }

        private struct MeshData
        {
            public Vector3 start;
            public Vector3 end;
            public Vector3 random;
            public Vector2 time;
            public ushort inverted;
        }
    }
}