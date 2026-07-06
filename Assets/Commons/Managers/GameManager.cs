using System;
using UnityEngine;

namespace Evolution.Commons.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public bool IsPaused;
        public bool IsSimulationRunning = false;

        public event Action OnPause;
        public event Action OnUnPause;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                return;
            }
            Destroy(this);
        }
        //Roll a 50/50
        public bool CoinFlip()
        {
            return UnityEngine.Random.Range(0, 2) != 0;
        }
        //Returns the roll of a percentage, percentage 80, means there is an 80 percente chance to roll true
        public bool RollDice(int percentage)
        {
            if (percentage > 100) return true;
            if (percentage <= 0) return false;

            return UnityEngine.Random.Range(0, 100) < percentage;
        }
        public void Pause()
        {
            if (IsPaused)
            {
                IsPaused = false;
                OnUnPause?.Invoke();
            }
            else
            {
                IsPaused = true;
                OnPause?.Invoke();
            }
        }
        
    }
}
