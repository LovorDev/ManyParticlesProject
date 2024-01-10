using System.Collections.Generic;

using UnityEngine;

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
        

        private readonly List<ITick> _tickable = new();
        private readonly List<IStart> _startable = new();


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

            var particlesController = new ParticlesController(_particlesConfig,first,second);
            _startable.Add(particlesController);
            _tickable.Add(particlesController);
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

        private BoxesModel CreateMovement(Dictionary<KeyCode, Vector3> dictionary, Transform firstCube)
        {
            var input = new InputController(dictionary);
            var boxModel = new BoxesModel(firstCube.position);
            _startable.Add(new PositionSetController(input, boxModel, .05f));
            _startable.Add(new CubesTranslator(boxModel, firstCube));
            _tickable.Add(input);
            return boxModel;
        }
    }
}