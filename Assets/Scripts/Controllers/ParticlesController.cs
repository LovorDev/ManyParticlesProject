using Cysharp.Threading.Tasks.Linq;

using UnityEngine;

using Utils;

using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class ParticlesController : IStart
    {
        private readonly ParticlesConfig _particlesConfig;
        private readonly BoxesModel _firstCubeModel;
        private readonly BoxesModel _secondCubeModel;

        private readonly ParticleHelper _particleHelper;

        public ParticlesController(ParticlesConfig particlesConfig, BoxesModel firstCubeModel,
            BoxesModel secondCubeModel, ParticleHelper particleHelper)
        {
            _particlesConfig = particlesConfig;
            _firstCubeModel = firstCubeModel;
            _secondCubeModel = secondCubeModel;
            _particleHelper = particleHelper;
        }

        void IStart.Start()
        {
            _particleHelper.Init();
            UpdateBuffers();
            _firstCubeModel.CubePosition
                .CombineLatest(_secondCubeModel.CubePosition, (vector3, vector4) => (vector3, vector4))
                .Subscribe(OnPositionChanged);
        }

        private void OnPositionChanged((Vector3 start, Vector3 end) position)
        {
            _particleHelper.ChangePosition(position.start, position.end);
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

            _particleHelper.SetData(data);
        }
    }
}