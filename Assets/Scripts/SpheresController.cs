using System;

using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace DefaultNamespace
{
    public class SpheresController : IStart, IDisposable
    {
        private static readonly int Textures = Shader.PropertyToID("_Textures");
        private static readonly int TextureIndex = Shader.PropertyToID("_TextureIndex");
        
        private readonly Renderer[] _spheres;
        private readonly Texture[] _texture;
        private readonly DistanceController _distanceController;
        private readonly GameObject _spheresParent;

        private bool _enabled;

        public SpheresController(Renderer[] spheres, Texture[] texture, DistanceController distanceController,
            GameObject sphereParent)
        {
            _spheres = spheres;
            _texture = texture;
            _distanceController = distanceController;
            _spheresParent = sphereParent;
        }


        public void Start()
        {
            _distanceController.DistanceChanged += OnDistanceChanged;
            SetUpTextures();
        }


        public void Dispose()
        {
            _distanceController.DistanceChanged -= OnDistanceChanged;
        }

        private void OnDistanceChanged(float distance)
        {
            var enabling = distance < 10;
            if ((enabling && _enabled) || (!enabling && !_enabled))
            {
                return;
            }

            _enabled = enabling;

            _spheresParent.SetActive(_enabled);
        }

        private void SetUpTextures()
        {
            var propertyBlock = new MaterialPropertyBlock();

            var width = _texture[0].width;
            var height = _texture[0].height;
            var depth = _texture.Length;

            var texture2DArray = new Texture2DArray(width, height, depth, _texture[0].graphicsFormat,
                TextureCreationFlags.None);

            for (var i = 0; i < depth; i++)
            {
                Graphics.CopyTexture(_texture[i], 0, 0, texture2DArray, i, 0);
            }

            _spheres[0].sharedMaterial.SetTexture(Textures, texture2DArray);
            for (var i = 0; i < _spheres.Length; i++)
            {
                propertyBlock.SetFloat(TextureIndex, i);
                _spheres[i].SetPropertyBlock(propertyBlock);
            }
        }
    }
}