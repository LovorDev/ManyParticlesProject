using System.Collections.Generic;

using UnityEngine;

using Utils;

namespace DefaultNamespace
{
    public class FirstSceneScope : ScopeBase
    {
        [SerializeField]
        private Renderer _firstCube;

        [SerializeField]
        private Renderer _secondCube;

        [SerializeField]
        private ParticlesConfig _particlesConfig;

        [SerializeField]
        private DistanceShowView _distanceShowView;

        [SerializeField]
        private Renderer[] _spheres;

        [SerializeField]
        private Texture[] _textures;

        [SerializeField]
        private GameObject _sphereParent;


        protected override void OnAwake()
        {
            InitComponents();
        }

        private void InitComponents()
        {
            var first = CreateMovement(new Dictionary<KeyCode, Vector3>
            {
                [KeyCode.W] = Vector3.forward,
                [KeyCode.A] = Vector3.left,
                [KeyCode.S] = Vector3.back,
                [KeyCode.D] = Vector3.right,
                [KeyCode.LeftShift] = Vector3.up,
                [KeyCode.LeftControl] = Vector3.down,
            }, _firstCube, Color.green);

            var second = CreateMovement(new Dictionary<KeyCode, Vector3>
            {
                [KeyCode.UpArrow] = Vector3.forward,
                [KeyCode.LeftArrow] = Vector3.left,
                [KeyCode.DownArrow] = Vector3.back,
                [KeyCode.RightArrow] = Vector3.right,
                [KeyCode.LeftShift] = Vector3.up,
                [KeyCode.LeftControl] = Vector3.down,
            }, _secondCube, Color.red);
            var particleHelper = new ParticleHelper(_particlesConfig);
            Register(particleHelper);
            Register(new ParticlesController(_particlesConfig, first, second, particleHelper));
            var distanceController = new DistanceController(first, second, _distanceShowView);
            Register(distanceController);
            Register(new SpheresController(_spheres, _textures, distanceController, _sphereParent));
            Register(new SceneSwitchController(distanceController));
        }

        private BoxesModel CreateMovement(Dictionary<KeyCode, Vector3> dictionary, Renderer firstCube, Color color)
        {
            var input = new InputController(dictionary);
            var boxModel = new BoxesModel(firstCube.transform.position);
            Register(new PositionSetController(input, boxModel, .05f));
            Register(new CubesController(boxModel, firstCube, color));
            Register(input);
            Register(boxModel);
            return boxModel;
        }
    }
}