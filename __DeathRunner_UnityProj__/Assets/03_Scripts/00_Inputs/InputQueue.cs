using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using F32  = System.Single;
using Bool = System.Boolean;

namespace DeathRunner.Inputs
{
    public readonly struct InputQueue<T> where T : struct
    {
        private readonly F32      _bufferTimeInSeconds;
        private readonly Queue<T> _inputQueue;
        private readonly T        _defaultValueToReturn;
        
        public InputQueue(F32 bufferTimeInSeconds)
        {
            this._bufferTimeInSeconds = bufferTimeInSeconds;
            this._inputQueue = new Queue<T>();
            this._defaultValueToReturn = default;
        }
        
        public InputQueue(F32 bufferTimeInSeconds, T customDefaultValueToReturn)
        {
            this._bufferTimeInSeconds = bufferTimeInSeconds;
            this._inputQueue = new Queue<T>();
            this._defaultValueToReturn = customDefaultValueToReturn;
        }

        public async UniTask Enqueue(T input)
        {
            this._inputQueue.Enqueue(input);
            
            await UniTask.Delay(TimeSpan.FromSeconds(_bufferTimeInSeconds), ignoreTimeScale: false);
            
            this._inputQueue.Dequeue();
        }
        
        public void Dequeue() => _inputQueue.Dequeue();

        public void Clear()
        {
            this._inputQueue.Clear();
        }
        
        public T Peek  => (_inputQueue.Count == 0) ? _defaultValueToReturn : _inputQueue.Peek();
        public T Value => (_inputQueue.Count == 0) ? _defaultValueToReturn : _inputQueue.Dequeue();
    }
}
