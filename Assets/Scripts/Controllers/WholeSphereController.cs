using UnityEngine;

using Utils;

namespace DefaultNamespace
{
    public class WholeSphereController : IStart
    {
        private readonly ParticlesConfig _particlesConfig;
        private readonly ParticleHelper _particleHelper;

        public WholeSphereController(ParticlesConfig particlesConfig, ParticleHelper particleHelper)
        {
            _particlesConfig = particlesConfig;
            _particleHelper = particleHelper;
        }

        public void Start()
        {
            _particleHelper.Init();
            UpdateBuffers();
        }

        private void UpdateBuffers()
        {
            var data = new MeshData[_particlesConfig.Count];
            for (var i = 0; i < _particlesConfig.Count; i++)
            {
                var range = Random.Range(.9f, 1.1f);
                data[i] = new MeshData
                {
                    start = Random.insideUnitSphere,
                    end = Random.insideUnitSphere,
                    random = Random.insideUnitSphere,
                    time = new Vector2(range * i * _particlesConfig.TimeDelay,
                        Random.Range(_particlesConfig.RandomScale.x, _particlesConfig.RandomScale.y)),
                    inverted = (ushort)(Random.value > .5f ? 1 : 0),
                };
            }

            _particleHelper.SetData(data);
        }
    }
}