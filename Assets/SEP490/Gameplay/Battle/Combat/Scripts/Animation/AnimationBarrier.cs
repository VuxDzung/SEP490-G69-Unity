namespace SEP490G69.Battle.Combat
{
    using System;

    public class AnimationBarrier
    {
        private int _remaining;
        private Action _onCompleted;

        public AnimationBarrier(int count, Action onCompleted)
        {
            _remaining = count;
            _onCompleted = onCompleted;
        }

        public AnimationBarrier()
        {
            _remaining = 0;
            _onCompleted = null;
        }

        public void SetCount(int count)
        {
            _remaining = count;
        }

        public void AddCount(int count)
        {
            _remaining += count;
        }

        public void SetOnCompletedCallback(Action onCompleted)
        {
            _onCompleted = onCompleted;
        }

        public void Signal()
        {
            _remaining--;

            if (_remaining <= 0)
            {
                _onCompleted?.Invoke();
            }
        }
    }
}