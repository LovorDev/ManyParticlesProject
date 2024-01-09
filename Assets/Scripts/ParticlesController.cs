using UnityEngine;

namespace DefaultNamespace
{
    // [ExecuteInEditMode]
    public class ParticlesController : MonoBehaviour
    {
        [SerializeField] private Mesh _mesh;
        [SerializeField] private Material _material;
        
        [SerializeField]
        private Transform _cubePrefab;

        [SerializeField]
        private int _count;

        [SerializeField]
        private float _timeDelay = 0.1f;

        [SerializeField]
        private Transform _startPoint;

        [SerializeField]
        private Transform _endPoint;

        [SerializeField]
        private float _speedScale = 1;

        [SerializeField]
        private float _noiseScale;

        private Vector3[] _startPositions;
        private Vector3[] _endPositions;
        private float[] _delay;


        private Vector3[] _spawnedCubes;
        private Matrix4x4[] _matrices;
        private RenderParams _renderParams;

        private void Start()
        {
            _spawnedCubes = new Vector3[_count];
            _startPositions = new Vector3[_count];
            _endPositions = new Vector3[_count];
            _delay = new float[_count];
            _matrices = new Matrix4x4[_count];
            
            _renderParams = new RenderParams(_material);

            Spawn();
        }

        private void Update()
        {
            for (var i = 0; i < _spawnedCubes.Length; i++)
            {
                var (pos, rot) = CalculatePosition(_startPoint.position + _startPositions[i],
                    _endPoint.position + _endPositions[i],
                    (Time.time + _delay[i] * i) * _speedScale);

                _matrices[i].SetTRS(pos, rot, Vector3.one * .5f);
                _spawnedCubes[i] = pos;
            }

            Graphics.RenderMeshInstanced(_renderParams, _mesh, 0, _matrices);
        }

        [ContextMenu("Spawn")]
        private void Spawn()
        {
            for (var i = 0; i < _count; i++)
            {
                _startPositions[i] = Random.insideUnitSphere;
                _endPositions[i] = Random.insideUnitSphere * .5f;
                _delay[i] = Random.Range(_timeDelay * .8f, _timeDelay * 1.2f);
                _spawnedCubes[i] = _startPoint.position + _startPositions[i];
            }
        }

        private (Vector3 pos, Quaternion rot) CalculatePosition(Vector3 startPosition, Vector3 endPosition, float time)
        {
            var sin = Mathf.Repeat(time, 1f);
            var strength = 1 - Mathf.Abs(2 * (sin - 0.5f));
            var pos = Vector3.Lerp(startPosition, endPosition, sin);
            pos += Mathf.PerlinNoise(time, time) * _noiseScale * strength *
                   Vector3.Cross(endPosition - startPosition, Vector3.up);
            return (pos, Quaternion.identity);
        }
    }
}