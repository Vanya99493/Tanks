using ShellModule;
using UnityEngine;

namespace Infrastructure.FactoryModule
{
    public class ShellFactory
    {
        public ShellView SpawnShell(ShellView shellPrefab, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            return Object.Instantiate(shellPrefab, spawnPosition, spawnRotation);
        }
    }
}