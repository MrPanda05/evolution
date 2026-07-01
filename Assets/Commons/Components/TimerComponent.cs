using UnityEngine;
using System;
using Evolution.Commons.Managers;

namespace Evolution.Commons.Components
{
    public class TimerComponent : MonoBehaviour
    {
        [SerializeField]
        private float _duration;
        [SerializeField]
        private bool _startOnAwake;
        [SerializeField]
        private bool _loop;
        [SerializeField]
        private float _timer;
        private bool _isRunning;
        public event Action OnTimeFinished;
        public float Elapsed => Mathf.Max(0f, _duration - _timer);
        public float Remaining => Mathf.Max(0f, _timer);
        public float CurrentDuration => _duration;
        public bool IsRunning => _isRunning;
        private float _leftOver = 0;
        
        private void Awake()
        {
            if (_startOnAwake)
            {
                StartTimer();
            }
        }
        private void OnEnable()
        {
            GameManager.Instance.OnPause += Pause;
            GameManager.Instance.OnUnPause += UnPause;
        }
        private void OnDisable()
        {
            GameManager.Instance.OnPause -= Pause;
            GameManager.Instance.OnUnPause -= UnPause;
        }
        private void Pause()
        {
            _leftOver = Remaining;
            StopTimerWithoutEvent();
        }
        public void Loop()
        {
            _loop = true;
        }
        public void StopLooping()
        {
            _loop = false;
        }
        private void UnPause()
        {
            float previousTime = _duration;
            if (_leftOver > 0)
            {
                StartTimer(_leftOver);
                _leftOver = 0;
                _duration = previousTime;
            }
        }
        public void StartTimer()
        {
            _timer = _duration;
            _isRunning = true;
        }
        public void StartTimer(float duration)
        {
            _duration = duration;
            StartTimer();
        }
        private void TimerTimeOut()
        {
            _isRunning = false;
            OnTimeFinished?.Invoke();
        }
        public void StopTimer()
        {
            _timer = _duration;
            TimerTimeOut();
        }
        public void StopTimerWithoutEvent()
        {
            _timer = _duration;
            _isRunning = false;
        }
        public void ResetTimer()
        {
            _timer = _duration;
        }
        public void ResetTimerAndStop()
        {
            _timer = _duration;
            _isRunning = false;
        }
        public void SetDuration(float duration)
        {
            if (_isRunning)
            {
                Debug.LogWarning("Changing timer while running might be a bad idea");
            }
            _duration = duration;
        }
        private void Update()
        {
            if (!_isRunning) return;
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                TimerTimeOut();
                if (_loop)
                {
                    //print("Looping");
                    _timer = _duration;
                    _isRunning = true;
                }
            }
        }
    }
}
