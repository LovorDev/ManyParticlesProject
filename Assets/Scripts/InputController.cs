using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace DefaultNamespace
{
    public class InputController: ITick
    {
        public AsyncReactiveProperty<Vector3> AxisTriggered { get; } = new(default);
        private Vector3 _pressedKeys = new();

        private KeyCode[] _possibleKeys = new[]
        {
            KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D,
            KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow,
        };

        private Dictionary<KeyCode, Vector3> _keyMapBinding;
        public InputController(Dictionary<KeyCode, Vector3> keyMapBinding)
        {
            _keyMapBinding = keyMapBinding;
        }

        public void Tick()
        {
            _pressedKeys.Set(0,0,0);
            foreach (var keyCode in _keyMapBinding)
            {
                if (Input.GetKey(keyCode.Key))
                {
                    _pressedKeys += keyCode.Value;
                }
            }

            if (_pressedKeys != Vector3.zero)
            {
                AxisTriggered.Value = _pressedKeys;
            }
        }
    }

    public interface ITick
    {
        public void Tick();
    }
}