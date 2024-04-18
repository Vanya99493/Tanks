using System.Collections;
using CameraModule;
using Infrastructure.Configs;
using Infrastructure.FactoryModule;
using TankModule;
using UIModule;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure
{
    public class Game : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private RoundRulesConfig roundRulesConfig;
        [SerializeField] private CameraMover cameraMover;
        [SerializeField] private UIController uiController;
        [Header("Tank setup")]
        [SerializeField] private Transform[] spawnPositions;
        [SerializeField] private TankView tankPrefab;
        [SerializeField] private Color[] tanksColors;

        private Tank[] _tanks;
        private int _roundNumber;

        private void Start()
        {
            InstantiateTanks();
            SetCameraTargets();

            StartCoroutine(RoundStarting());
        }

        private void InstantiateTanks()
        {
            _tanks = new Tank[spawnPositions.Length];
            TankFactory tankFactory = new TankFactory();
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                _tanks[i] = tankFactory.SpawnTank(tankPrefab, spawnPositions[i].position, 
                    spawnPositions[i].rotation, 100, 
                    tanksColors[i]);
                _tanks[i].Death += OnTankDestroy;
            }
        }

        private void SetCameraTargets()
        {
            cameraMover.SetTargets(_tanks);
        }

        private IEnumerator RoundStarting()
        {
            ResetAllTanks();
            DisableTanksControl();

            cameraMover.SetStartPosition();

            _roundNumber++;
            uiController.ActivatePreRoundMessage(_roundNumber);

            yield return new WaitForSeconds(roundRulesConfig.StartDelay);
            
            EnableTanksControl();
            uiController.DeactivateMessage();
        }

        private void OnTankDestroy(Tank destroyedTank)
        {
            Tank winnerTank = null;
            int index = -1;
            for (int i = 0; i < _tanks.Length; i++)
            {
                if (_tanks[i].Id != destroyedTank.Id)
                {
                    winnerTank = _tanks[i];
                    index = i;
                    break;
                }
            }

            winnerTank.AddWin();
            
            StartCoroutine(RoundEnding(winnerTank, index));
        }

        private IEnumerator RoundEnding(Tank winnerTank, int winnerTankIndex)
        {
            DisableTanksControl();
            uiController.ActivateEndRoundMessage(winnerTankIndex, _tanks);
            
            yield return new WaitForSeconds(roundRulesConfig.EndDelay);
            
            if (winnerTank.Wins >= roundRulesConfig.RoundsToWinNumber)
                SceneManager.LoadScene(0);
            else
                StartCoroutine(RoundStarting());
        }

        private void ResetAllTanks()
        {
            for (int i = 0; i < _tanks.Length; i++)
            {
                _tanks[i].Reset(spawnPositions[i].position);
            }
        }

        private void EnableTanksControl()
        {
            for (int i = 0; i < _tanks.Length; i++)
            {
                _tanks[i].EnableControl();
            }
        }

        private void DisableTanksControl()
        {
            for (int i = 0; i < _tanks.Length; i++)
            {
                _tanks[i].DisableControl();
            }
        }
    }
}