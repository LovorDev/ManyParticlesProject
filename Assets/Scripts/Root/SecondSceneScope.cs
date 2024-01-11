using UnityEngine;

using Utils;

namespace DefaultNamespace
{
    public class SecondSceneScope : ScopeBase
    {
        [SerializeField]
        private ParticlesConfig _particlesConfig;

        protected override void OnAwake()
        {
            var particleHelper = new ParticleHelper(_particlesConfig);
            Register(particleHelper);
            Register(new WholeSphereController(_particlesConfig, particleHelper));
        }
    }
}