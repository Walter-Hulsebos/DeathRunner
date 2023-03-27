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
        
        //NOTE: [Walter] We're using the double buffer pattern here, to avoid race conditions.
        private readonly NativeArray<HealthData>[]       _healthPools          = new NativeArray<HealthData>[2];
        private readonly NativeArray<HealthChangeData>[] _healthChangesBuffers = new NativeArray<HealthChangeData>[2];

        private I32 _currentBufferIndex;


        //TODO: Actually handle single-frame health changes. Right now their health change is a per-second value.

        public HealthData this[U16 index] => _healthPools[_currentBufferIndex][index];
        //public HealthData this[I32 index] => _healthPool[index];

        private void OnEnable()
        {
            _healthPools[0] = new NativeArray<HealthData>(length: 1024, allocator: Allocator.Persistent);
            _healthPools[1] = new NativeArray<HealthData>(length: 1024, allocator: Allocator.Persistent);
            
            _healthChangesBuffers[0] = new NativeArray<HealthChangeData>(length: 4096, allocator: Allocator.Persistent);
            _healthChangesBuffers[1] = new NativeArray<HealthChangeData>(length: 4096, allocator: Allocator.Persistent);
            _currentBufferIndex = 0;
        }

        private void OnDestroy()
        {
            _healthPools[0].Dispose();
            _healthPools[1].Dispose();
            
            _healthChangesBuffers[0].Dispose();
            _healthChangesBuffers[1].Dispose();
        }

        private void Update() => Execute();

        public void Execute()
        {
            HandleQueuedHealthChanges();
        }
        
        private void HandleQueuedHealthChanges()
        {
            // Get the read and write arrays for the current buffer index
            NativeArray<HealthChangeData> __readHealthChanges  = _healthChangesBuffers[_currentBufferIndex];
            NativeArray<HealthChangeData> __writeHealthChanges = _healthChangesBuffers[(_currentBufferIndex + 1) % 2];

            NativeArray<HealthData> __healthPoolsRead  = _healthPools[_currentBufferIndex];
            NativeArray<HealthData> __healthPoolsWrite = _healthPools[(_currentBufferIndex + 1) % 2];

            // Run the job
            HealthChangeJob __healthChangeJob = new HealthChangeJob
            {
                changesRead  = __readHealthChanges,
                changesWrite = __writeHealthChanges,
                healthsRead  = __healthPoolsRead,
                healthsWrite = __healthPoolsWrite,
                deltaTime    = Time.deltaTime,
            };
            JobHandle __healthChangeJobHandle = __healthChangeJob.Schedule(arrayLength: __readHealthChanges.Length, innerloopBatchCount: 64);
            __healthChangeJobHandle.Complete();

            // Swap the read and write arrays
            _healthPools[_currentBufferIndex] = __healthPoolsWrite;
            _healthChangesBuffers[_currentBufferIndex] = __writeHealthChanges;
            _currentBufferIndex = (_currentBufferIndex + 1) % 2;
        }

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
            
            Debug.LogError(message: "Exceeded max health pool size!", context: this);

            return 0;
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
            for (U16 __index = 0; __index < _healthChangesBuffers[_currentBufferIndex].Length; __index += 1)
            {
                HealthChangeData __healthChange = _healthChangesBuffers[_currentBufferIndex][__index];

                if (__healthChange.IsFinished)
                {
                    //Replaced finished health change with the new one.
                    _healthChangesBuffers[_currentBufferIndex][__index] = healthChangeData;
                    return __index;
                }
            }
            
            Debug.LogError(message: "Exceeded max health changes size!", context: this);
            return 0;
        }
        
        [PublicAPI]
        public HealthChangeData GetHealthChange(U16 index)
        {
            return _healthChangesBuffers[_currentBufferIndex][index];
        }
    }
    
    [BurstCompile]
    public struct HealthChangeJob : IJobParallelFor
    {
        #region Variables
    
        [ReadOnly]  public NativeArray<HealthChangeData> changesRead;
        [WriteOnly] public NativeArray<HealthChangeData> changesWrite;
        
        [ReadOnly]  public NativeArray<HealthData> healthsRead;
        [WriteOnly] public NativeArray<HealthData> healthsWrite;

        [ReadOnly] public F32 deltaTime;

        #endregion
    
        #region Methods
    
        public void Execute(I32 index)
        {
            HealthChangeData __change = changesRead[index];
            HealthData       __health = healthsRead[__change.targetHealthIndex];

            if (__change.IsFinished) return;
            __change.secondsLeft -= deltaTime;
            
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

            F32 __changeAmount = (__change.isSingleFrame) 
                ? __change.delta 
                : __change.delta * deltaTime;

            F32 __newHealth = __health.current + __changeAmount;
            
            //Clamp the new health value, so it doesn't go over the max health.
            __health.current = min(__newHealth, __health.max);
            //Calculate the primantissa, which is the current health divided by the max health.
            __health.primantissa = clamp(__health.current / __health.max, 0, 1);
            
            changesWrite[index] = __change;
            healthsWrite[__change.targetHealthIndex] = __health;
        }
            
        #endregion
            
    }
}