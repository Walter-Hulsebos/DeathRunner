using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using F32  = System.Single;
using I32  = System.Int32;
using Bool = System.Boolean;

namespace DeathRunner.Inputs
{
    public struct InputQueue<T> where T : struct
    {
        private readonly F32      _bufferTimeInSeconds;
        private readonly Queue<T> _inputQueue;
        private readonly T        _defaultValueToReturn;
        //private          I32      _manuallyDequeuedCount;
        
        public InputQueue(F32 bufferTimeInSeconds)
        {
            this._bufferTimeInSeconds = bufferTimeInSeconds;
            this._inputQueue = new Queue<T>();
            this._defaultValueToReturn = default;
            //this._manuallyDequeuedCount = 0;
        }
        
        public InputQueue(F32 bufferTimeInSeconds, T customDefaultValueToReturn)
        {
            this._bufferTimeInSeconds = bufferTimeInSeconds;
            this._inputQueue = new Queue<T>();
            this._defaultValueToReturn = customDefaultValueToReturn;
            //this._manuallyDequeuedCount = 0;
        }

        public async UniTask Enqueue(T input)
        {
            this._inputQueue.Enqueue(input);
            
            // Remove the oldest input after their buffer time has passed.
            await UniTask.Delay(TimeSpan.FromSeconds(_bufferTimeInSeconds), ignoreTimeScale: false);

            if (_inputQueue.Count > 0)
            {
                this._inputQueue.Dequeue();
            }
        }
        
        public void Dequeue() => _inputQueue.Dequeue();
        public void Clear()   => _inputQueue.Clear();

        public T Peek  => (_inputQueue.Count == 0) ? _defaultValueToReturn : _inputQueue.Peek();
        public T Value => (_inputQueue.Count == 0) ? _defaultValueToReturn : _inputQueue.Dequeue();
    }
}