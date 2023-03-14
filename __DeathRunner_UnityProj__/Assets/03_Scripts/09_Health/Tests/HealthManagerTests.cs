using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

using I32 = System.Int32;
using U16 = System.UInt16;
using F32 = System.Single;

using Bool = System.Boolean;

namespace DeathRunner.Health.Tests
{
    public class HealthManagerTests
    {
        private HealthManager _healthManager;

        [SetUp]
        public void SetUp()
        {
            _healthManager = new GameObject().AddComponent<HealthManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_healthManager.gameObject);
        }

        [Test]
        public void HealthManager_ReserveHealth_AddsHealthToPool()
        {
            // Arrange
            HealthData __healthData = new HealthData(startingHealth: 100, maxHealth: 100, unitType: UnitType.All);
            
            // Act
            U16 __index = _healthManager.ReserveHealth(__healthData);
            HealthData __retrievedHealthData = _healthManager[__index];
            
            // Assert
            Assert.AreEqual(expected: __healthData, actual: __retrievedHealthData);
        }
        
        [Test]
        public void HealthManager_FreeHealth_SetsHealthToZero()
        {
            // Arrange
            HealthData __healthData = new HealthData(startingHealth: 100, maxHealth: 100, unitType: UnitType.All);
            U16 __index = _healthManager.ReserveHealth(__healthData);
            
            // Act
            _healthManager.FreeHealth(__index);
            HealthData __retrievedHealthData = _healthManager[__index];
            
            // Assert
            Assert.AreEqual(expected: -1, actual: __retrievedHealthData.current);
            Assert.AreEqual(expected:  0, actual: __retrievedHealthData.max);
            Assert.AreEqual(expected:  0, actual: __retrievedHealthData.primantissa);
        }

        [Test]
        public void HealthManager_QueueHealthChange_AddsChangeToQueue()
        {
            // Arrange
            HealthData __healthData = new HealthData(startingHealth: 50, maxHealth: 100, unitType: UnitType.All);
            U16 __index = _healthManager.ReserveHealth(__healthData);
            HealthChangeData __healthChangeData = new HealthChangeData(delta: +10, targetHealthIndex: __index, affectedUnitTypes: UnitType.All);
            
            // Act
            U16 __changeIndex = _healthManager.QueueHealthChange(__healthChangeData);
            HealthChangeData __retrievedChangeData = _healthManager.GetHealthChange(__changeIndex);
            
            // Assert
            Assert.AreEqual(expected: __healthChangeData.delta, actual: __retrievedChangeData.delta);
        }
        
        [UnityTest]
        public IEnumerator HealthManager_Execute_UpdatesHealth_SingleFrame()
        {
            const F32 STARTING_HEALTH = 50;
            const F32 MAX_HEALTH      = 150;
            const F32 DELTA           = +50;
            const F32 EXPECTED_HEALTH = STARTING_HEALTH + DELTA;
            const F32 EXPECTED_PRIM   = EXPECTED_HEALTH / MAX_HEALTH;

            // Arrange
            HealthData __healthData = new HealthData(startingHealth: STARTING_HEALTH, maxHealth: MAX_HEALTH, UnitType.Player);
            U16 __index = _healthManager.ReserveHealth(__healthData);
            
            HealthChangeData __healthChangeData = new HealthChangeData(delta: DELTA, targetHealthIndex: __index, durationInSeconds: 0, affectedUnitTypes: UnitType.All);
            _healthManager.QueueHealthChange(__healthChangeData);
            
            // Act
            //_healthManager.Execute();
            yield return null;
            
            // Assert
            HealthData __retrievedHealthData = _healthManager[__index];
            Assert.AreEqual(expected: EXPECTED_HEALTH, actual: __retrievedHealthData.current);
            Assert.AreEqual(expected: EXPECTED_PRIM,   actual: __retrievedHealthData.primantissa);
        }
        
        [UnityTest]
        public IEnumerator HealthManage_Execute_UpdatesHealth_OverTime()
        {
            const F32 MAX_HEALTH      = 500;
            const F32 DELTA           = +50;
            const F32 STARTING_HEALTH = 25;
            const F32 STARTING_PRIM   = STARTING_HEALTH / MAX_HEALTH;
            const F32 SECONDS_TO_WAIT = 2;
            const F32 EXPECTED_HEALTH = STARTING_HEALTH + (DELTA * SECONDS_TO_WAIT);
            const F32 EXPECTED_PRIM   = EXPECTED_HEALTH / MAX_HEALTH;
            
            // Arrange
            HealthData __healthData = new HealthData(startingHealth: STARTING_HEALTH, maxHealth: MAX_HEALTH, UnitType.Player);
            U16 __index = _healthManager.ReserveHealth(__healthData);
            HealthChangeData __healthChangeData = new HealthChangeData(delta: DELTA, targetHealthIndex: __index, durationInSeconds: 2, affectedUnitTypes: UnitType.All);
            _healthManager.QueueHealthChange(__healthChangeData);
            
            HealthData __healthDataBefore = _healthManager[__index];
            
            // Act (Wait for 2 seconds)
            yield return new WaitForSeconds(SECONDS_TO_WAIT);
            
            // Assert
            HealthData __healthDataAfter = _healthManager[__index];
            
            Assert.AreEqual(expected: STARTING_HEALTH, actual: __healthDataBefore.current);
            Assert.AreEqual(expected: STARTING_PRIM,   actual: __healthDataBefore.primantissa);
            
            Assert.AreEqual(expected: EXPECTED_HEALTH, actual: __healthDataAfter.current);
            Assert.AreEqual(expected: EXPECTED_PRIM,   actual: __healthDataAfter.primantissa);
            
            //HealthChangeData __retrievedChangeData = _healthManager.GetHealthChange(__changeIndex);
            //Assert.IsTrue(__retrievedChangeData.secondsLeft < __retrievedChangeData.durationInSeconds);
        }
    }
}