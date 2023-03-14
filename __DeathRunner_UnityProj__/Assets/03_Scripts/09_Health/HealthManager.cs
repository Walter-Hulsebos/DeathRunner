using System;
using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using static Unity.Mathematics.math;

using JetBrains.Annotations;

using I32 = System.Int32;
using U16 = System.UInt16;
using F32 = System.Single;

using Bool = System.Boolean;

namespace DeathRunner.Health
{
    public sealed class HealthManager : MonoBehaviour
    {
        //private NativeArray<HealthData>       _healthPool    = new NativeArray<HealthData>(      length: 1024, allocator: Allocator.Persistent);
        private NativeArray<HealthData>[]     _healthPools   = new NativeArray<HealthData>[2];
        private NativeArray<HealthChangeData> _healthChanges = new NativeArray<HealthChangeData>(length: 4096, allocator: Allocator.Persistent);
        
        //private NativeArray<HealthChangeData>[] _healthChangesBuffers = new NativeArray<HealthChangeData>[2];

        private I32 _currentBufferIndex;


        //TODO: Actually handle single-frame health changes. Right now their health change is a per-second value.

        public HealthData this[U16 index] => _healthPools[_currentBufferIndex][index];
        //public HealthData this[I32 index] => _healthPool[index];

        private void Start()
        {
            _healthPools[0] = new NativeArray<HealthData>(length: 1024, allocator: Allocator.Persistent);
            _healthPools[1] = new NativeArray<HealthData>(length: 1024, allocator: Allocator.Persistent);
            
            //_healthChangesBuffers[0] = new NativeArray<HealthChangeData>(length: 4096, allocator: Allocator.Persistent);
            //_healthChangesBuffers[1] = new NativeArray<HealthChangeData>(length: 4096, allocator: Allocator.Persistent);
            _currentBufferIndex = 0;
        }


        private void Update() => Execute();

        public void Execute()
        {
            HandleQueuedHealthChanges();
            
            //RemoveAllEmptyHealths();
        }

        private void HandleQueuedHealthChanges()
        {
            // Get the read and write arrays for the current buffer index
            //NativeArray<HealthChangeData> __readHealthChanges  = _healthChangesBuffers[_currentBufferIndex];
            //NativeArray<HealthChangeData> __writeHealthChanges = _healthChangesBuffers[(_currentBufferIndex + 1) % 2];
            
            NativeArray<HealthData> __readHealthPool  = _healthPools[_currentBufferIndex];
            NativeArray<HealthData> __writeHealthPool = _healthPools[(_currentBufferIndex + 1) % 2];

            // Run the job
            HealthChangeJob __healthChangeJob = new HealthChangeJob
            {
                changes      = _healthChanges,
                healthsRead  = __readHealthPool,
                healthsWrite = __writeHealthPool,
                deltaTime    = Time.deltaTime,
            };
            JobHandle __healthChangeJobHandle = __healthChangeJob.Schedule(arrayLength: _healthChanges.Length, innerloopBatchCount: 64);
            __healthChangeJobHandle.Complete();

            // Swap the read and write arrays
            _healthPools[_currentBufferIndex] = __writeHealthPool;
            //_healthChangesBuffers[_currentBufferIndex] = __writeHealthChanges;
            _currentBufferIndex = (_currentBufferIndex + 1) % 2;
        }


        // private void RemoveAllEmptyHealths()
        // {
        //     
        // }

        /// <summary>
        /// Will reserve a health index in the health pool.
        /// If the health pool is full, it will resize the health pool and return the new index.
        /// </summary>
        /// <returns></returns>
        [PublicAPI]
        public U16 ReserveHealth(HealthData healthData)
        {
            
            
            for (U16 __index = 0; __index < _healthPools[_currentBufferIndex].Length; __index += 1)
            {
                HealthData __health = _healthPools[_currentBufferIndex][__index];
                
                Bool __isHealthEmpty = (__health.current <= 0);
                if (__isHealthEmpty)
                {
                    _healthPools[_currentBufferIndex][__index] = healthData;
                    return __index;
                }
            }
            
            //If we get here, it means that the array is full and we need to resize it
            U16 __oldSize = (U16)_healthPools[_currentBufferIndex].Length;
            I32 __newSize = (_healthPools[_currentBufferIndex].Length * 2);
            if (__newSize > U16.MaxValue)
            {
                throw new System.Exception(message: "Health pool array is too big! (65535 is the max size of a U16 array)");
            }

            Debug.LogWarning(message: "Resizing health pool array, this basically shouldn't happen! It's a very expensive operation!", context: this);
            NativeArray<HealthData> __newHealthPool = new NativeArray<HealthData>(length: __newSize, allocator: Allocator.Persistent);

            //Copy the data over.
            for (U16 __index = 0; __index < __oldSize; __index += 1)
            {
                __newHealthPool[__index] = _healthPools[_currentBufferIndex][__index];
            }
            __newHealthPool[__oldSize] = healthData;
            
            _healthPools[_currentBufferIndex].Dispose();
            _healthPools[_currentBufferIndex] = __newHealthPool;

            return __oldSize;
        }
        
