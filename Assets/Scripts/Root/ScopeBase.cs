using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace DefaultNamespace
{
    public abstract class ScopeBase : MonoBehaviour
    {
        private readonly List<object> _objects = new();
        private List<IDisposable> _disposables = new();
        private List<ITick> _tickable = new();
        private List<IStart> _startable = new();

        private void Awake()
        {
            OnAwake();
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

        protected void Register(object obj)
        {
            _objects.Add(obj);
        }

        protected abstract void OnAwake();
    }
}