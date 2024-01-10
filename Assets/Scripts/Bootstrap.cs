using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Object = UnityEngine.Object;

namespace DefaultNamespace
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField]
        private Transform _firstCube;

        [SerializeField]
        private Transform _secondCube;

        [SerializeField]
        private ParticlesConfig _particlesConfig;

        [SerializeField]
        private DistanceShowView _distanceShowView;


        private readonly List<object> _objects = new();
        private List<ITick> _tickable = new();
        private List<IStart> _startable = new();
        private List<IDisposable> _disposables = new();


        private void Awake()
        {
            var first = CreateMovement(new Dictionary<KeyCode, Vector3>
            {
                [KeyCode.W] = Vector3.forward,
                [KeyCode.A] = Vector3.left,
                [KeyCode.S] = Vector3.back,
                [KeyCode.D] = Vector3.right,
            }, _firstCube);

            var second = CreateMovement(new Dictionary<KeyCode, Vector3>
            {
                [KeyCode.UpArrow] = Vector3.forward,
                [KeyCode.LeftArrow] = Vector3.left,
                [KeyCode.DownArrow] = Vector3.back,
                [KeyCode.RightArrow] = Vector3.right,
            }, _secondCube);

            _objects.Add(new ParticlesController(_particlesConfig, first, second));
            _objects.Add(new DistanceController(first, second, _distanceShowView));

            _tickable = _objects.OfType<ITick>().ToList();
            _startable = _objects.OfType<IStart>().ToList();
            _disposables = _objects.OfType<IDisposable>().ToList();
        }

        private void Start()
        {
            foreach (var start in _startable)
            {
                start.Start();
            }
        }

        private void Update()
        {
            foreach (var tick in _tickable)
            {
                tick.Tick();
            }
        }

        private void OnDestroy()
        {
            _tickable = null;
            _startable = null;
            for (var i = 0; i < _disposables.Count; i++)
            {
                _disposables[i].Dispose();
                _disposables[i] = null;
            }

            _disposables = null;
        }

        private BoxesModel CreateMovement(Dictionary<KeyCode, Vector3> dictionary, Transform firstCube)
        {
            var input = new InputController(dictionary);
            var boxModel = new BoxesModel(firstCube.position);
            _objects.Add(new PositionSetController(input, boxModel, .05f));
            _objects.Add(new CubesTranslator(boxModel, firstCube));
            _objects.Add(input);
            _objects.Add(boxModel);
            return boxModel;
        }
    }
}