using System;

namespace SEP490G69
{
    public class Timer
    {
        private bool _isRunning;
        private ETimerType _timerType;
        private Action<float> onUpdate;
        public Action<Timer> OnExpired;
        public Action<ETimerOperation, float> OnChange;
        public bool IsRunning => Value > 0;

        public float Value { get; private set; }

        public Timer(ETimerType timerType = ETimerType.Countdown)
        {
            _timerType = timerType;
            switch (timerType)
            {
                case ETimerType.Countdown:
                    onUpdate = UpdateCoutdown;
                    break;
                case ETimerType.Stopwatch:
                    onUpdate = UpdateStopwatch;
                    break;
            }
        }

        public void StartTimer(float value, bool addRemaining = true)
        {
            if (Value > 0 && addRemaining)
            {
                Value += value;
            }
            else
            {
                Value = value;
                OnChange?.Invoke(ETimerOperation.Start, value);
            }
            _isRunning = true;
        }
        public void PauseTimer()
        {
            _isRunning = false;
            OnChange?.Invoke(ETimerOperation.Paused, Value);
        }
        public void UnpauseTimer()
        {
            if (Value <= 0) return;
            _isRunning = false;
            OnChange?.Invoke(ETimerOperation.Unpaused, Value);
        }
        public void StopTimer()
        {
            if (_isRunning)
            {
                _isRunning = false;
                OnExpired?.Invoke(this);
                Value = 0;
            }
        }

        public void Update(float deltaTime)
        {
            if (_isRunning)
            {
                onUpdate?.Invoke(deltaTime);
            }
        }
        private void UpdateCoutdown(float deltaTime)
        {
            Value -= deltaTime;
            OnChange?.Invoke(ETimerOperation.Running, Value);
            if (Value <= 0)
            {
                Value = 0;
                _isRunning = false;
                OnExpired?.Invoke(this);
                OnChange?.Invoke(ETimerOperation.Finished, Value);
            }
        }
        private void UpdateStopwatch(float deltaTime)
        {
            Value += deltaTime;
            OnChange?.Invoke(ETimerOperation.Running, Value);
        }
    }
}