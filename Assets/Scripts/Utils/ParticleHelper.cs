using System;

using DefaultNamespace;

using UnityEngine;

namespace Utils
{
    public class ParticleHelper : IDisposable, ITick
    {
        private static readonly int StartPosition = Shader.PropertyToID("start_position");
        private static readonly int EndPosition = Shader.PropertyToID("end_position");
        private static readonly int Data = Shader.PropertyToID("data");

        private readonly uint[] _args = { 0, 0, 0, 0, 0 };
        private readonly ParticlesConfig _particlesConfig;
        private ComputeBuffer _argsBuffer;
        private ComputeBuffer _meshPropertiesBuffer;
        private int _kernel;
        private int _threadGroupsX;

        public ParticleHelper(ParticlesConfig particlesConfig)
        {
            _particlesConfig = particlesConfig;
        }

        public void ChangePosition(Vector3 start, Vector3 end)
        {
            _particlesConfig.Compute.SetVector(StartPosition, start);
            _particlesConfig.Compute.SetVector(EndPosition, end);
            _particlesConfig.Compute.Dispatch(_kernel, _threadGroupsX, 1, 1);
        }

        public void Init()
        {
            _threadGroupsX = Mathf.CeilToInt(_particlesConfig.Count / 64f);
            _kernel = _particlesConfig.Compute.FindKernel("cs_main");
            _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        }

        public void SetData(Array data)
        {
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

        public void Tick()
        {
            Graphics.DrawMeshInstancedIndirect(_particlesConfig.Mesh, 0, _particlesConfig.Material,
                new Bounds(Vector3.zero, Vector3.one * 1000), _argsBuffer);
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
    }
}