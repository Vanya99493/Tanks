using TankModule;
using UnityEngine;

namespace Infrastructure.FactoryModule
{
    public class TankFactory
    {
        private int _currentId;

        public TankFactory(int currentId = 1)
        {
            _currentId = currentId;
        }
        
        public Tank SpawnTank(TankView tankPrefab, Vector3 spawnPosition, Quaternion spawnRotation, int health, Color tankColor)
        {
            TankView tankView = Object.Instantiate(tankPrefab, spawnPosition, spawnRotation);
            
            Tank tank = new Tank(tankView, _currentId++, health, tankColor);

            return tank;
        }
    }
}