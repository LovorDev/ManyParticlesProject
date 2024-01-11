using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Create ParticlesConfig", fileName = "ParticlesConfig", order = 0)]
    public class ParticlesConfig : ScriptableObject
    {
        public Mesh Mesh => _mesh;

        public Material Material => _material;

        public int Count => _count;

        public float TimeDelay => _timeDelay;

        public Vector2 RandomScale => _randomScale;

        public ComputeShader Compute => _compute;

        [SerializeField]
        private Mesh _mesh;

        [SerializeField]
        private Material _material;

        [SerializeField]
        private int _count;

        [SerializeField]
        private float _timeDelay = 0.1f;

        [SerializeField]
        private Vector2 _randomScale;

        [SerializeField]
        private ComputeShader _compute;
    }
}