        [PublicAPI]
        public void FreeHealth(U16 index)
        {
            //TODO: Consider doing this in update instead of here? In case of race conditions?
            _healthPools[_currentBufferIndex][index].Clear();
        }

        [PublicAPI]
        public U16 QueueHealthChange(HealthChangeData healthChangeData)
        {
            //NOTE: [Walter]
            //Will iterate over health changes and find the slot where the health change time is < 0.
            //If it finds a slot where the health change time is <= 0, it will replace that slot with the new health change.
            //If it doesn't find a slot where the health change time is <= 0, it will resize the array and add the new health change to the beginning of that.
            //REVIEW: Review this logic? Perhaps we should simply ignore the health change if the array is full?
            
            for (U16 __index = 0; __index < _healthChanges.Length; __index += 1)
            {
                HealthChangeData __healthChange = _healthChanges[__index];
                
                //NOTE: [Walter] See how we're not checking if the secondsLeft is <= 0?
                //That's because we always want it to at least run one frame.
                Bool __healthChangeIsFinished = (__healthChange.secondsLeft < 0);
                if (__healthChangeIsFinished)
                {
                    _healthChanges[__index] = healthChangeData;
                    return __index;
                }
            }
            
            //If we get here, it means that the array is full and we need to resize it. 
            U16 __oldSize = (U16)_healthChanges.Length;
            I32 __newSize = (_healthChanges.Length * 2);
            if (__newSize >= U16.MaxValue)
            {
                throw new Exception(message: "Health changes array is too big! (65535 is the max size of a U16 array)");
            }
            
            Debug.LogWarning(message: "Resizing health changes array, this should not happen often! It's a very expensive operation!", context: this);
            NativeArray<HealthChangeData> __newHealthChanges = new NativeArray<HealthChangeData>(length: __newSize, allocator: Allocator.Persistent);
            
            //Copy the data over.
            for (U16 __index = 0; __index < __oldSize; __index += 1)
            {
                __newHealthChanges[__index] = _healthChanges[__index];
            }
            __newHealthChanges[__oldSize] = healthChangeData;
            
            _healthChanges.Dispose();
            _healthChanges = __newHealthChanges;

            return __oldSize;
        }
        
        [PublicAPI]
        public HealthChangeData GetHealthChange(U16 index)
        {
            return _healthChanges[index];
        }
    }
    
    [BurstCompile]
    public struct HealthChangeJob : IJobParallelFor
    {
        #region Variables
    
        public NativeArray<HealthChangeData> changes;
        [ReadOnly]  public NativeArray<HealthData> healthsRead;
        [WriteOnly] public NativeArray<HealthData> healthsWrite;

        [ReadOnly] public F32 deltaTime;
        
    
        #endregion
    
        #region Methods
    
        public void Execute(I32 index)
        {
            HealthChangeData __change = changes[index];
            HealthData       __health = healthsRead[__change.targetHealthIndex];
            
            //NOTE: [Walter] See how we're not checking if the secondsLeft is <= 0?
            //That's because we always want it to at least run one frame.
            Bool __hasNoTimeLeft = (__change.secondsLeft < 0);
            if (__hasNoTimeLeft) return;
            
            Bool __healthIsEmpty = (__health.current <= 0);
            if (__healthIsEmpty) return;
            
            Bool __healthIsFull = (__health.current >= __health.max);
            if (__healthIsFull) return;

            Bool __cannotAffectUnitType = ((__change.affectedUnitTypes & __health.unitType) == 0);
            if (__cannotAffectUnitType)
            {
                __change.secondsLeft = -1; //Ignore this health change.
                return;
            }

            F32 __changeAmount = (__change.isInstant) 
                ? __change.delta 
                : __change.delta * deltaTime;

            F32 __newHealth = __health.current + __changeAmount;
            
            //Clamp the new health value, so it doesn't go over the max health.
            __health.current = min(__newHealth, __health.max);
            //Calculate the primantissa, which is the current health divided by the max health.
            __health.primantissa = clamp(__health.current / __health.max, 0, 1);
            
            __change.secondsLeft -= deltaTime;
            
            healthsWrite[__change.targetHealthIndex] = __health;
        }
            
        #endregion
            
    }
}